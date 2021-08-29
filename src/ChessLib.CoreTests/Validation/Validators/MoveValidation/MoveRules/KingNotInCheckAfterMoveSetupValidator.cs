using System;
using System.Linq;
using System.Linq.Expressions;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using Moq;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules
{
    internal class KingNotInCheckAfterMoveSetup
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

        internal static Expression<Func<IBitboard, bool>> SetupKingInCheckMock(Board board, Board postMoveBoard, Move move, bool moveLeavesKingInCheck, Mock<IBitboard> bitboardMock)
        {
            var setupExpression = SetupKingInCheckExpression(board, postMoveBoard);
            bitboardMock.Setup(setupExpression)
                .Returns(moveLeavesKingInCheck)
                .Verifiable();
            return setupExpression;
        }
    }
}