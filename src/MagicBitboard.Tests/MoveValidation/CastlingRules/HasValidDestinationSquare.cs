using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types;
using MagicBitboard;
using NUnit.Framework;

namespace ChessLib.MagicBitboard.Tests.MoveValidation.CastlingRules
{
    [TestFixture]
    public class HasValidDestinationSquare : MagicBitboard.MoveValidation.CastlingRules.HasValidDestinationSquare
    {
        #region BadDestination

        [Test]
        public void Validate_ShouldReturnBadDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = new ushort[] { 57, 63, 1, 5, 13, 28 };
            var boardInfo = new BoardInfo("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.AreEqual(MoveExceptionType.Castle_BadDestinationSquare, Validate(boardInfo, postMoveBoard, move));
            }
        }
        #endregion
        #region GoodDestination

        [Test]
        public void Validate_ShouldReturnNoErrorForGoodDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = this.ValidDestinationSquares;
            var boardInfo = new BoardInfo("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.IsNull(Validate(boardInfo, postMoveBoard, move));
            }
        }
        #endregion
    }
}
