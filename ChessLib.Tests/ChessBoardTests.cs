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
        public const string initialFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public const string oneCaptureAvailable = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2";
        public const string twoCapturesAvailable = "rnbqkbnr/ppp1p1pp/8/3p1p2/4P1P1/8/PPPP1P1P/RNBQKBNR w KQkq d6 0 2";
        public const string preEnPassentPosition = "rnbqkbnr/pp1ppppp/8/2p1P3/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2";
        public const string enPassentPosition = "rnbqkbnr/pp1pp1pp/8/2p1Pp2/8/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 3";

        #region Applying Moves Tests
        [Test]
        public void TestInterprettingLegalPawnMoveE4()
        {
            var board = new ChessBoard();
            var move = board.InterpretMove("e4", Color.White);
            Assert.AreEqual(new Move(Square.FromString("e2"), Square.FromString("e4"), false, Piece.Pawn), move);
        }
        #endregion

        #region Pawn Move Tests
        [Test]
        public void TestEnPassentSquareFlagged()
        {
            var board = new ChessBoard(preEnPassentPosition);
            board.MovePiece("f5", Color.Black);
        }
        [Test]
        public void TestStartingPositionWhite()
        {
            var board = new ChessBoard(initialFen);
            var pawnSq = Square.FromString("e2");
            var expectedTargets = new[] { Square.FromString("e3"), Square.FromString("e4") };
            var receivedTargets = board.GetTargetSquares(pawnSq);
            Assert.AreEqual(expectedTargets.Length, receivedTargets.Count());
            foreach (var target in expectedTargets)
            {
                Assert.Contains(target, receivedTargets.ToList());
            }
        }
        [Test]
        public void TestOneCapturePositionWhite()
        {
            var board = new ChessBoard(oneCaptureAvailable);
            var pawnSq = Square.FromString("e4");
            var expectedTargets = new[] { Square.FromString("d5"), Square.FromString("e5") };
            var receivedTargets = board.GetTargetSquares(pawnSq);
            Assert.AreEqual(expectedTargets.Length, receivedTargets.Count());
            foreach (var target in expectedTargets)
            {
                Assert.Contains(target, receivedTargets.ToList());
            }
        }
        [Test]
        public void TestTwoCapturePositionWhite()
        {
            var board = new ChessBoard(twoCapturesAvailable);
            var pawnSq = Square.FromString("e4");
            var expectedTargets = new[] { Square.FromString("d5"), Square.FromString("e5"), Square.FromString("f5") };
            var receivedTargets = board.GetTargetSquares(pawnSq);
            Assert.AreEqual(expectedTargets.Length, receivedTargets.Count());
            foreach (var target in expectedTargets)
            {
                Assert.Contains(target, receivedTargets.ToList());
            }
        }
        [Test]
        public void TestTwoCapturePositionBlack()
        {
            var board = new ChessBoard(twoCapturesAvailable);
            var pawnSq = Square.FromString("f5");
            var expectedTargets = new[] { Square.FromString("g4"), Square.FromString("e4"), Square.FromString("f4") };
            var receivedTargets = board.GetTargetSquares(pawnSq);
            Assert.AreEqual(expectedTargets.Length, receivedTargets.Count());
            foreach (var target in expectedTargets)
            {
                Assert.Contains(target, receivedTargets.ToList());
            }
        }
        [Test]
        public void TestNoMoves()
        {
            var board = new ChessBoard("rnbqkbnr/ppp1pppp/8/8/8/3p4/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            var pawnSq = Square.FromString("d2");

            var receivedTargets = board.GetTargetSquares(pawnSq);
            Assert.AreEqual(0, receivedTargets.Count());

        }
        [Test]
        public void TestOneMove()
        {
            var board = new ChessBoard("rnbqkbnr/ppp1pppp/8/8/3p4/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            var pawnSq = Square.FromString("d2");

            var receivedTargets = board.GetTargetSquares(pawnSq).ToList();
            Assert.Contains(Square.FromString("d3"), receivedTargets);

        }


        [Test]
        public void TestStartingPositionBlack()
        {
            var board = new ChessBoard(initialFen);
            var pawnSq = Square.FromString("e7");
            var expectedTargets = new[] { Square.FromString("e6"), Square.FromString("e5") };
            var receivedTargets = board.GetTargetSquares(pawnSq);
            Assert.AreEqual(expectedTargets.Length, receivedTargets.Count());
            foreach (var target in expectedTargets)
            {
                Assert.Contains(target, receivedTargets.ToList());
            }
        }
        [Test]
        public void TestOneCapturePositionBlack()
        {
            var board = new ChessBoard(oneCaptureAvailable);
            var pawnSq = Square.FromString("d5");
            var expectedTargets = new[] { Square.FromString("d4"), Square.FromString("e4") };
            var receivedTargets = board.GetTargetSquares(pawnSq);
            Assert.AreEqual(expectedTargets.Length, receivedTargets.Count());
            foreach (var target in expectedTargets)
            {
                Assert.Contains(target, receivedTargets.ToList());
            }
        }

        #endregion Pawn Move Tests

        #region Knight Move Tests
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
        #endregion

        #region Bishop Move Tests
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
        #endregion

        #region Rook Move Tests
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
        #endregion
    }
}
