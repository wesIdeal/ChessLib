using MagicBitboard;
using MagicBitboard.Helpers;
using NUnit.Framework;
using System;

namespace MagicBitboard.Helpers.Tests
{
    [TestFixture]
    public class MoveHelperTests
    {
        [Test]
        public void TestHyphenAsSquareTextReturnsNull()
        {
            var expected = (ushort?)null;
            Assert.AreEqual(expected, MoveHelpers.SquareTextToIndex("-"));
        }

        [Test]
        public void TestEmptyTextThrowsExc()
        {
            var expected = (ushort?)null;
            Assert.Throws(typeof(ArgumentException), () => { MoveHelpers.SquareTextToIndex(""); });
        }

        [Test]
        public void TestShortSquareTextThrowsExc()
        {
            var expected = (ushort?)null;
            Assert.Throws(typeof(ArgumentException), () => { MoveHelpers.SquareTextToIndex("a"); });
        }
        [Test]
        public void TestLongSquareTextThrowsExc()
        {
            var expected = (ushort?)null;
            var message = "";
            Assert.Throws(typeof(ArgumentException), () =>
            {
                try
                {
                    MoveHelpers.SquareTextToIndex("a22");
                }
                catch (ArgumentException a)
                {
                    message = a.Message;
                    throw;
                }
            });
            Console.Write(message);
        }
        [Test]
        public void TestFileOutOfRangeSquareTextThrowsExc()
        {
            var expected = (ushort?)null;
            var message = "";
            Assert.Throws(typeof(ArgumentException), () =>
            {
                try
                {
                    MoveHelpers.SquareTextToIndex("i2");
                }
                catch (ArgumentException a)
                {
                    message = a.Message;
                    throw;
                }
            });
            Console.Write(message);
        }

        [Test]
        public void IndividualSquareValidityTest()
        {
            Assert.AreEqual(0x0000000000000001, MoveHelpers.IndividialSquares[0, 0]);
            Assert.AreEqual(0x0000000000000002, MoveHelpers.IndividialSquares[0, 1]);
            Assert.AreEqual(0x0000000000000004, MoveHelpers.IndividialSquares[0, 2]);
            Assert.AreEqual(0x0000000000000008, MoveHelpers.IndividialSquares[0, 3]);
            Assert.AreEqual(0x0000000000000010, MoveHelpers.IndividialSquares[0, 4]);
            Assert.AreEqual(0x0000000000000020, MoveHelpers.IndividialSquares[0, 5]);
            Assert.AreEqual(0x0000000000000040, MoveHelpers.IndividialSquares[0, 6]);
            Assert.AreEqual(0x0000000000000080, MoveHelpers.IndividialSquares[0, 7]);
            Assert.AreEqual(0x0000000000000100, MoveHelpers.IndividialSquares[1, 0]);
            Assert.AreEqual(0x0000000000000200, MoveHelpers.IndividialSquares[1, 1]);
            Assert.AreEqual(0x0000000000000400, MoveHelpers.IndividialSquares[1, 2]);
            Assert.AreEqual(0x0000000000000800, MoveHelpers.IndividialSquares[1, 3]);
            Assert.AreEqual(0x0000000000001000, MoveHelpers.IndividialSquares[1, 4]);
            Assert.AreEqual(0x0000000000002000, MoveHelpers.IndividialSquares[1, 5]);
            Assert.AreEqual(0x0000000000004000, MoveHelpers.IndividialSquares[1, 6]);
            Assert.AreEqual(0x0000000000008000, MoveHelpers.IndividialSquares[1, 7]);
            Assert.AreEqual(0x0000000000010000, MoveHelpers.IndividialSquares[2, 0]);
            Assert.AreEqual(0x0000000000020000, MoveHelpers.IndividialSquares[2, 1]);
            Assert.AreEqual(0x0000000000040000, MoveHelpers.IndividialSquares[2, 2]);
            Assert.AreEqual(0x0000000000080000, MoveHelpers.IndividialSquares[2, 3]);
            Assert.AreEqual(0x0000000000100000, MoveHelpers.IndividialSquares[2, 4]);
            Assert.AreEqual(0x0000000000200000, MoveHelpers.IndividialSquares[2, 5]);
            Assert.AreEqual(0x0000000000400000, MoveHelpers.IndividialSquares[2, 6]);
            Assert.AreEqual(0x0000000000800000, MoveHelpers.IndividialSquares[2, 7]);
            Assert.AreEqual(0x0000000001000000, MoveHelpers.IndividialSquares[3, 0]);
            Assert.AreEqual(0x0000000002000000, MoveHelpers.IndividialSquares[3, 1]);
            Assert.AreEqual(0x0000000004000000, MoveHelpers.IndividialSquares[3, 2]);
            Assert.AreEqual(0x0000000008000000, MoveHelpers.IndividialSquares[3, 3]);
            Assert.AreEqual(0x0000000010000000, MoveHelpers.IndividialSquares[3, 4]);
            Assert.AreEqual(0x0000000020000000, MoveHelpers.IndividialSquares[3, 5]);
            Assert.AreEqual(0x0000000040000000, MoveHelpers.IndividialSquares[3, 6]);
            Assert.AreEqual(0x0000000080000000, MoveHelpers.IndividialSquares[3, 7]);
            Assert.AreEqual(0x0000000100000000, MoveHelpers.IndividialSquares[4, 0]);
            Assert.AreEqual(0x0000000200000000, MoveHelpers.IndividialSquares[4, 1]);
            Assert.AreEqual(0x0000000400000000, MoveHelpers.IndividialSquares[4, 2]);
            Assert.AreEqual(0x0000000800000000, MoveHelpers.IndividialSquares[4, 3]);
            Assert.AreEqual(0x0000001000000000, MoveHelpers.IndividialSquares[4, 4]);
            Assert.AreEqual(0x0000002000000000, MoveHelpers.IndividialSquares[4, 5]);
            Assert.AreEqual(0x0000004000000000, MoveHelpers.IndividialSquares[4, 6]);
            Assert.AreEqual(0x0000008000000000, MoveHelpers.IndividialSquares[4, 7]);
            Assert.AreEqual(0x0000010000000000, MoveHelpers.IndividialSquares[5, 0]);
            Assert.AreEqual(0x0000020000000000, MoveHelpers.IndividialSquares[5, 1]);
            Assert.AreEqual(0x0000040000000000, MoveHelpers.IndividialSquares[5, 2]);
            Assert.AreEqual(0x0000080000000000, MoveHelpers.IndividialSquares[5, 3]);
            Assert.AreEqual(0x0000100000000000, MoveHelpers.IndividialSquares[5, 4]);
            Assert.AreEqual(0x0000200000000000, MoveHelpers.IndividialSquares[5, 5]);
            Assert.AreEqual(0x0000400000000000, MoveHelpers.IndividialSquares[5, 6]);
            Assert.AreEqual(0x0000800000000000, MoveHelpers.IndividialSquares[5, 7]);
            Assert.AreEqual(0x0001000000000000, MoveHelpers.IndividialSquares[6, 0]);
            Assert.AreEqual(0x0002000000000000, MoveHelpers.IndividialSquares[6, 1]);
            Assert.AreEqual(0x0004000000000000, MoveHelpers.IndividialSquares[6, 2]);
            Assert.AreEqual(0x0008000000000000, MoveHelpers.IndividialSquares[6, 3]);
            Assert.AreEqual(0x0010000000000000, MoveHelpers.IndividialSquares[6, 4]);
            Assert.AreEqual(0x0020000000000000, MoveHelpers.IndividialSquares[6, 5]);
            Assert.AreEqual(0x0040000000000000, MoveHelpers.IndividialSquares[6, 6]);
            Assert.AreEqual(0x0080000000000000, MoveHelpers.IndividialSquares[6, 7]);
            Assert.AreEqual(0x0100000000000000, MoveHelpers.IndividialSquares[7, 0]);
            Assert.AreEqual(0x0200000000000000, MoveHelpers.IndividialSquares[7, 1]);
            Assert.AreEqual(0x0400000000000000, MoveHelpers.IndividialSquares[7, 2]);
            Assert.AreEqual(0x0800000000000000, MoveHelpers.IndividialSquares[7, 3]);
            Assert.AreEqual(0x1000000000000000, MoveHelpers.IndividialSquares[7, 4]);
            Assert.AreEqual(0x2000000000000000, MoveHelpers.IndividialSquares[7, 5]);
            Assert.AreEqual(0x4000000000000000, MoveHelpers.IndividialSquares[7, 6]);
            Assert.AreEqual(0x8000000000000000, MoveHelpers.IndividialSquares[7, 7]);
        }

        [Test]
        public void IndexToFile()
        {
            for (int i = 0; i < 64; i++)
            {
                char expected;
                switch (i % 8)
                {
                    case 0: expected = 'a'; break;
                    case 1: expected = 'b'; break;
                    case 2: expected = 'c'; break;
                    case 3: expected = 'd'; break;
                    case 4: expected = 'e'; break;
                    case 5: expected = 'f'; break;
                    case 6: expected = 'g'; break;
                    case 7: expected = 'h'; break;
                    default: throw new Exception("Error in IndexToChar test.");
                }
                Assert.AreEqual(expected, i.IndexToFileDisplay());
            }
        }
        [Test]
        public void IndexToRank()
        {
            for (int i = 0; i < 64; i++)
            {
                char expected;
                switch (i / 8)
                {
                    case 0: expected = '1'; break;
                    case 1: expected = '2'; break;
                    case 2: expected = '3'; break;
                    case 3: expected = '4'; break;
                    case 4: expected = '5'; break;
                    case 5: expected = '6'; break;
                    case 6: expected = '7'; break;
                    case 7: expected = '8'; break;
                    default: throw new Exception("Error in IndexToChar test.");
                }
                Assert.AreEqual(expected, i.IndexToRankDisplay());
            }
        }

    }
}
