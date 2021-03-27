using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Services;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.MoveValidation;

namespace ChessLib.Data
{
    /// <summary>
    ///     Used to translate moves from text format (SAN | LAN) into a ushort-based move object <see cref="Move" />
    /// </summary>
    public class MoveTranslatorService : MoveDisplayService
    {
        protected readonly string CastleKingSide = "O-O";
        protected readonly string CastleQueenSide = "O-O-O";

        /// <summary>
        ///     Constructs the service based on the normal starting position
        /// </summary>
        public MoveTranslatorService()
        {
            InitializeBoard();
        }

        /// <summary>
        ///     Constructs the service based on an existing board configuration
        /// </summary>
        /// <param name="board">The configuration/state of the current board.</param>
        public MoveTranslatorService(in IBoard board) : this()
        {
            InitializeBoard(board);
        }

        /// <summary>
        ///     Constructs the service based on an existing board configuration supplied via a PremoveFEN string
        /// </summary>
        /// <param name="fen">A PremoveFEN string detailing the board config</param>
        public MoveTranslatorService(string fen) : this()
        {
            InitializeBoard(fen);
        }

        /// <summary>
        ///     Used to initialize the underlying board object based on the initial starting position
        /// </summary>
        public void InitializeBoard()
        {
            var fen = FENHelpers.FENInitial;
            InitializeBoard(fen);
        }

        /// <summary>
        ///     Used to initialize the underlying board object based on PremoveFEN
        /// </summary>
        /// <param name="fen">PremoveFEN string detailing the board configuration</param>
        public void InitializeBoard(string fen)
        {
            Initialize(fen);
        }

        /// <summary>
        ///     Used to initialize the underlying board object based on an existing board.
        /// </summary>
        /// <remarks>Is non-destructive to the board passed to the method by using the board's Clone() method.</remarks>
        /// <param name="board">An existing board to base the service's board from.</param>
        public void InitializeBoard(in IBoard board)
        {
            Initialize(board);
        }


        /// <summary>
        ///     Create move from long alg. notation
        /// </summary>
        /// <example>e2e4 from initial position is 1. e4. e7e8q would be e8=Q</example>
        /// <param name="lanMove"></param>
        /// <returns>A basic move object, applicable to the current board.</returns>
        /// <exception cref="MoveException">
        ///     If
        ///     <param name="lanMove">lanMove</param>
        ///     is less than 4 characters or greater than 5, or the source and/or destination strings did not translate to a board
        ///     index.
        /// </exception>
        public Move FromLongAlgebraicNotation(string lanMove)
        {
            var basicMove = BasicMoveFromLAN(lanMove);
            return GenerateMoveFromIndexes(basicMove.SourceIndex, basicMove.DestinationIndex, basicMove.PromotionPiece);
        }

        public Move GenerateMoveFromIndexes(ushort sourceIndex, ushort destinationIndex,
            PromotionPiece? promotionPiece)
        {
            var moveType = BoardHelpers.GetMoveType(Board, sourceIndex, destinationIndex);
            var move = (Move)MoveHelpers.GenerateMove(sourceIndex, destinationIndex, moveType, promotionPiece ?? PromotionPiece.Knight);
            move.SAN = MoveToSAN(move);
            return move;
        }

        /// <summary>
        ///     Gets a basic move with no SAN and no en passant information
        /// </summary>
        /// <param name="lan"></param>
        /// <returns></returns>
        public static IMove BasicMoveFromLAN(string lan)
        {
            var length = lan.Length;
            if (length < 4 || length > 5)
            {
                throw new MoveException($"LAN move {lan} has invalid length.");
            }

            var sourceString = lan.Substring(0, 2);
            var destString = lan.Substring(2, 2);
            var source = sourceString.SquareTextToIndex();
            var dest = destString.SquareTextToIndex();
            if (source == null || dest == null)
            {
                throw new MoveException(
                    $"Unexpected value when converting LAN move to source and destination index: {lan}");
            }

            var promotionChar = lan.Length == 5 ? lan[4] : (char?)null;
            var promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionChar);
            var isPromotion = length == 5;
            return MoveHelpers.GenerateMove(source.Value, dest.Value,
                isPromotion ? MoveType.Promotion : MoveType.Normal, promotionPiece);
        }

        /// <summary>
        ///     Creates moves from long algebraic notation (LAN) sequential move array
        /// </summary>
        /// <remarks>Does not alter board state from the initialized state.</remarks>
        /// <param name="lanMoves">Sequential set of moves in string format.</param>
        /// <returns>A collection of moves based on the current board.</returns>
        /// <exception cref="MoveException">
        ///     If an element of
        ///     <param name="lanMoves">lanMoves</param>
        ///     is less than 4 characters or greater than 5, or the source and/or destination strings did not translate to a board
        ///     index.
        /// </exception>
        public IEnumerable<Move> FromLongAlgebraicNotation(IEnumerable<string> lanMoves)
        {
            var savedBoard = (IBoard)Board.Clone();
            var moves = new List<Move>();
            foreach (var lanMove in lanMoves)
            {
                var move = FromLongAlgebraicNotation(lanMove);
                Board = Board.ApplyMoveToBoard(move, true);
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

        public IMove GetMoveFromSAN(string sanMove)
        {
            IMove resultingMove;
            Debug.WriteLine(Board.ToFEN());
            var move = StripNonMoveInfoFromMove(sanMove);
            if (move.Length < 2)
            {
                throw new MoveException("Invalid move. Must have at least 2 characters.");
            }

            var colorMoving = Board.ActivePlayer;

            if (char.IsLower(move[0]))
            {
                resultingMove = GetPawnMoveDetails(move);
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
                var squaresAttackingTarget =
                    Bitboard.Instance.PiecesAttackingSquare(Board.Occupancy, destinationSquare.Value);
                if (squaresAttackingTarget == 0)
                {
                    throw new MoveException(
                        $"No pieces on any squares are attacking the square {destinationSquare.Value.IndexToSquareDisplay()}",
                        Board);
                }

                var possibleAttackersOfType = new List<ushort>();
                var applicableBlockerBoard = Board.Occupancy.Occupancy(Board.ActivePlayer, pieceMoving);
                foreach (var possAttacker in squaresAttackingTarget.GetSetBits())
                {
                    if ((possAttacker.GetBoardValueOfIndex() & applicableBlockerBoard) != 0)
                    {
                        possibleAttackersOfType.Add(possAttacker);
                    }
                }

                if (possibleAttackersOfType.Count == 0)
                {
                    throw new MoveException(
                        $"Error with move {sanMove}:No pieces of type {pieceMoving.ToString()} are attacking the square {destinationSquare.Value.IndexToSquareDisplay()}",
                        Board);
                }

                if (possibleAttackersOfType.Count == 1)
                {
                    resultingMove = MoveHelpers.GenerateMove(possibleAttackersOfType[0], destinationSquare.Value);
                }
                else
                {
                    resultingMove = DetermineWhichPieceMovesToSquare(move, possibleAttackersOfType, applicableBlockerBoard,
                        destinationSquare.Value);
                }
            }

            if (resultingMove == null)
            {
                throw new NoNullAllowedException($"MoveTranslatorService: MoveValue should not be null after translation. Error in PGN or application for move {sanMove}.");
            }

            return resultingMove;
        }

        private IMove DetermineWhichPieceMovesToSquare(in string move, IEnumerable<ushort> possibleAttackersOfType,
            ulong applicableBb, ushort destinationSquare)
        {
            var mv = (string)move.Clone();
            mv = mv.Substring(1);
            mv = mv.Substring(0, mv.Length - 2);
            mv = mv.Replace("x", "");
            ushort source;
            if (mv.Length == 2)
            {
                var sourceIdx = mv.SquareTextToIndex();
                if (sourceIdx == null)
                {
                    throw new MoveException($"Error parsing source disambiguating square from {move}");
                }

                source = sourceIdx.Value;
            }
            else if (mv.Length == 1)
            {
                ushort[] sourceSquares;
                if (char.IsDigit(mv[0]))
                {
                    var sourceRank = ushort.Parse(mv) - 1;
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

                if (sourceSquares.Length > 1)
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
                    var moveValidator = new MoveValidator(Board, testMove);
                    if (moveValidator.Validate() == MoveError.NoneSet)
                    {
                        narrowedSquares.Add(square);
                    }
                }

                if (narrowedSquares.Count != 1)
                {
                    throw new MoveException($"Problem finding only one attacking piece from move {move}.");
                }

                source = narrowedSquares[0];
            }

            var sourceVal = source.GetBoardValueOfIndex();
            if ((sourceVal & applicableBb) == 0)
            {
                throw new MoveException(
                    $"No pieces attack {destinationSquare.IndexToSquareDisplay()} from move {move}.");
            }

            return MoveHelpers.GenerateMove(source, destinationSquare);
        }

        private IMove GetPawnMoveDetails(string move)
        {
            var colorMoving = Board.ActivePlayer;
            var promotionPiece = PromotionPiece.Knight;
            var pawnBitBoard = Board.Occupancy[(int)colorMoving][(int)Piece.Pawn];

            var moveLength = move.Length;
            var isCapture = move.Contains("x");

            var isPromotion = move.Contains("=");
            ushort startingFile = 0;
            var moveType = MoveType.Normal;
            if (isCapture)
            {
                startingFile = (ushort)(move[0] - 'a');
                move = move.Substring(2, moveLength - 2);
                if (Board.EnPassantSquare.HasValue)
                {
                }
            }

            if (isPromotion)
            {
                moveType = MoveType.Promotion;
                var equalIndex = move.IndexOf('=');
                var promotionPieceStr = move.Substring(equalIndex + 1, 1);
                promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionPieceStr[0]);
                move = move.Substring(0, equalIndex);
            }

            var destIndex = move.SquareTextToIndex();
            if (!destIndex.HasValue)
            {
                throw new NoNullAllowedException("MoveTranslatorService: destIndex should not be null.");
            }
            var likelyStartingSq = (ushort)(colorMoving == Color.White ? destIndex.Value - 8 : destIndex.Value + 8);
            var sourceIndex = likelyStartingSq;
            Debug.Assert(destIndex.HasValue);

            var isPossibleInitialMove =
                destIndex.Value.GetRank() == 3 && colorMoving == Color.White ||
                destIndex.Value.GetRank() == 4 && colorMoving == Color.Black;
            if (isCapture)
            {
                var destinationFile = destIndex.Value.FileFromIdx();
                if (colorMoving == Color.White)
                {
                    var modifier = destinationFile - startingFile + 8;
                    sourceIndex = (ushort)(destIndex.Value - modifier);
                }
                else
                {
                    var modifier = startingFile - destinationFile + 8;
                    sourceIndex = (ushort)(destIndex.Value + modifier);
                }
            }
            else if (isPossibleInitialMove)
            {
                var possibleStartingIndex = colorMoving == Color.White ? destIndex.Value - 8 : destIndex + 8;
                var srcValue = ((ushort)possibleStartingIndex).GetBoardValueOfIndex();
                //first check rank 2
                if ((pawnBitBoard & srcValue) == 0)
                {
                    //no, it was from the starting position
                    sourceIndex = colorMoving == Color.White
                        ? (ushort)(possibleStartingIndex - 8)
                        : (ushort)(possibleStartingIndex + 8);
                }
                else
                {
                    sourceIndex = (ushort)possibleStartingIndex.Value;
                }
            }

            if (isCapture && Board.EnPassantSquare.HasValue && destIndex.Value == Board.EnPassantSquare.Value)
            {
                moveType = MoveType.EnPassant;
            }

            return MoveHelpers.GenerateMove(sourceIndex, destIndex.Value, moveType, promotionPiece);
        }
    }
}