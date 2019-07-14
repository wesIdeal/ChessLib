using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Validators.MoveValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ChessLib.Data
{
    /// <summary>
    /// Used to translate moves from text format (SAN | LAN) into a ushort-based move object <see cref="MoveExt"/>
    /// </summary>
    public class MoveTranslatorService : MoveDisplayService
    {
        /// <summary>
        /// Constructs the service based on the normal starting position
        /// </summary>
        public MoveTranslatorService()
        {
            InitializeBoard();
        }

        /// <summary>
        /// Constructs the service based on an existing board configuration
        /// </summary>
        /// <param name="board">The configuration/state of the current board.</param>
        public MoveTranslatorService(in IBoard board)
        {
            InitializeBoard(board);
        }

        /// <summary>
        /// Constructs the service based on an existing board configuration supplied via a PremoveFEN string
        /// </summary>
        /// <param name="fen">A PremoveFEN string detailing the board config</param>
        public MoveTranslatorService(string fen)
        {
            InitializeBoard(fen);
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
        /// Used to initialize the underlying board object based on the initial starting position
        /// </summary>
        public void InitializeBoard()
        {
            var fen = FENHelpers.FENInitial;
            InitializeBoard(fen);
        }

        /// <summary>
        /// Used to initialize the underlying board object based on PremoveFEN
        /// </summary>
        /// <param name="fen">PremoveFEN string detailing the board configuration</param>
        public void InitializeBoard(string fen)
        {
            Initialize(fen);
        }
        /// <summary>
        /// Used to initialize the underlying board object based on an existing board.
        /// </summary>
        /// <remarks>Is non-destructive to the board passed to the method by using the board's Clone() method.</remarks>
        /// <param name="board">An existing board to base the service's board from.</param>
        public void InitializeBoard(in IBoard board)
        {
            Initialize(board);
        }
        /// <summary>
        /// Given a board and a simple algebraic notation (SAN), generate a move object
        /// </summary>
        /// <param name="moveText">The SAN text of a move</param>
        /// <returns>A move object based on the board state and information from text.</returns>
        /// <exception cref="MoveException">If no destination index is present in the SAN text or if no piece was found at the source index.</exception>
        public MoveExt GenerateMoveFromText(string moveText)
        {

            var md = GetAvailableMoveDetails(moveText, Board.ActivePlayer);
            if (!md.DestinationIndex.HasValue)
            {
                throw new MoveException("Move detail error: no destination index was supplied.");
            }
            if (md.SourceFile == null || md.SourceRank == null)
            {
                md.SourceIndex = FindPieceSourceIndex(md);
            }
            if (md.SourceIndex == null)
            {
                throw new MoveException("Move detail error: no piece found at source.");
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
        ///  </summary>
        /// <example>e2e4 from initial position is 1. e4. e7e8q would be e8=Q</example>
        /// <param name="lanMove"></param>
        /// <returns>A basic move object, applicable to the current board.</returns>
        ///<exception cref="MoveException">If <param name="lanMove">lanMove</param> is less than 4 characters or greater than 5, or the source and/or destination strings did not translate to a board index.</exception>
        public MoveExt FromLongAlgebraicNotation(string lanMove)
        {
            if (lanMove.Length < 4 || lanMove.Length > 5)
            {
                throw new MoveException($"Failed to parse LAN move {lanMove}. LAN must be 4-5 characters. Ex: e2e4 (e4) or e7e8q (e8=Q)");
            }
            var sourceString = lanMove.Substring(0, 2);
            var destString = lanMove.Substring(2, 2);
            var source = sourceString.SquareTextToIndex();
            var dest = destString.SquareTextToIndex();
            if (source == null || dest == null)
            {
                throw new MoveException($"Unexpected value when converting LAN move to source and destination index: {lanMove}");
            }

            var promotionChar = lanMove.Length == 5 ? lanMove[4] : (char?)null;
            var promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionChar);
            return GenerateMoveFromIndexes(source.Value, dest.Value, promotionPiece);
        }

        public MoveExt GenerateMoveFromIndexes(ushort sourceIndex, ushort destinationIndex, PromotionPiece? promotionPiece)
        {
            var rv = Bitboard.GetMove(Board, sourceIndex, destinationIndex, promotionPiece ?? PromotionPiece.Knight);
            rv.SAN = MoveToSAN(rv);
            return rv;
        }

        /// <summary>
        /// Creates moves from long algebraic notation (LAN) sequential move array
        ///  </summary>
        /// <remarks>Does not alter board state from the initialized state.</remarks>
        /// <param name="lanMoves">Sequential set of moves in string format.</param>
        /// <returns>A collection of moves based on the current board.</returns>
        ///<exception cref="MoveException">If an element of <param name="lanMoves">lanMoves</param> is less than 4 characters or greater than 5, or the source and/or destination strings did not translate to a board index.</exception>
        public IEnumerable<MoveExt> FromLongAlgebraicNotation(IEnumerable<string> lanMoves)
        {
            var savedBoard = (IBoard)Board.Clone();
            var moves = new List<MoveExt>();
            foreach (var lanMove in lanMoves)
            {
                var move = FromLongAlgebraicNotation(lanMove);
                Board = Board.ApplyMoveToBoard(move);
                moves.Add(move);
            }
            InitializeBoard(savedBoard);
            return moves;
        }

        /// <summary>
        /// Gets move details by using regex. Second step in translating SAN -> ushort move.
        /// </summary>
        /// <param name="move">SAN move</param>
        /// <param name="color">Active player</param>
        public static MoveDetail GetAvailableMoveDetails(string move, Color color)
        {
            MoveDetail md = new MoveDetail() { MoveText = move, Color = color };
            if (move.Length < 2) throw new Exception("Invalid move. Must have at least 2 characters.");
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
                    var moveValidator = new MoveValidator(Board, proposedMove);
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
            var activePlayer = Board.ActivePlayer;
            var pieceOccupancy = Board.GetPiecePlacement().Occupancy(activePlayer, moveDetail.Piece);
            var totalBoardOccupancy = Board.TotalOccupancy();
            return FindPieceMoveSourceIndex(moveDetail, pieceOccupancy, totalBoardOccupancy);
        }

    }
}
