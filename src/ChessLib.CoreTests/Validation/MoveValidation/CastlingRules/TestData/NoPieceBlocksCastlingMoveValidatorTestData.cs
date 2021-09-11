using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.CastlingRules.TestData
{
    internal class NoPieceBlocksCastlingMoveValidatorTestData
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();


        public static IEnumerable<TestCaseData> GetTestCases()
        {
            const string name = "Pieces blocking castling";
            const string emptyFenWhiteToMove = "4k3/8/8/8/8/8/8/4K3 w - - 0 1";
            foreach (var castlingMove in MoveHelpers.BlackCastlingMoves)
            {
                var san = castlingMove.GetCastlingSan();
                foreach (var testCaseData in GetCastlingMoves(Color.Black, castlingMove))
                {
                    yield return testCaseData.SetName($"{name} - Black {san}: {testCaseData.TestName}");
                }
            }

            foreach (var castlingMove in MoveHelpers.WhiteCastlingMoves)
            {
                var san = castlingMove.GetCastlingSan();
                foreach (var testCaseData in GetCastlingMoves(Color.White, castlingMove))
                {
                    yield return testCaseData.SetName($"{name} - White {san}: {testCaseData.TestName}");
                }
            }

            yield return new TestCaseData(fenTextToBoard.Translate(emptyFenWhiteToMove),
                    MoveHelpers.GenerateMove("e2".ToBoardIndex(), "e4".ToBoardIndex()))
                .SetName($"{name} - Non-castling move should return no error")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData(fenTextToBoard.Translate(emptyFenWhiteToMove),
                    MoveHelpers.GenerateMove("e2".ToBoardIndex(), "e4".ToBoardIndex(), MoveType.Castle))
                .SetName($"{name} - Bad castling move should return no error")
                .Returns(MoveError.BadDestination);
        }

        public static IEnumerable<TestCaseData> GetCastlingMoves(Color color, Move move)
        {
            const string emptyFenWhiteToMove = "4k3/8/8/8/8/8/8/4K3 w - - 0 1";
            const string emptyFenBlackToMove = "4k3/8/8/8/8/8/8/4K3 b - - 0 1";
            var rookInitialSquareIndex = MoveHelpers.GetRookMoveForCastleMove(move).SourceIndex;
            var between = BoardHelpers.InBetween(rookInitialSquareIndex, move.SourceIndex);
            var blockerPermutations = MovingPieceService.GetAllPermutationsOfSetBits(between.GetSetBits(), 0, 0)
                .Where(x => x != 0);
            var fen = color == Color.Black ? emptyFenBlackToMove : emptyFenWhiteToMove;
            var board = fenTextToBoard.Translate(fen);

            foreach (var permutation in blockerPermutations)
            {
                var editedBoard = (Board)board.Clone();
                editedBoard.Occupancy[(int)color][(int)Piece.King] = permutation;
                var strBlockers = string.Join(", ", permutation.GetSetBits());
                yield return new TestCaseData(editedBoard, move)
                    .SetName($"Blockers on indices {strBlockers}")
                    .Returns(MoveError.CastleOccupancyBetween);
            }

            yield return new TestCaseData(board, move)
                .SetName("No Blockers")
                .Returns(MoveError.NoneSet);
        }
    }
}