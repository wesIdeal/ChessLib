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
    class ChessBoardTests
    {
        [Test]
        public void GetKnightTargetsFromOccupiedSquare()
        {
            var board = new ChessBoard();
            var g1Origin = new Square(File.g, 0);
            var targetsFromg1 = new[] { new Square(File.f, 2), new Square(File.h, 2) };
            var retrievedTargets = board.GetTargetSquares(g1Origin);
            Assert.AreEqual(targetsFromg1.Count(), retrievedTargets.Count());
            foreach (var target in targetsFromg1)
            {
                Assert.Contains(target, retrievedTargets.ToList());
            }

        }

        [Test]
        public void TestBishopMoves1()
        {
            var board = new ChessBoard("r1bqkbnr/1ppp1ppp/p1n5/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4");
            var a4 = Square.FromString("a4");
            var targetsFroma4 = new[] { Square.FromString("b5"), Square.FromString("c6"), Square.FromString("b3") };
            var retrievedTargets = board.GetTargetSquares(a4);
            Assert.AreEqual(targetsFroma4.Count(), retrievedTargets.Count());
            foreach (var target in targetsFroma4)
            {
                Assert.Contains(target, retrievedTargets.ToList());
            }
        }
        [Test]
        public void TestBishopMoves2()
        {
            var board = new ChessBoard("r1bqkbnr/2pp1ppp/p1n5/1p2p3/4P3/1B3N2/PPPP1PPP/RNBQK2R b KQkq - 1 5");
            var b3 = Square.FromString("b3");
            var targetsFromb3 = new[] { Square.FromString("a4"), Square.FromString("c4"), Square.FromString("d5"), Square.FromString("e6"), Square.FromString("f7") };
            var retrievedTargets = board.GetTargetSquares(b3);
            Assert.AreEqual(targetsFromb3.Count(), retrievedTargets.Count());
            foreach (var target in targetsFromb3)
            {
                Assert.Contains(target, retrievedTargets.ToList());
            }
        }

        [Test]
        public void TestRookMoves()
        {
            var board = new ChessBoard("rnbqkbnr/pppppp1p/6p1/8/4P1R1/8/PPPP1PPP/RNBQKBN1 w KQkq - 0 0");
            var g4 = Square.FromString("g4");
            var targetsFroma4 = new[] { Square.FromString("f4"), Square.FromString("g3"), Square.FromString("h4"), Square.FromString("g5"), Square.FromString("g6") };
            var retrievedTargets = board.GetTargetSquares(g4);
            Assert.AreEqual(targetsFroma4.Count(), retrievedTargets.Count());
            foreach (var target in targetsFroma4)
            {
                Assert.Contains(target, retrievedTargets.ToList());
            }
        }
    }
}
