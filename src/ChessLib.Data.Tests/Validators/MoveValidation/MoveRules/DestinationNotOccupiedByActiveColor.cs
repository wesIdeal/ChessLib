using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.Validators.Tests.MoveValidation.MoveRules
{
    [TestFixture()]
    class DestinationNotOccupiedByActiveColor : ChessLib.Validators.MoveValidation.MoveRules.DestinationNotOccupiedByActiveColor
    {
        [Test]
        public void ShouldReturnNullIfTargetUnoccupied()
        {
            var board = new BoardInfo("4k3/8/8/8/8/5N2/8/4K3 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board.GetPiecePlacement(), Color.White, move);
            Assert.IsNull(Validate(board,postMove,move));
        }
        [Test]
        public void ShouldReturnNullIfTargetOccupiedByOpponent()
        {
            var board = new BoardInfo("4k3/8/8/8/8/5N2/8/4K1b1 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board.GetPiecePlacement(), Color.White, move);
            Assert.IsNull(Validate(board, postMove, move));
        }
        [Test]
        public void ShouldReturnErrorIfTargetOccupiedByActivePlayer()
        {
            var board = new BoardInfo("4k3/8/8/8/8/5N2/8/4K1B1 w - - 0 1");
            var move = MoveHelpers.GenerateMove(21, 6);
            var postMove = BoardHelpers.GetBoardPostMove(board.GetPiecePlacement(), Color.White, move);
            Assert.AreEqual(MoveError.ActiveColorPieceAtDestination, Validate(board, postMove, move));
        }
    }
}
