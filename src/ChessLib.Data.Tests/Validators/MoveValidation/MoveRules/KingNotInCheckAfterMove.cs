using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules.Tests
{
    [TestFixture()]
    class KingNotInCheckAfterMove : Core.Validation.Validators.MoveValidation.MoveRules.KingNotInCheckAfterMove
    {
        private static readonly FenReader FenReader = new FenReader();

        [Test]
        public void ShouldReturnErrorIfMoveLeavesKingInCheck()
        {
            var biDiscovered = FenReader.GetBoard("4k3/4r3/8/8/8/8/4B3/4K3 w - - 0 1");
            var biInCheck = FenReader.GetBoard("4k3/4r3/8/8/8/8/8/3BK3 w - - 0 1");

            var moveDiscovery = MoveHelpers.GenerateMove(12, 3);
            var moveInCheck = MoveHelpers.GenerateMove(3, 10);

            var discoveredPostMove = BoardHelpers.GetBoardPostMove(biDiscovered, moveDiscovery);
            var inCheckPostMove = BoardHelpers.GetBoardPostMove(biInCheck, moveInCheck);

            Assert.AreEqual(MoveError.MoveLeavesKingInCheck, Validate(biDiscovered, discoveredPostMove, moveDiscovery));
            Assert.AreEqual(MoveError.MoveLeavesKingInCheck, Validate(biInCheck, inCheckPostMove, moveInCheck));

        }

        [Test]
        public void ShouldReturnNullWhenMoveBlocksCheck()
        {
            var bi = FenReader.GetBoard("4k3/4r3/8/8/8/8/8/3BK3 w - - 0 1");
            var move = MoveHelpers.GenerateMove(3, 12);
            var postMoveBoard = BoardHelpers.GetBoardPostMove(bi, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(bi, postMoveBoard, move));
        }

        [Test]
        public void ShouldReturnNullWhenCheckIsNotPresent()
        {
            var bi = FenReader.GetBoard("3k4/4b3/8/8/8/8/4B3/5K2 w - - 0 1");
            var move = MoveHelpers.GenerateMove(12, 3);
            var postMoveBoard = BoardHelpers.GetBoardPostMove(bi, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(bi, postMoveBoard, move));
        }

        [Test]
        public void ValidateMove_ShouldThrowException_IfMoveLeavesKingInCheck()
        {
            var move = MoveHelpers.GenerateMove(62, 44);
            var kingInCheck = FenReader.GetBoard("5kb1/8/8/8/8/8/6K1/5R2 b - - 1 2");
            var postMoveBoard = BoardHelpers.GetBoardPostMove(kingInCheck, move);
            Assert.AreEqual(MoveError.MoveLeavesKingInCheck, Validate(kingInCheck, postMoveBoard, move));
        }

        [Test]
        public void ValidateMove_ShouldThrowException_IfMoveLeavesKingInCheck2()
        {
            var move = MoveHelpers.GenerateMove(53, 62);
            var kingInCheck = FenReader.GetBoard("5k2/5b2/8/8/8/8/6K1/5R2 b - - 1 2");
            var postMoveBoard = BoardHelpers.GetBoardPostMove(kingInCheck, move);
            Assert.AreEqual(MoveError.MoveLeavesKingInCheck, Validate(kingInCheck, postMoveBoard, move), "ValidateMove should throw and exception if the move leaves the King in check. MoveValue is Bg7.");
        }

        [Test]
        public void ValidateMove_ShouldThrowException_IfMoveLeavesKingInCheck3()
        {
            var move = MoveHelpers.GenerateMove(61, 60);
            var kingInCheck = FenReader.GetBoard("5k2/3B4/8/8/8/1b6/6K1/5R2 b - - 1 2");
            var postMoveBoard = BoardHelpers.GetBoardPostMove(kingInCheck, move);
            Assert.AreEqual(MoveError.MoveLeavesKingInCheck, Validate(kingInCheck, postMoveBoard, move), "ValidateMove should throw and exception if the move leaves the King in check.");
        }
    }
}
