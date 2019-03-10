using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicBitboard;
using MagicBitboard.Helpers;
using NUnit.Framework;
namespace ChessLib.Tests
{
    [TestFixture]
    public class TestMoves
    {
        [Test]
        public void TestInitializeFileMasks()
        {
            Assert.AreEqual(0x101010101010101, MoveHelpers.FileMasks[0]);
            Assert.AreEqual(0x202020202020202, MoveHelpers.FileMasks[1]);
            Assert.AreEqual(0x404040404040404, MoveHelpers.FileMasks[2]);
            Assert.AreEqual(0x808080808080808, MoveHelpers.FileMasks[3]);
            Assert.AreEqual(0x1010101010101010, MoveHelpers.FileMasks[4]);
            Assert.AreEqual(0x2020202020202020, MoveHelpers.FileMasks[5]);
            Assert.AreEqual(0x4040404040404040, MoveHelpers.FileMasks[6]);
            Assert.AreEqual(0x8080808080808080, MoveHelpers.FileMasks[7]);
        }

        [Test]
        public void TestInitializeRankMasks()
        {
            Assert.AreEqual(0xff, MoveHelpers.RankMasks[0]);
            Assert.AreEqual(0xff00, MoveHelpers.RankMasks[1]);
            Assert.AreEqual(0xff0000, MoveHelpers.RankMasks[2]);
            Assert.AreEqual(0xff000000, MoveHelpers.RankMasks[3]);
            Assert.AreEqual(0xff00000000, MoveHelpers.RankMasks[4]);
            Assert.AreEqual(0xff0000000000, MoveHelpers.RankMasks[5]);
            Assert.AreEqual(0xff000000000000, MoveHelpers.RankMasks[6]);
            Assert.AreEqual(0xff00000000000000, MoveHelpers.RankMasks[7]);
        }
        [Test]
        public void TestDisplayBitsMSB()
        {
            ulong testVal = 0x8000000000000000;
            var expected =
                "0 0 0 0 0 0 0 1\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n";
            Assert.AreEqual(expected, MoveHelpers.GetDisplayBits(testVal));
        }
        [Test]
        public void TestDisplayBitsLSB()
        {
            ulong testVal = 0x0000000000000001;
            var expected =
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "1 0 0 0 0 0 0 0\r\n";
            Assert.AreEqual(expected, MoveHelpers.GetDisplayBits(testVal));
        }

       

     



    }
}
