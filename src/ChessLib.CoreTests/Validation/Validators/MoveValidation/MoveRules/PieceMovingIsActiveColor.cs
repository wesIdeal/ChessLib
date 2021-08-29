//using ChessLib.Core;
//using ChessLib.Core.Types;
//using ChessLib.Core.Types.Exceptions;
//using ChessLib.Core.Types.Helpers;
//using ChessLib.Data.Helpers;
//using NUnit.Framework;
//using NUnit.Framework.Internal;

//namespace ChessLib.Data.Validators.MoveValidation.MoveRules.Tests
//{
//    [TestFixture]
//    class PieceMovingIsActiveColor : Core.Validation.Validators.MoveValidation.MoveRules.PieceMovingIsActiveColor
//    {
//        private readonly Move _goodMove = MoveHelpers.GenerateMove(4, 5);
//        private readonly Move _badColor = MoveHelpers.GenerateMove(60, 61);
//        private readonly Board _boardInfo = new Board("4k3/8/8/8/8/8/8/4K3 w - - 0 1");
//        private readonly ulong[][] _postMoveBoard = new ulong[2][];
//        [Test]
//        public void Validate_ShouldReturnNullForGoodMove()
//        {
//            Assert.AreEqual(MoveError.NoneSet, Validate(_boardInfo, _postMoveBoard, _goodMove));
//        }

//        [Test]
//        public void Validate_ShouldReturnCorrectErrorForBadMove()
//        {
//            Assert.AreEqual(MoveError.ActivePlayerHasNoPieceOnSourceSquare, Validate(_boardInfo, _postMoveBoard, _badColor));
//        }
//    }
//}

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules
{
}