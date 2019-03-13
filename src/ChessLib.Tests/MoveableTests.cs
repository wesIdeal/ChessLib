using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib;
namespace ChessLib.Tests
{
    [TestFixture]
    public class MoveableTests : Movable
    {
        #region MoveN Tests
       
        [Test]
        public void MoveNTestBeyondRank()
        {
            var sq = new Square("a8");
            var movedSq = MoveN(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNTestNormal()
        {
            var sq = new Square("a1");
            var movedSq = MoveN(sq);
            Assert.AreEqual(new Square("a2"), movedSq);
        }
        #endregion

        #region MoveNE Tests
        [Test]
        public void MoveNETestBeyondFile()
        {
            var sq = new Square("h1");
            var movedSq = MoveNE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNETestBeyondRank()
        {
            var sq = new Square("a8");
            var movedSq = MoveNE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNETestNormal()
        {
            var sq = new Square("a1");
            var movedSq = MoveNE(sq);
            Assert.AreEqual(new Square("b2"), movedSq);
        }
        #endregion

        #region MoveE Tests

        [Test]
        public void MoveETestBeyondFile()
        {
            var sq = new Square("h1");
            var movedSq = MoveE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveETestNormal()
        {
            var sq = new Square("a1");
            var movedSq = MoveE(sq);
            Assert.AreEqual(new Square("b1"), movedSq);
        }
        #endregion

        #region MoveSE Tests
        [Test]
        public void MoveSETestBeyondFile()
        {
            var sq = new Square("h2");
            var movedSq = MoveSE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSETestBeyondRank()
        {
            var sq = new Square("g1");
            var movedSq = MoveSE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSETestNormal()
        {
            var sq = new Square("g2");
            var movedSq = MoveSE(sq);
            Assert.AreEqual(new Square("h1"), movedSq);
        }
        #endregion

        #region MoveS Tests

        [Test]
        public void MoveSTestBeyondRank()
        {
            var sq = new Square("d1");
            var movedSq = MoveS(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSTestNormal()
        {
            var sq = new Square("d2");
            var movedSq = MoveS(sq);
            Assert.AreEqual(new Square("d1"), movedSq);
        }
        #endregion

        #region MoveSW Tests
        [Test]
        public void MoveSWTestBeyondFile()
        {
            var sq = new Square("a2");
            var movedSq = MoveSW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSWTestBeyondRank()
        {
            var sq = new Square("d1");
            var movedSq = MoveSW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSWTestNormal()
        {
            var sq = new Square("g2");
            var movedSq = MoveSW(sq);
            Assert.AreEqual(new Square("f1"), movedSq);
        }
        #endregion

        #region MoveW Tests

        [Test]
        public void MoveWTestBeyondFile()
        {
            var sq = new Square("a1");
            var movedSq = MoveW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveWTestNormal()
        {
            var sq = new Square("d2");
            var movedSq = MoveW(sq);
            Assert.AreEqual(new Square("c2"), movedSq);
        }
        #endregion

        #region MoveNW Tests
        [Test]
        public void MoveNWTestBeyondFile()
        {
            var sq = new Square("a7");
            var movedSq = MoveNW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNWTestBeyondRank()
        {
            var sq = new Square("d8");
            var movedSq = MoveNW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNWTestNormal()
        {
            var sq = new Square("b7");
            var movedSq = MoveNW(sq);
            Assert.AreEqual(new Square("a8"), movedSq);
        }
        #endregion
    }
}
