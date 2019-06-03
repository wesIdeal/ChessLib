using System;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChessLib.Data.Helpers
{
    public static class MoveHelpers
    {
        private const string RegExPieces = "[NBRQK]";
        private const string RegExFiles = "[a-h]";
        private const string RegExRanks = "[1-8]";
        private static readonly string RegExMoveDetails = $"((?<piece>{RegExPieces})((?<sourceFile>{RegExFiles})|(?<sourceRank>{RegExRanks}))?|(?<pawnFile>{RegExFiles}))(?<capture>x)?(?<destinationFile>{RegExFiles})(?<destinationRank>{RegExRanks})|((?<pawnFile>{RegExFiles})(?<destinationRank>{RegExRanks}))";
        private const string RegExCastleLongGroup = "castleLong";
        private const string RegExPromotion = "(?<sourceFile>[a-h])(((?<capture>x)(?<destinationFile>[a-h])(?<destinationRank>[1-8]))|(?<destinationRank>[1-8]))=(?<promotionPiece>[NBRQK])";
        private const string RegExCastleShortGroup = "castleShort";
        private static readonly string RegExCastle = $"(?<{RegExCastleLongGroup}>O-O-O)|(?<{RegExCastleShortGroup}>O-O)";

        public static ulong IndexToValue(this ushort idx) => (ulong)0x01 << idx;

        public static MoveExt GenerateMove(ushort fromIdx, ushort toIdx, MoveType moveType = MoveType.Normal, PromotionPiece promotionPiece = 0)
        {
            var mt = (ushort)moveType << 14;
            var pp = (ushort)promotionPiece << 12;
            var origin = fromIdx << 6;
            var dest = toIdx << 0;
            return new MoveExt((ushort)(mt | pp | origin | dest));
        }



        public static MoveExt GenerateMoveFromText<T>(this IBoard board, string moveText) where T : MoveValidatorBase
        {
            var md = MoveHelpers.GetAvailableMoveDetails(moveText, board.ActivePlayer);
            if (!md.SourceFile.HasValue || !md.SourceRank.HasValue)
            {
                var sourceIndex = FindPieceSourceIndex<T>(board, md);
                md.SourceIndex = sourceIndex ?? throw new MoveException("Move detail error: no piece found at source.");
            }

            Debug.Assert(md.SourceIndex != null, "md.SourceIndex != null");
            Debug.Assert(md.DestinationIndex != null, "md.DestinationIndex != null");
            if (md.IsCapture && md.Piece == Piece.Pawn &&
                (board.OpponentOccupancy() & (1ul << md.DestinationIndex)) == 0 &&
                (md.DestinationRank == 2 || md.DestinationRank == 5)) md.MoveType = MoveType.EnPassant;
            var moveExt = MoveHelpers.GenerateMove(md.SourceIndex.Value, md.DestinationIndex.Value, md.MoveType,
                md.PromotionPiece ?? 0);
            return moveExt;
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
        private static ushort? FindPieceSourceIndex<T>(IBoard board, MoveDetail moveDetail) where T : MoveValidatorBase
        {
            return moveDetail.Piece == Piece.Pawn ?
                // FindPawnMoveSourceIndex(board, moveDetail):
                FindPieceMoveSourceIndex<T>(board, moveDetail,
                    board.GetPiecePlacement().Occupancy(board.ActivePlayer, moveDetail.Piece), board.TotalOccupancy()) :
                FindPieceMoveSourceIndex<T>(board, moveDetail,
                 board.GetPiecePlacement().Occupancy(board.ActivePlayer, moveDetail.Piece), board.TotalOccupancy());
        }

        /// <summary>
        ///     Used by the Find[piece]MoveSourceIndex to find the source of a piece moving parsed from SAN text.
        /// </summary>
        /// <param name="md">Available Move details</param>
        /// <param name="pieceMoveMask">The move mask for the piece</param>
        /// <param name="pieceOccupancy">The occupancy for the piece in question</param>
        /// <returns></returns>
        private static ushort? FindPieceMoveSourceIndex<T>(IBoard board, MoveDetail md, ulong pieceMoveMask, ulong pieceOccupancy) where T : MoveValidatorBase
        {
            ulong sourceSquares;
            if ((sourceSquares = pieceMoveMask & pieceOccupancy) == 0) return null;

            if (md.SourceFile != null)
                sourceSquares &= BoardHelpers.FileMasks[md.SourceFile.Value];
            if (md.SourceRank != null)
                sourceSquares &= BoardHelpers.RankMasks[md.SourceRank.Value];
            var sourceIndices = sourceSquares.GetSetBits();

            if (sourceIndices.Length == 0) return null;
            if (sourceIndices.Length > 1)
            {
                var possibleSources = new List<ushort>();

                foreach (var sourceIndex in sourceIndices)
                {
                    if (!md.DestinationIndex.HasValue) throw new MoveException("No destination value provided.");
                    var proposedMove = MoveHelpers.GenerateMove(sourceIndex, md.DestinationIndex.Value, md.MoveType, md.PromotionPiece ?? PromotionPiece.Knight);
                    var moveValidator = (T)Activator.CreateInstance(typeof(T), board, proposedMove);
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

        public static string MoveToSAN(this MoveExt move, IBoard boardInfo, bool recordResult = true)
        {
            var sideMoving = boardInfo.ActivePlayer;
            var preMoveBoard = boardInfo.GetPiecePlacement();
            var postMoveBoard = boardInfo.GetPiecePlacement().GetBoardPostMove(sideMoving, move);
            var srcPiece = boardInfo.GetPiecePlacement().GetPieceOfColorAtIndex(move.SourceIndex)?.Piece;
            if (srcPiece == null) throw new MoveException("No piece at source index.", MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare, move, sideMoving);
            var strSrcPiece = GetSANSourceString(boardInfo, move, srcPiece.Value);
            var strDstSquare = move.DestinationIndex.IndexToSquareDisplay();
            string checkInfo = "", result = "", promotionInfo = "", capture = "";

            var opponentOcc = preMoveBoard.Occupancy(sideMoving.Toggle());
            if ((opponentOcc & move.DestinationValue) != 0 || move.MoveType == MoveType.EnPassant)
            {
                capture = "x";
            }

            if (move.MoveType == MoveType.Promotion)
            {
                promotionInfo = $"={PieceHelpers.GetCharFromPromotionPiece(move.PromotionPiece)}";
            }

            var board = boardInfo.ApplyMoveToBoard(move);
            if (board.IsActivePlayerInCheck())
            {
                checkInfo = "+";
                if (board.IsCheckmate())
                {
                    checkInfo = $"#";
                    if (recordResult)
                    {
                        result = (sideMoving == Color.White ? "1-0" : "0-1");
                    }
                }
            }
            else if (recordResult)
            {

                result = board.IsStalemate() ? "1/2-1/2" : "";
            }

            //Get piece representation
            return $"{strSrcPiece}{capture}{move.DestinationIndex.IndexToSquareDisplay()}{promotionInfo}{checkInfo} {result}".Trim();
        }

        public static string GetSANSourceString(IBoard board, MoveExt move, Piece src)
        {
            if (src == Piece.King)
            {
                return "K";
            }
            if (src == Piece.Pawn)
            {
                //if the move was an En Passant or a capture, return the file letter
                return move.MoveType == MoveType.EnPassant || (move.SourceIndex.GetFile() != move.DestinationIndex.GetFile())
                    ? move.SourceIndex.IndexToFileDisplay().ToString() : "";
            }

            var strSrcPiece = src.GetCharRepresentation().ToString().ToUpper();
            var otherLikePieces = board.GetPiecePlacement().Occupancy(board.ActivePlayer, src);
            var duplicateAttackerIndexes = new List<ushort>();

            foreach (var attackerIndex in otherLikePieces.GetSetBits())
            {
                if (board.CanPieceMoveToDestination(attackerIndex, move.DestinationIndex))
                {
                    duplicateAttackerIndexes.Add(attackerIndex);
                }
            }

            if (duplicateAttackerIndexes.Count() == 1) return strSrcPiece;
            var duplicateFiles = duplicateAttackerIndexes.Select(x => x.GetFile()).GroupBy(x => x)
                .Any(x => x.Count() > 1);
            var duplicateRanks = duplicateAttackerIndexes.Select(x => x.GetRank()).GroupBy(x => x)
                .Any(x => x.Count() > 1);

            if (!duplicateFiles)
            {
                return strSrcPiece += move.SourceIndex.IndexToFileDisplay();
            }
            else if (!duplicateRanks)
            {
                return strSrcPiece += move.SourceIndex.IndexToRankDisplay();
            }
            else
            {
                return strSrcPiece += move.SourceIndex.IndexToSquareDisplay();
            }
        }
    }

}
