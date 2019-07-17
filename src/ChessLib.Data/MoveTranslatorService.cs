using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Validators.MoveValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChessLib.Data
{
    /// <summary>
    /// Used to translate moves from text format (SAN | LAN) into a ushort-based move object <see cref="MoveExt"/>
    /// </summary>
    public class MoveTranslatorService : MoveDisplayService
    {
        
        protected readonly string CastleKingSide = "O-O";
        protected readonly string CastleQueenSide = "O-O-O";
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
        public MoveTranslatorService(in IBoard board) : this()
        {
            InitializeBoard(board);
        }

        /// <summary>
        /// Constructs the service based on an existing board configuration supplied via a PremoveFEN string
        /// </summary>
        /// <param name="fen">A PremoveFEN string detailing the board config</param>
        public MoveTranslatorService(string fen) : this()
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

        private string StripNonMoveInfoFromMove(in string move)
        {
            var nmi = new[] { "#", "+", "1/2-1/2", "1-0", "0-1" };
            var mv = (string)move.Clone();
            foreach (var s in nmi)
            {
                mv = mv.Replace(s, "");
            }

            return mv;
        }

        public MoveExt GetMoveFromSAN(string sanMove)
        {
            
            MoveExt moveExt = null;
            var move = StripNonMoveInfoFromMove(sanMove);
            if (move.Length < 2)
            {
                throw new MoveException("Invalid move. Must have at least 2 characters.");
            }
            var colorMoving = Board.ActivePlayer;
            var moveType = MoveType.Normal;
            if (char.IsLower(move[0]))
            {
                moveExt = GetPawnMoveDetails(move);
            }
            else if (move == CastleKingSide || move == CastleQueenSide)
            {
                if (colorMoving == Color.White)
                {
                    return move == CastleKingSide ? MoveHelpers.WhiteCastleKingSide : MoveHelpers.WhiteCastleQueenSide;
                }
                return move == CastleKingSide ? MoveHelpers.BlackCastleKingSide : MoveHelpers.BlackCastleQueenSide;
            }
            else
            {
                var pieceMoving = PieceHelpers.GetPiece(move[0]);
                var destinationSquare = move.Substring(move.Length - 2, 2).SquareTextToIndex();
                Debug.Assert(destinationSquare.HasValue && destinationSquare >= 0 && destinationSquare < 64);
                var squaresAttackingTarget = Board.PiecesAttackingSquare(destinationSquare.Value);
                if (squaresAttackingTarget == 0)
                {
                    throw new MoveException($"No pieces on any squares are attacking the square {destinationSquare.Value.IndexToSquareDisplay()}");
                }

                var possibleAttackersOfType = new List<ushort>();
                var applicableBB = Board.GetPiecePlacement()[(int)colorMoving][(int)pieceMoving];
                foreach (var possAttacker in squaresAttackingTarget.GetSetBits())
                {
                    if ((possAttacker.GetBoardValueOfIndex() & applicableBB) != 0)
                    {
                        possibleAttackersOfType.Add(possAttacker);
                    }
                }

                if (possibleAttackersOfType.Count == 0)
                {
                    throw new MoveException($"No pieces of type {pieceMoving.ToString()} are attacking the square {destinationSquare.Value.IndexToSquareDisplay()}");
                }
                else if (possibleAttackersOfType.Count == 1)
                {
                    moveExt = MoveHelpers.GenerateMove(possibleAttackersOfType[0], destinationSquare.Value);
                }
                else
                {
                    moveExt = DetermineWhichPieceMovesToSquare(move, possibleAttackersOfType, applicableBB,
                        destinationSquare.Value);
                }
            }
            if (moveExt == null)
            {
                throw new NotImplementedException("Move from san not implemented for this piece.");
            }

            return moveExt;
        }

        private MoveExt DetermineWhichPieceMovesToSquare(in string move, IEnumerable<ushort> possibleAttackersOfType, ulong applicableBb, ushort destinationSquare)
        {
            var mv = (string)move.Clone();
            mv = mv.Substring(1);
            mv = mv.Substring(0, mv.Length - 2);
            mv = mv.Replace("x", "");
            ushort source = 0;
            ushort? sourceIdx = null;
            if (mv.Length == 2)
            {
                sourceIdx = mv.SquareTextToIndex();
                if (sourceIdx == null)
                {
                    throw new MoveException($"Error parsing source disambiguating square from {move}");
                }

                source = sourceIdx.Value;
            }
            else if (mv.Length == 1)
            {
                ushort[] sourceSquares = null;
                if (char.IsDigit(mv[0]))
                {
                    var sourceRank = ushort.Parse(mv).GetRank();
                    sourceSquares = possibleAttackersOfType.Where(s => s.GetRank() == sourceRank).ToArray();

                }
                else
                {
                    var sourceFile = mv[0] - 'a';
                    sourceSquares = possibleAttackersOfType.Where(s => s.GetFile() == sourceFile).ToArray();
                }

                if (!sourceSquares.Any())
                {
                    throw new MoveException($"Problem finding attacking piece from move {move}.");
                }

                if (sourceSquares.Count() > 1)
                {
                    throw new MoveException($"Problem finding only one attacking piece from move {move}.");
                }

                source = sourceSquares[0];
            }

            else
            {
                var narrowedSquares = new List<ushort>();
                foreach (var square in possibleAttackersOfType)
                {
                    var testMove = MoveHelpers.GenerateMove(square, destinationSquare);
                    MoveValidator moveValidator = new MoveValidator(Board, testMove);
                    if (moveValidator.Validate() == MoveError.NoneSet)
                    {
                        narrowedSquares.Add(square);
                    }
                }
                if (narrowedSquares.Count!= 1)
                { throw new MoveException($"Problem finding only one attacking piece from move {move}."); }

                source = narrowedSquares[0];
            }
            var sourceVal = source.ToBoardValue();
            if ((sourceVal & applicableBb) == 0)
            {
                throw new MoveException($"No pieces attack {destinationSquare.IndexToSquareDisplay()} from move {move}.");
            }
            return MoveHelpers.GenerateMove(source, destinationSquare);
        }

        private MoveExt GetPawnMoveDetails(string move)
        {
            var colorMoving = Board.ActivePlayer;
            var promotionPiece = PromotionPiece.Knight;
            var pawnBB = Board.GetPiecePlacement()[(int)colorMoving][(int)Piece.Pawn];

            var moveLength = move.Length;
            bool isCapture = move.Contains("x");

            bool isPromotion = move.Contains("=");
            ushort sourceIndex = 0;
            ushort? destIndex = 0;
            ushort startingFile = 0;
            MoveType moveType = MoveType.Normal;
            if (isCapture)
            {
                startingFile = (ushort)(move[0] - 'a');
                move = move.Substring(2, moveLength - 2);
            }
            if (isPromotion)
            {
                moveType = MoveType.Promotion;
                var equalIndex = move.IndexOf('=');
                var promotionPieceStr = move.Substring(equalIndex + 1, 1);
                promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionPieceStr[0]);
                move = move.Substring(0, equalIndex);
            }

            destIndex = move.SquareTextToIndex();
            Debug.Assert(destIndex.HasValue);

            bool isPossibleInitialMove =
                (destIndex.Value.IsIndexOnRank(3) && colorMoving == Color.White) ||
                (destIndex.Value.IsIndexOnRank(4) && colorMoving == Color.Black);
            var destinationFile = destIndex.Value.FileFromIdx();
            var countBack = isCapture ?
                colorMoving.Equals(Color.White) ? startingFile - destinationFile - 8
                    : destinationFile - startingFile - 8 : -8;
            if (colorMoving == Color.Black)
            {
                countBack = Math.Abs(countBack);
            }
            sourceIndex = (ushort)(destIndex.Value + countBack);
            if (isPossibleInitialMove && !isCapture)
            {

                var srcValue = (sourceIndex).ToBoardValue();
                //first check rank 2
                if ((pawnBB & srcValue) == 0)
                {
                    //no, it was from the starting position
                    sourceIndex = colorMoving == Color.White ? (ushort)(sourceIndex - 8) : (ushort)(sourceIndex + 8);
                }
            }
            return MoveHelpers.GenerateMove(sourceIndex, destIndex.Value, moveType, promotionPiece);
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
