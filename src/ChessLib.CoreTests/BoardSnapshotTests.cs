using NUnit.Framework;

namespace ChessLib.Core.Tests
{
    [TestFixture]
    public class BoardSnapshotTests
    {
        [SetUp]
        public void SetUp()
        {
            gameMove = PostMoveStateFactory.ApplyMoveToBoard(CoreTestConstants.EnglishTabiyaBoard,
                CoreTestConstants.EnglishTabiyaNextMove, out postMoveBoard);
        }

        private Board postMoveBoard;
        private PostMoveState gameMove;


        [Test]
        public void BoardStateHashTest()
        {
            Assert.AreEqual(CoreTestConstants.EnglishTabiyaBoardStateHash, gameMove.BoardStateHash);
        }
    }
}