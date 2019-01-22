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
            var sq = Square.FromString("a8");
            var movedSq = MoveN(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNTestNormal()
        {
            var sq = Square.FromString("a1");
            var movedSq = MoveN(sq);
            Assert.AreEqual(Square.FromString("a2"), movedSq);
        }
        #endregion

        #region MoveNE Tests
        [Test]
        public void MoveNETestBeyondFile()
        {
            var sq = Square.FromString("h1");
            var movedSq = MoveNE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNETestBeyondRank()
        {
            var sq = Square.FromString("a8");
            var movedSq = MoveNE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNETestNormal()
        {
            var sq = Square.FromString("a1");
            var movedSq = MoveNE(sq);
            Assert.AreEqual(Square.FromString("b2"), movedSq);
        }
        #endregion

        #region MoveE Tests

        [Test]
        public void MoveETestBeyondFile()
        {
            var sq = Square.FromString("h1");
            var movedSq = MoveE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveETestNormal()
        {
            var sq = Square.FromString("a1");
            var movedSq = MoveE(sq);
            Assert.AreEqual(Square.FromString("b1"), movedSq);
        }
        #endregion

        #region MoveSE Tests
        [Test]
        public void MoveSETestBeyondFile()
        {
            var sq = Square.FromString("h2");
            var movedSq = MoveSE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSETestBeyondRank()
        {
            var sq = Square.FromString("g1");
            var movedSq = MoveSE(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSETestNormal()
        {
            var sq = Square.FromString("g2");
            var movedSq = MoveSE(sq);
            Assert.AreEqual(Square.FromString("h1"), movedSq);
        }
        #endregion

        #region MoveS Tests

        [Test]
        public void MoveSTestBeyondRank()
        {
            var sq = Square.FromString("d1");
            var movedSq = MoveS(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSTestNormal()
        {
            var sq = Square.FromString("d2");
            var movedSq = MoveS(sq);
            Assert.AreEqual(Square.FromString("d1"), movedSq);
        }
        #endregion

        #region MoveSW Tests
        [Test]
        public void MoveSWTestBeyondFile()
        {
            var sq = Square.FromString("a2");
            var movedSq = MoveSW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSWTestBeyondRank()
        {
            var sq = Square.FromString("d1");
            var movedSq = MoveSW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveSWTestNormal()
        {
            var sq = Square.FromString("g2");
            var movedSq = MoveSW(sq);
            Assert.AreEqual(Square.FromString("f1"), movedSq);
        }
        #endregion

        #region MoveW Tests

        [Test]
        public void MoveWTestBeyondFile()
        {
            var sq = Square.FromString("a1");
            var movedSq = MoveW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveWTestNormal()
        {
            var sq = Square.FromString("d2");
            var movedSq = MoveW(sq);
            Assert.AreEqual(Square.FromString("c2"), movedSq);
        }
        #endregion

        #region MoveNW Tests
        [Test]
        public void MoveNWTestBeyondFile()
        {
            var sq = Square.FromString("a7");
            var movedSq = MoveNW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNWTestBeyondRank()
        {
            var sq = Square.FromString("d8");
            var movedSq = MoveNW(sq);
            Assert.AreEqual(null, movedSq);
        }
        [Test]
        public void MoveNWTestNormal()
        {
            var sq = Square.FromString("b7");
            var movedSq = MoveNW(sq);
            Assert.AreEqual(Square.FromString("a8"), movedSq);
        }
        #endregion
    }
}
