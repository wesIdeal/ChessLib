using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.CastlingRules.TestData
{
    internal class NotInCheckBeforeMoveValidatorTestData
    {
        public static IEnumerable<TestCaseData> GetNotInCheckBeforeMoveTestCases()
        {
            var e2 = "e2".ToBoardIndex();
            var e4 = "e4".ToBoardIndex();
            var board = new Board();
            var move = MoveHelpers.GenerateMove(e2, e4);
            yield return new TestCaseData(board, move, true)
                .SetName("NotInCheckBeforeMoveValidator - Castling side is in check")
                .Returns(MoveError.CastleKingInCheck);
            yield return new TestCaseData(board, move, false)
                .SetName("NotInCheckBeforeMoveValidator - Castling side is not in check")
                .Returns(MoveError.NoneSet);
        }
    }
}