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
        private readonly MoveExt _goodMove = MoveHelpers.GenerateMove(4, 5);
        private readonly MoveExt _badColor = MoveHelpers.GenerateMove(60, 61);
        private readonly BoardInfo _boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/8/8/8/4K3 w - - 0 1");
        private readonly ulong[][] _postMoveBoard = new ulong[2][];
        [Test]
        public void Validate_ShouldReturnNullForGoodMove()
        {
            Assert.IsNull(Validate(_boardInfo, _postMoveBoard, _goodMove));
        }

        [Test]
        public void Validate_ShouldReturnCorrectErrorForBadMove()
        {
            Assert.AreEqual(MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare, Validate(_boardInfo, _postMoveBoard, _badColor));
        }
    }
}