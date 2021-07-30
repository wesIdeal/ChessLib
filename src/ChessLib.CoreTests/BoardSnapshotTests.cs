using ChessLib.Core;
using NUnit.Framework;

namespace ChessLib.Core.Tests
{
    [TestFixture]
    public class BoardSnapshotTests
    {
        private readonly BoardSnapshot _boardSnapshot = new BoardSnapshot(CoreTestConstants.EnglishTabiyaBoard,
            CoreTestConstants.EnglishTabiyaNextMove);

        [Test]
        public void BoardSnapshotConstructionTest()
        {
            ;

            Assert.AreNotSame(CoreTestConstants.EnglishTabiyaBoard, _boardSnapshot.Board);
        }

        [Test]
        public void BoardStateHashTest()
        {
            Assert.AreEqual(CoreTestConstants.EnglishTabiyaBoardStateHash, _boardSnapshot.BoardStateHash);
        }

    }
}