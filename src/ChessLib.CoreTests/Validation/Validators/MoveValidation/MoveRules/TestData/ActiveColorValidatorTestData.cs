using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules.TestData
{
    internal class ActiveColorValidatorTestData
    {
        private const string name = "Active Color";

        internal static IEnumerable<TestCaseData> GetActiveColorValidatorTestCases()
        {
            var initialBoard = new Board();
            var e2 = "e2".ToBoardIndex();
            var e3 = "e3".ToBoardIndex();
            var e4 = "e4".ToBoardIndex();
            var e6 = "e6".ToBoardIndex();
            var e7 = "e7".ToBoardIndex();

            var sourceEmptyMove = MoveHelpers.GenerateMove(e3, e4);
            var sourceOccupiedWithActiveColorMove = MoveHelpers.GenerateMove(e2, e4);
            var sourceOccupiedWithOpponentColorMove = MoveHelpers.GenerateMove(e7, e6);
            yield return new TestCaseData(initialBoard, sourceEmptyMove)
                .SetName($"{name} - should return error condition if source square is empty")
                .SetDescription("Test validator -  if source square is occupied by Active Color")
                .Returns(MoveError.ActivePlayerHasNoPieceOnSourceSquare);

            yield return new TestCaseData(initialBoard, sourceOccupiedWithActiveColorMove)
                .SetName($"{name} - should return no error when source occupied by Active Color")
                .SetDescription("Test validator -  if source square is occupied by Active Color")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData(initialBoard, sourceOccupiedWithOpponentColorMove)
                .SetName($"{name} - should return error condition if source square is occupied by Opponent Color")
                .SetDescription("Test validator -  if source square is occupied by Active Color")
                .Returns(MoveError.ActivePlayerHasNoPieceOnSourceSquare);
        }
    }
}