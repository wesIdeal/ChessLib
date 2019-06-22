using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.MoveValidation;

namespace ChessLib.Data
{
    public class MoveTranslatorService
    {
        private readonly IBoard _board;
        public MoveTranslatorService(IBoard board)
        {
            _board = board;
        }

        public MoveTranslatorService(string fen)
        {
            _board = new BoardInfo(fen);
        }

        #region RegEx strings
        private const string RegExPieces = "[NBRQK]";
        private const string RegExFiles = "[a-h]";
        private const string RegExRanks = "[1-8]";
        private static readonly string RegExMoveDetails = $"((?<piece>{RegExPieces})((?<sourceFile>{RegExFiles})|(?<sourceRank>{RegExRanks}))?|(?<pawnFile>{RegExFiles}))(?<capture>x)?(?<destinationFile>{RegExFiles})(?<destinationRank>{RegExRanks})|((?<pawnFile>{RegExFiles})(?<destinationRank>{RegExRanks}))";
        private const string RegExCastleLongGroup = "castleLong";
        private const string RegExPromotion = "(?<sourceFile>[a-h])(((?<capture>x)(?<destinationFile>[a-h])(?<destinationRank>[1-8]))|(?<destinationRank>[1-8]))=(?<promotionPiece>[NBRQK])";
        private const string RegExCastleShortGroup = "castleShort";
        private static readonly string RegExCastle = $"(?<{RegExCastleLongGroup}>O-O-O)|(?<{RegExCastleShortGroup}>O-O)";
        #endregion


        /// <summary>
        /// Given a board and a SAN, generate a move object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="board"></param>
        /// <param name="moveText"></param>
        /// <returns></returns>
        public MoveExt GenerateMoveFromText(string moveText)
        {

            var md = GetAvailableMoveDetails(moveText, _board.ActivePlayer);
            if (!md.DestinationIndex.HasValue)
            {
                throw new MoveException("Move detail error: no destination index was supplied.");
            }
            if (!md.SourceFile.HasValue || !md.SourceRank.HasValue)
            {
                md.SourceIndex = FindPieceSourceIndex(md);
                if (md.SourceIndex == null)
                {
                    throw new MoveException("Move detail error: no piece found at source.");
                }
            }

            if (IsEnPassantCapture(md))
            {
                md.MoveType = MoveType.EnPassant;
            }

            var source = md.SourceIndex.Value;
            var destination = md.DestinationIndex.Value;
            var moveType = md.MoveType;
            var promotionPiece = md.PromotionPiece ?? PromotionPiece.Knight;
            var moveExt = MoveHelpers.GenerateMove(source, destination, moveType, promotionPiece);
            return moveExt;
        }

        /// <summary>
        /// Create move from long alg. notation
        /// 
        /// </summary>
        /// <example>e2e4 from inital position is 1. e4. e7e8q would be e8=Q</example>
        /// <param name="lanMove"></param>
        /// <remarks>
        /// Move will need to be validated against a board, as it can only know if the move is normal or promotion.
        /// It will not be aware of En Passant captures or Castling.
        /// This constructor's main use is to interpret moves from an engine and not for normal use.
        /// </remarks>
        /// <returns>A basic move that needs to be validated against a board of pieces. Null if the move source and/or destination are invalid.</returns>
        public static MoveExt FromLANMove(string lanMove)
        {
            var moveType = MoveType.Normal;
            var promotionPiece = PromotionPiece.Knight;
            var source = lanMove.Substring(0, 2).SquareTextToIndex();
            var dest = lanMove.Substring(2, 2).SquareTextToIndex();
            if (source == null || dest == null) { return null; }
            if (lanMove.Length == 5)
            {
                moveType = MoveType.Promotion;
                var promotionPieceChar = lanMove[4];
                promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionPieceChar);
            }
            return MoveHelpers.GenerateMove(source.Value, dest.Value, moveType, promotionPiece);
        }

        private bool IsEnPassantCapture(MoveDetail md)
        {
            bool isPawnCapture = md.IsCapture && md.Piece == Piece.Pawn;
            bool isDestinationRankEnPassantRank = md.DestinationRank == 2 || md.DestinationRank == 5;
            bool doesOpponentOccupyDestinationSquare = DoesOpponentOccupySquare(md.DestinationIndex.Value);
            return (isPawnCapture && !doesOpponentOccupyDestinationSquare && isDestinationRankEnPassantRank);
        }

        private bool DoesOpponentOccupySquare(ushort square)
        {
            var totalOpponentOccupancy = _board.OpponentOccupancy();
            return (totalOpponentOccupancy & (1ul << square)) == 1;
        }

        /// <summary>
        /// Gets move details by using regex. Second step in translating SAN -> ushort move.
        /// </summary>
        /// <param name="move">SAN move</param>
        /// <param name="color">Active player</param>
        public static MoveDetail GetAvailableMoveDetails(string move, Color color)
        {
            MoveDetail md = new MoveDetail() { MoveText = move, Color = color };
            if (move.Length < 2) throw new System.Exception("Invalid move. Must have at least 2 characters.");
            Match promotionMatch, castleMatch;
            var match = Regex.Match(move, RegExMoveDetails);
            md.SourceFile = match.Groups["sourceFile"].Success ? (ushort)(match.Groups["sourceFile"].Value[0] - 'a') : (ushort?)null;
            md.SourceRank = match.Groups["sourceRank"].Success ? (ushort)(ushort.Parse(match.Groups["sourceRank"].Value) - 1) : (ushort?)null;
            md.DestinationFile = match.Groups["destinationFile"].Success ? (ushort)(match.Groups["destinationFile"].Value[0] - 'a') : (ushort?)null;
            md.DestinationRank = match.Groups["destinationRank"].Success ? (ushort)(ushort.Parse((match.Groups["destinationRank"].Value)) - 1) : (ushort?)null;
            md.IsCapture = match.Groups["capture"].Success;

            if ((promotionMatch = Regex.Match(move, RegExPromotion)).Success)
            {
                md.MoveType = MoveType.Promotion;
                md.Piece = Piece.Pawn;
                md.PromotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionMatch.Groups["promotionPiece"].Value[0]);
            }
            if ((castleMatch = Regex.Match(move, RegExCastle)).Success)
            {
                md.Piece = Piece.King;
                md.MoveType = MoveType.Castle;
                md.SourceFile = 4;
                md.DestinationRank = md.SourceRank = (ushort)(color == Color.Black ? 7 : 0);
                md.DestinationFile = castleMatch.Groups[RegExCastleLongGroup].Success ? (ushort?)2 : (ushort?)6;
                return md;
            }

            if (match.Groups["pawnFile"].Success)
            {
                md.Piece = Piece.Pawn;

                if (md.IsCapture)
                {
                    md.SourceFile = (ushort)(match.Groups["pawnFile"].Value[0] - 'a');
                    if (color == Color.Black)
                    {
                        Debug.Assert(md.DestinationRank != null, "md.DestinationRank != null");
                        md.SourceRank = (ushort)(md.DestinationRank.Value + 1);
                    }
                    else
                    {
                        Debug.Assert(md.DestinationRank != null, "md.DestinationRank != null");
                        md.SourceRank = (ushort)(md.DestinationRank.Value - 1);
                    }
                }
                else
                {
                    md.DestinationFile = match.Groups["pawnFile"].Success ? (ushort)(match.Groups["pawnFile"].Value[0] - 'a') : (ushort?)null;
                }

            }
            else
            {
                var pieceMatch = match.Groups["piece"];
                md.Piece = PieceHelpers.GetPiece(pieceMatch.Value[0]);
            }

            return md;
        }

        /// <summary>
        ///     Used by the Find[piece]MoveSourceIndex to find the source of a piece moving parsed from SAN text.
        /// </summary>
        /// <param name="md">Available Move details</param>
        /// <param name="pieceMoveMask">The move mask for the piece</param>
        /// <param name="pieceOccupancy">The occupancy for the piece in question</param>
        /// <returns></returns>
        private ushort? FindPieceMoveSourceIndex(MoveDetail md, ulong pieceMoveMask, ulong pieceOccupancy)
        {
            ulong sourceSquares = pieceMoveMask & pieceOccupancy;
            if (sourceSquares == 0)
            {
                return null;
            }

            if (md.SourceFile != null)
            {
                sourceSquares &= BoardHelpers.FileMasks[md.SourceFile.Value];
            }

            if (md.SourceRank != null)
            {
                sourceSquares &= BoardHelpers.RankMasks[md.SourceRank.Value];
            }
            var sourceIndices = sourceSquares.GetSetBits();

            if (sourceIndices.Length == 0) return null;
            if (sourceIndices.Length > 1)
            {
                var possibleSources = new List<ushort>();

                foreach (var sourceIndex in sourceIndices)
                {
                    if (!md.DestinationIndex.HasValue) throw new MoveException("No destination value provided.");
                    var proposedMove = MoveHelpers.GenerateMove(sourceIndex, md.DestinationIndex.Value, md.MoveType, md.PromotionPiece ?? PromotionPiece.Knight);
                    var moveValidator = new MoveValidator(_board, proposedMove);
                    var validationResult = moveValidator.Validate();
                    if (validationResult == null)
                        possibleSources.Add(sourceIndex);
                }
                if (possibleSources.Count > 1) throw new MoveException("More than one piece can reach destination square.");
                if (possibleSources.Count == 0) return null;
                return possibleSources[0];
            }
            return sourceIndices[0];
        }

        /// <summary>
        ///     Find's a piece's source index, given some textual clues, such as piece type, color, and destination
        /// </summary>
        /// <param name="moveDetail">Details of move, gathered from text description (SAN)</param>
        /// <returns>The index from which the move was made.</returns>
        /// <exception cref="MoveException">
        ///     Thrown when the source can't be determined, piece on square cannot be determined, more
        ///     than one piece of type could reach destination, or piece cannot reach destination.
        /// </exception>
        private ushort? FindPieceSourceIndex(MoveDetail moveDetail)
        {
            var activePlayer = _board.ActivePlayer;
            var pieceOccupancy = _board.GetPiecePlacement().Occupancy(activePlayer, moveDetail.Piece);
            var totalBoardOccupancy = _board.TotalOccupancy();
            return FindPieceMoveSourceIndex(moveDetail, pieceOccupancy, totalBoardOccupancy);
        }

    }
}
