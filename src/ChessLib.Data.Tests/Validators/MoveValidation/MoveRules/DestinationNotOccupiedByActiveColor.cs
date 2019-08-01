using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Exceptions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules.Tests
{
    [TestFixture()]
    class DestinationNotOccupiedByActiveColor : Data.Validators.MoveValidation.MoveRules.DestinationNotOccupiedByActiveColor
    {
        [Test]
        public void ShouldReturnNullIfTargetUnoccupied()
        {
            var board = new BoardInfo("4k3/8/8/8/8/5N2/8/4K3 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(board,postMove,move));
        }
        [Test]
        public void ShouldReturnNullIfTargetOccupiedByOpponent()
        {
            var board = new BoardInfo("4k3/8/8/8/8/5N2/8/4K1b1 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.NoneSet, Validate(board, postMove, move));
        }
        [Test]
        public void ShouldReturnErrorIfTargetOccupiedByActivePlayer()
        {
            var board = new BoardInfo("4k3/8/8/8/8/5N2/8/4K1B1 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board, move);
            Assert.AreEqual(MoveError.ActiveColorPieceAtDestination, Validate(board, postMove, move));
        }
    }
}
