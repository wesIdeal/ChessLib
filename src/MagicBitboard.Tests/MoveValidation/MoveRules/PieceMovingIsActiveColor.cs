using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using MagicBitboard;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.MagicBitboard.Tests.MoveValidation.MoveRules
{
    [TestFixture]
    class PieceMovingIsActiveColor : MagicBitboard.MoveValidation.MoveRules.PieceMovingIsActiveColor
    {
        private MoveExt goodMove = MoveHelpers.GenerateMove(4, 5);
        private MoveExt badColor = MoveHelpers.GenerateMove(60, 61);
        private BoardInfo boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/8/8/8/4K3 w - - 0 1");
        private ulong[][] postMoveBoard = new ulong[2][];
        [Test]
        public void Validate_ShouldReturnNullForGoodMove()
        {
            Assert.IsNull(Validate(boardInfo, postMoveBoard, goodMove));
        }

        [Test]
        public void Validate_ShouldReturnCorrectErrorForBadMove()
        {
            Assert.AreEqual(MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare, Validate(boardInfo, postMoveBoard, badColor));
        }
    }
}