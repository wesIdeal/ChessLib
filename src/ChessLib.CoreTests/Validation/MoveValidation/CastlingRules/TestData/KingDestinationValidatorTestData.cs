using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.CastlingRules.TestData
{
    internal class KingDestinationValidatorTestData
    {
        private const string name = "Valid King Destination";
        private static readonly FenTextToBoard fenTranslator = new FenTextToBoard();
        private static readonly Board WhiteCastlingBoard = new Board();

        private static readonly Board BlackCastlingBoard =
            fenTranslator.Translate("rnbqkbnr/pppppppp/8/8/8/5N2/PPPPPPPP/RNBQKB1R b KQkq - 1 1");

        public static IEnumerable<TestCaseData> GetValidDestinationTestCases(Color activeColor)
        {
            var board = (Board)(activeColor == Color.Black ? BlackCastlingBoard.Clone() : WhiteCastlingBoard.Clone());
            var castlingMoves = activeColor == Color.Black
                ? MoveHelpers.BlackCastlingMoves
                : MoveHelpers.WhiteCastlingMoves;
            var opponentCastlingMoves = activeColor == Color.Black
                ? MoveHelpers.WhiteCastlingMoves
                : MoveHelpers.BlackCastlingMoves;
            foreach (var castlingMove in castlingMoves)
            {
                var castlingSan = castlingMove.GetCastlingSan();
                var testName =
                    $"{name} - {activeColor} {castlingSan} destination {castlingMove.DestinationIndex.ToSquareString()} (Valid)";
                yield return new TestCaseData(board, castlingMove)
                    .SetName(testName)
                    .Returns(MoveError.NoneSet);
            }

            foreach (var castlingMove in opponentCastlingMoves)
            {
                var castlingSan = castlingMove.GetCastlingSan();
                var testName =
                    $"{name} - {activeColor} {castlingSan} destination {castlingMove.DestinationIndex.ToSquareString()} (Invalid)";
                yield return new TestCaseData(board, castlingMove)
                    .SetName(testName)
                    .Returns(MoveError.CastleBadDestinationSquare);
            }

            var normalMoveWhite= MoveHelpers.GenerateMove("e2".ToBoardIndex(), "e4".ToBoardIndex(), MoveType.Normal);
            var normalMoveBlack = MoveHelpers.GenerateMove("e7".ToBoardIndex(), "e5".ToBoardIndex(), MoveType.Normal);
            yield return new TestCaseData(board, normalMoveWhite).SetName(
                    $"{name} - White [normal move]")
                .Returns(MoveError.NoneSet);
            yield return new TestCaseData(board, normalMoveBlack).SetName(
                    $"{name} - Black [normal move]")
                .Returns(MoveError.NoneSet);
            var nonCastlingMoveWhiteSide = MoveHelpers.GenerateMove("e2".ToBoardIndex(), "e4".ToBoardIndex(), MoveType.Castle);
            var nonCastlingMoveBlackSide = MoveHelpers.GenerateMove("e7".ToBoardIndex(), "e5".ToBoardIndex(), MoveType.Castle);
            var nonCastlingMoves = new[] { nonCastlingMoveBlackSide, nonCastlingMoveWhiteSide };
            foreach (var nonCastlingMove in nonCastlingMoves)
            {
                yield return new TestCaseData(board, nonCastlingMove)
                    .SetName(
                        $"{name} - {activeColor} [non-castling move] destination {nonCastlingMove.DestinationIndex.ToSquareString()} (Invalid)")
                    .Returns(MoveError.CastleBadDestinationSquare); 
            }
        }
    }
}