using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib;
using System.Diagnostics;

namespace ChessLib.Tests
{
    [TestFixture]
    public class TestUtilities
    {
        private readonly char?[,] GoodBoard = new char?[8, 8] { { 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' }, { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' }, { null, null, null, null, null, null, null, null }, { null, null, null, null, null, null, null, null }, { null, null, null, null, null, null, null, null }, { null, null, null, null, null, null, null, null }, { 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' }, { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' } };
        private const string initialBoard = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private const string tooManyEmptySquares = "rnbqkbnr/pppppppp/9/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private const string notEnoughPieces = "rnbqkbnr/pppppppp/9/8/8/8/PPPPPPPP/RNBQKBNR w - 0 1";
        private const string invalidActiveColor = "rnbqkbnr/pppppppp/9/8/8/8/PPPPPPPP/RNBQKBNR c KQkq - 0 1";
        private const string validFENNoEnPassent = "rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2";
        private const string validFENWithEnPassent = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
        private const string validFENNoCastling = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b - e3 0 1";
        private BoardProperties bp = new BoardProperties();

        [Test]
        public void BoardFromFenTooManyEmptySquaresOnRank6()
        {
            var ex = Assert.Throws<FENException>(() => Utilities.BoardFromFEN(tooManyEmptySquares, out bp));
            Assert.AreEqual("Invalid rank passed from FEN on rank 6.", ex.Message);
        }
        [Test]
        public void NotEnoughPieces()
        {
            Assert.Throws<ArgumentException>(() => Utilities.GetBoardPropertiesFromFEN(notEnoughPieces));
        }
        [Test]
        public void TestInvalidActiveColor()
        {
            Assert.Throws<ArgumentException>(() => Utilities.GetBoardPropertiesFromFEN(invalidActiveColor));
        }

        [Test]
        public void NormalBoard()
        {
            var board = Utilities.BoardFromFEN(initialBoard, out bp);
            TestContext.Out.Write(board.ToString());
            Assert.AreEqual(GoodBoard, board);
            
        }

        [Test]
        public void TestFENPropertiesValid()
        {
            bp = Utilities.GetBoardPropertiesFromFEN(validFENWithEnPassent);
            Assert.AreEqual(Color.Black, bp.ActiveColor);
            Assert.IsTrue(bp.CanWhiteCastleQueenSide);
            Assert.IsTrue(bp.CanWhiteCastleKingSide);
            Assert.IsTrue(bp.CanBlackCastleQueenSide);
            Assert.IsTrue(bp.CanBlackCastleKingSide);
            Assert.AreEqual(new Square("e3"), bp.EnPassentSquare);
            Assert.AreEqual(0, bp.HalfmoveClock);
            Assert.AreEqual(1, bp.FullMoveNumber);
        }
        [Test]
        public void TestFENPropertiesValidNoEnPassent()
        {
            bp = Utilities.GetBoardPropertiesFromFEN(validFENNoEnPassent);
            Assert.AreEqual(null, bp.EnPassentSquare);

        }

        [Test]
        public void TestFENNoCasting()
        {
            bp = Utilities.GetBoardPropertiesFromFEN(validFENNoCastling);
            Assert.IsFalse(bp.CanWhiteCastleQueenSide);
            Assert.IsFalse(bp.CanWhiteCastleKingSide);
            Assert.IsFalse(bp.CanBlackCastleQueenSide);
            Assert.IsFalse(bp.CanBlackCastleKingSide);
        }

        [Test]
        public void ArrayRankToRealRank()
        {
            Assert.AreEqual(8, Utilities.ArrayRankToRealRank(0));
            Assert.AreEqual(7, Utilities.ArrayRankToRealRank(1));
            Assert.AreEqual(6, Utilities.ArrayRankToRealRank(2));
            Assert.AreEqual(5, Utilities.ArrayRankToRealRank(3));
            Assert.AreEqual(4, Utilities.ArrayRankToRealRank(4));
            Assert.AreEqual(3, Utilities.ArrayRankToRealRank(5));
            Assert.AreEqual(2, Utilities.ArrayRankToRealRank(6));
            Assert.AreEqual(1, Utilities.ArrayRankToRealRank(7));
            Assert.Throws<ArgumentException>(() => Utilities.ArrayRankToRealRank(8));
            Assert.Throws<ArgumentException>(() => Utilities.ArrayRankToRealRank(-1));
        }

        #region Target Squares
        [Test]
        public void TestTargetSquaresForKnightOnRim()
        {
            var g1Origin = new Square(File.g, 0);
            var targetsFromg1 = new[] { new Square(File.e, 1), new Square(File.f, 2), new Square(File.h, 2) };
            var retrievedTargets = Utilities.GetTargetSquaresForKnight(g1Origin);
            Assert.AreEqual(targetsFromg1.Count(), retrievedTargets.Count());
            foreach (var target in targetsFromg1)
            {
                Assert.Contains(target, retrievedTargets.ToList());
            }
        }
        [Test]
        public void TestTargetSquaresForKnightInMiddle()
        {
            var e3Origin = new Square(File.e, 3);
            var targetsFromg1 = new[] { new Square(File.d, 5), new Square(File.f, 5), new Square(File.g, 4), new Square(File.g, 2), new Square(File.f, 1), new Square(File.d, 1), new Square(File.c, 2), new Square(File.c, 4 )};
            var retrievedTargets = Utilities.GetTargetSquaresForKnight(e3Origin);
            Assert.AreEqual(targetsFromg1.Count(), retrievedTargets.Count());
            foreach (var target in targetsFromg1)
            {
                Assert.Contains(target, retrievedTargets.ToList());
            }
        }


        #endregion
    }
}
