using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.MoveValidation.MoveRules;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules.TestData
{
    internal class KingNotInCheckAfterMoveValidatorTestData : MoveValidatorTestData
    {
        private static Expression<Func<IBitboard, bool>> SetupKingInCheckExpression(Board board, Board postMoveBoard)
        {
            var activeKingIndex =
                postMoveBoard.Occupancy.Occupancy(board.ActivePlayer, Piece.King).GetSetBits().First();
            var attackerColor = board.OpponentColor;
            var postMoveOccupancy = postMoveBoard.Occupancy.Occupancy();
            return mock =>
                mock.IsSquareAttackedByColor(
                    It.Is<ushort>(x => x == activeKingIndex),
                    It.Is<Color>(x => x == attackerColor),
                    It.Is<ulong[][]>(x => x.Occupancy(null, null) == postMoveOccupancy),
                    It.Is<ushort?>(x => x == board.EnPassantIndex));
        }

        internal static Expression<Func<IBitboard, bool>> SetupKingInCheckMock(Board board, Board postMoveBoard,
            Move move, bool moveLeavesKingInCheck, Mock<IBitboard> bitboardMock)
        {
            var setupExpression = SetupKingInCheckExpression(board, postMoveBoard);
            bitboardMock.Setup(setupExpression)
                .Returns(moveLeavesKingInCheck)
                .Verifiable();
            return setupExpression;
        }

        /// <summary>
        ///     Get the test cases for testing <see cref="KingNotInCheckAfterMoveValidator" />
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestCaseData> GetKingNotInCheckAfterMoveValidatorTests()
        {
            const string pseudoTestCaseName = "Validator.Move.KingNotInCheckAfterMoveValidator";
            var board = fenTextToBoard.Translate("4k3/8/8/8/8/8/8/4K3 w - - 0 1");
            var postMoveBoard = fenTextToBoard.Translate("3k4/8/8/8/8/8/8/3K4 w - - 0 1");
            var move = MoveHelpers.GenerateMove("e2".SquareTextToIndex(), "e4".SquareTextToIndex());

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