using System.Collections.Generic;
using System.Data;
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
            var fen = FENReader.FENInitial;
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

      



   

        private IMove DetermineWhichPieceMovesToSquare(in string move, IEnumerable<ushort> possibleAttackersOfType,
            ulong applicableBb, ushort destinationSquare)
        {
            var mv = (string) move.Clone();
            mv = mv.Substring(1);
            mv = mv.Substring(0, mv.Length - 2);
            mv = mv.Replace("x", "");
            ushort source;
            if (mv.Length == 2)
            {
                var sourceIdx = mv.SquareTextToIndex();

                source = sourceIdx;
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

    }
}