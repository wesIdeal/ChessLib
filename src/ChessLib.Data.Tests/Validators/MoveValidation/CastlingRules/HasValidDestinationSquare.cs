using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using NUnit.Framework;
using ChessLib.Data.Boards;

namespace ChessLib.Data.Validators.MoveValidation.CastlingRules.Tests
{
    [TestFixture]
    public class HasValidDestinationSquare : Data.Validators.MoveValidation.CastlingRules.HasValidDestinationSquare
    {
        #region BadDestination

        [Test]
        public void Validate_ShouldReturnBadDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = new ushort[] { 57, 63, 1, 5, 13, 28 };
            var board = new Board("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.AreEqual(MoveError.CastleBadDestinationSquare, Validate(board, postMoveBoard, move));
            }
        }
        #endregion
        #region GoodDestination

        [Test]
        public void Validate_ShouldReturnNoErrorForGoodDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = this.ValidDestinationSquares;
            var Board = new Board("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.AreEqual(MoveError.NoneSet, Validate(Board, postMoveBoard, move));
            }
        }
        #endregion
    }
}
