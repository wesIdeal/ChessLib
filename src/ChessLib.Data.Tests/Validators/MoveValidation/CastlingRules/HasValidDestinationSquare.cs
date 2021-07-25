using ChessLib.Core.Helpers;
using ChessLib.Core.Services;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Data.Validators.MoveValidation.CastlingRules.Tests
{
    [TestFixture]
    public class
        HasValidDestinationSquare : Core.Validation.Validators.MoveValidation.CastlingRules.HasValidDestinationSquare
    {
        private static readonly FenReader FenReader = new FenReader();

        [Test]
        public void Validate_ShouldReturnBadDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = new ushort[] { 57, 63, 1, 5, 13, 28 };
            var board = FenReader.GetBoard("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.AreEqual(MoveError.CastleBadDestinationSquare, Validate(board, postMoveBoard, move));
            }
        }

        [Test]
        public void Validate_ShouldReturnNoErrorForGoodDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = ValidDestinationSquares;
            var board = FenReader.GetBoard("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.AreEqual(MoveError.NoneSet, Validate(board, postMoveBoard, move));
            }
        }
    }
}