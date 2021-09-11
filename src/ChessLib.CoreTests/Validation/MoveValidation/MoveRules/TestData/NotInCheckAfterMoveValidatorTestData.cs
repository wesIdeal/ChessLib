using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.MoveValidation.MoveRules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.MoveRules.TestData
{
    internal class NotInCheckAfterMoveValidatorTestData
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();

        /// <summary>
        ///     Get the test cases for testing <see cref="NotInCheckAfterMoveValidator" />
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestCaseData> GetKingNotInCheckAfterMoveValidatorTests()
        {
            const string pseudoTestCaseName = "Validator.Move.KingNotInCheckAfterMoveValidator";
            var board = fenTextToBoard.Translate("4k3/8/8/8/8/8/8/4K3 w - - 0 1");
            var postMoveBoard = fenTextToBoard.Translate("3k4/8/8/8/8/8/8/3K4 w - - 0 1");
            var move = MoveHelpers.GenerateMove("e2".ToBoardIndex(), "e4".ToBoardIndex());

            yield return
                new TestCaseData(board, postMoveBoard, move, true)
                    .SetName(pseudoTestCaseName + " - In Check")
                    .SetDescription("Proposed move leaves King in check.")
                    .Returns(MoveError.MoveLeavesKingInCheck);

            yield return
                new TestCaseData(board, postMoveBoard, move, false)
                    .SetName(pseudoTestCaseName + " - Not In Check")
                    .SetDescription("Proposed move does not leave King in check.")
                    .Returns(MoveError.NoneSet);
        }
    }
}