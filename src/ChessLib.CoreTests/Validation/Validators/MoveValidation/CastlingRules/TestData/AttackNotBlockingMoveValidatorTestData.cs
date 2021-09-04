using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules.TestData
{
    internal class AttackNotBlockingMoveValidatorTestData
    {
        private const string name = "Castling Path Validation";
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();

        public static IEnumerable<TestCaseData> GetCastlingMovesForColor(Color color)
        {
            const string emptyFenWhiteToMove = "4k3/8/8/8/8/8/8/4K3 w - - 0 1";
            const string emptyFenBlackToMove = "4k3/8/8/8/8/8/8/4K3 b - - 0 1";
            var fen = color == Color.Black ? emptyFenBlackToMove : emptyFenWhiteToMove;
            var castlingMoves = color == Color.Black ? MoveHelpers.BlackCastlingMoves : MoveHelpers.WhiteCastlingMoves;

            foreach (var castlingMove in castlingMoves)
            {
                string san;
                if (MoveHelpers.KingSideCastlingMoves.Contains(castlingMove))
                {
                    san = "O-O";
                }
                else if (MoveHelpers.QueenSideCastlingMoves.Contains(castlingMove))
                {
                    san = "O-O-O";
                }
                else
                {
                    throw new ArgumentException("Move is not castling move.");
                }

                foreach (var attackTestCase in SetupPathSquareValidation(fenTextToBoard.Translate(fen),
                    castlingMove))
                {
                    yield return attackTestCase
                        .SetName($"{name} - {color} {san}: {attackTestCase.TestName}");
                }
            }
        }

        private static IEnumerable<TestCaseData> SetupPathSquareValidation(Board board, Move castlingMove)
        {
            var totalOccupancy = board.Occupancy.Occupancy();
            var enPassantSquare = board.EnPassantIndex;

            var inBetweenSquares = BoardHelpers.InBetween(castlingMove.SourceIndex, castlingMove.DestinationIndex) |
                                   castlingMove.DestinationValue;
            var inBetweenSquareArr = inBetweenSquares.GetSetBits().ToArray();
            var permutations =
                MovingPieceService.GetAllPermutationsOfSetBits(inBetweenSquareArr, 0, 0)
                    .Distinct()
                    .ToList();
            TestContext.WriteLine(permutations);
            TestContext.WriteLine("----------------------------------------");
            foreach (var attackedSquaresValue in permutations)
            {
                var bb = new Mock<IBitboard>();
                var attackedSquares = attackedSquaresValue.GetSetBits();

                foreach (var squareBetween in inBetweenSquareArr)
                {
                    var isAttacked = attackedSquares.Contains(squareBetween);
                    SetupMock(board, squareBetween, totalOccupancy, enPassantSquare, bb, isAttacked);
                }

                var strBlockers = attackedSquares.Any()? string.Join(", ", attackedSquares) : "[None]";
                TestContext.WriteLine($"Returning test case for {attackedSquaresValue}");
                var expectedReturnValue = attackedSquares.Any() ? MoveError.CastleThroughCheck : MoveError.NoneSet;
                yield return new TestCaseData(bb, board, castlingMove)
                    .SetName($"Indices Attacked: {strBlockers}")
                    .Returns(expectedReturnValue);
            }
        }


        private static void SetupMock(Board board, ushort square,
            ulong totalOccupancy,
            ushort? enPassantSquare, Mock<IBitboard> bb, bool returnValue)
        {
            Expression<Func<IBitboard, bool>> expression = x => x.IsSquareAttackedByColor(
                It.Is<ushort>(c => c == square),
                It.Is<Color>(c => c == board.OpponentColor),
                It.Is<ulong[][]>(c => c.Occupancy(null, null) == totalOccupancy),
                It.Is<ushort?>(c => c == enPassantSquare));

            bb.Setup(expression).Returns(returnValue).Verifiable();
        }
    }
}