using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicBitboard;
using MagicBitboard.Helpers;
using MagicBitboard.Enums;

namespace MagicBitboard.Tests
{
    [TestFixture]
    public class BitboardTests
    {

        [OneTimeSetUp]
        public void OneTimeSetup()
        {

        }
        //[Test]
        //public void TestQueenAttacksAtRandom()
        //{
        //    Bitboard _bb = new Bitboard();
        //    var occupancy = 0x1000818011030ul;
        //    var expectedAttackBoard = 0x805031e;
        //    var actual = _bb.GetAttackedSquares(Piece.Queen, 0, occupancy);
        //    Assert.AreEqual(expectedAttackBoard, actual);
        //}

        #region FEN

        const string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        [Test]
        public void TestInvalidHalfmoveClock()
        {
            var message = "";
            Assert.Throws(typeof(FENException), () =>
            {
                try { FENHelpers.GetMoveNumberFromString("-1"); }
                catch(FENException e) { message = e.Message; throw; }
            });
        }
        [Test]
        public void TestBlankStringInHalfmoveClock()
        {
            var message = "";
            Assert.Throws(typeof(FENException), () =>
            {
                try { FENHelpers.GetMoveNumberFromString(""); }
                catch (FENException e) { message = e.Message; throw; }
            });
        }
        [Test]
        public void TestHalfmoveClock()
        {
            Assert.AreEqual((uint?)24, FENHelpers.GetMoveNumberFromString("24"));
        }
        [Test]
        public void TestInvalidActiceColor()
        {
            Assert.Throws(typeof(FENException), () => { FENHelpers.GetActiveColor("z"); });
        }
        [Test]
        public void TestActiceColorWhite()
        {
            var expected = Color.White;
            Assert.AreEqual(expected, FENHelpers.GetActiveColor("w"));
        }
        [Test]
        public void TestActiveColorBlack()
        {
            var expected = Color.Black;
            Assert.AreEqual(expected, FENHelpers.GetActiveColor("b"));
        }
        [Test]
        public void TestInvalidCastlingAvailabilityEmptyString()
        {
            var message = "";
            Assert.Throws(typeof(FENException), () =>
            {
                try { FENHelpers.GetMoveNumberFromString(""); }
                catch (FENException e) { message = e.Message; throw; }
            });
        }

        [Test]
        public void TestIncompleteFEN()
        {
            var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0";
            Assert.Throws(typeof(FENException), () => { new Bitboard(fen); });
        }
        [Test]
        public void TestTooManyRanks()
        {
            var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            var message = "";
            Assert.Throws(typeof(FENException), () =>
            {
                try
                {
                    new Bitboard(fen);
                }
                catch (FENException f)
                {
                    message = f.Message;
                    throw;
                }
            });
            Console.WriteLine(message);
        }


        [Test]
        public void TestInvalidCharsFEN()
        {
            var fen = "rnbqkbnr/ppspppzp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 0";
            var message = "";
            Assert.Throws(typeof(FENException), () =>
            {
                try
                {
                    new Bitboard(fen);
                }
                catch (FENException f)
                {
                    message = f.Message;
                    throw;
                }
            });
            Console.WriteLine(message);
        }
        [Test]
        public void TestNotEnoughRanks()
        {
            var fen = "rnbqkbnr/pppppppp/8/8/8/8/RNBQKBNR w KQkq - 0 1";
            var message = "";
            Assert.Throws(typeof(FENException), () =>
            {
                try
                {
                    new Bitboard(fen);
                }
                catch (FENException f)
                {
                    message = f.Message;
                    throw;
                }
            });
            Console.WriteLine(message);
        }

        [Test]
        public void TestTooManyPieces()
        {
            var fen = "rnbqkbnr/ppppppppp/8/8/8/8/PPPPPPPP/RNBQrKBNR w KQkq - 0 1";
            var message = "";
            Assert.Throws(typeof(FENException), () =>
            {
                try
                {
                    new Bitboard(fen);
                }
                catch (FENException f)
                {
                    message = f.Message;
                    throw;
                }
            });
            Console.WriteLine(message);
        }
        [Test]
        public void TestNotEnoughPieces()
        {
            var fen = "rnbqkbnr/ppppppp/8/8/8/8/PPPPPPPP/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            var message = "";
            Assert.Throws(typeof(FENException), () =>
            {
                try
                {
                    new Bitboard(fen);
                }
                catch (FENException f)
                {
                    message = f.Message;
                    throw;
                }
            });
            Console.WriteLine(message);
        }
        #endregion
    }
}
