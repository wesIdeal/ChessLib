using System.Collections.Generic;
using ChessLib.Core;
using NUnit.Framework;

namespace ChessLib.Core.Tests
{
    [TestFixture]
    public class BoardSnapshotTests 
    {
        private readonly GameMove _gameMove = new GameMove(CoreTestConstants.EnglishTabiyaBoard,
            CoreTestConstants.EnglishTabiyaNextMove);

        [Test]
        public void BoardSnapshotConstructionTest()
        {
            ;

            Assert.AreNotSame(CoreTestConstants.EnglishTabiyaBoard, _gameMove.Board);
        }

        [Test]
        public void BoardStateHashTest()
        {
            Assert.AreEqual(CoreTestConstants.EnglishTabiyaBoardStateHash, _gameMove.BoardStateHash);
        }

      
    }
}