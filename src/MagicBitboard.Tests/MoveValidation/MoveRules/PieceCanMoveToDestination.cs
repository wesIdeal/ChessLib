using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using MagicBitboard;
using NUnit.Framework;

namespace ChessLib.MagicBitboard.Tests.MoveValidation.MoveRules
{
    [TestFixture]
    internal class PieceCanMoveToDestination : MagicBitboard.MoveValidation.MoveRules.PieceCanMoveToDestination
    {
        ulong[][] postMoveBoard = new ulong[2][];
        [Test]
        public void ShouldReturnNull_OnValidMove_EmptySquare_Edge()
        {
            var move = MoveHelpers.GenerateMove(0, 56);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/8/8/8/R3K3 w - - 0 1");
            Assert.IsNull(Validate(boardInfo, postMoveBoard, move), "Should return null when moving to empty square on a8.");
        }

        [Test]
        public void ShouldReturnNull_OnValidMove_EmptySquare_Middle()
        {
            var move = MoveHelpers.GenerateMove(2, 26);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/8/8/8/2R1K3 w - - 0 1");
            Assert.IsNull(Validate(boardInfo, postMoveBoard, move), "Should return null when moving to empty square on c4.");
        }

        [Test]
        public void ShouldReturnNull_OnValidMove_OpponentPieceAttacked_Edge()
        {
            var move = MoveHelpers.GenerateMove(2, 58);
            var boardInfo = BoardInfo.BoardInfoFromFen("2q1k3/8/8/8/8/8/8/2R1K3 w - - 0 1");
            Assert.IsNull(Validate(boardInfo, postMoveBoard, move), "Should return null when capturing on c8.");

        }

        [Test]
        public void ShouldReturnNull_OnValidMove_OpponentPieceAttacked_Middle()
        {
            var move = MoveHelpers.GenerateMove(2, 26);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/2q5/8/8/2R1K3 w - - 0 1");
            Assert.IsNull(Validate(boardInfo, postMoveBoard, move), "Should return null when capturing on c4.");
        }

        [Test]
        public void ShouldReturnProperError_WhenActivePlayerHasNoPieceAtSource()
        {
            var move = MoveHelpers.GenerateMove(0, 56);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/2q5/8/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare, Validate(boardInfo, postMoveBoard, move), "Should return error when active color has no piece at source.");
        }

        [Test]
        public void ShouldReturnProperError_WhenSlidingPieceBlocked()
        {
            var move = MoveHelpers.GenerateMove(2, 26);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/2q5/2p5/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveExceptionType.BadDestination, Validate(boardInfo, postMoveBoard, move), "Should return error when capturing on c4 but is blocked.");
        }

        [Test]
        public void ShouldReturnProperError_WhenDestinationIsInvalidForPiece()
        {
            var move = MoveHelpers.GenerateMove(2, 11);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/2q5/8/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveExceptionType.BadDestination, Validate(boardInfo, postMoveBoard, move), "Should return error when the destination is invalid.");
        }
    }
}