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
            Assert.AreEqual(0x101010101010101, BoardHelpers.FileMasks[0]);
            Assert.AreEqual(0x202020202020202, BoardHelpers.FileMasks[1]);
            Assert.AreEqual(0x404040404040404, BoardHelpers.FileMasks[2]);
            Assert.AreEqual(0x808080808080808, BoardHelpers.FileMasks[3]);
            Assert.AreEqual(0x1010101010101010, BoardHelpers.FileMasks[4]);
            Assert.AreEqual(0x2020202020202020, BoardHelpers.FileMasks[5]);
            Assert.AreEqual(0x4040404040404040, BoardHelpers.FileMasks[6]);
            Assert.AreEqual(0x8080808080808080, BoardHelpers.FileMasks[7]);
        }

        [Test]
        public void TestInitializeRankMasks()
        {
            Assert.AreEqual(0xff, BoardHelpers.RankMasks[0]);
            Assert.AreEqual(0xff00, BoardHelpers.RankMasks[1]);
            Assert.AreEqual(0xff0000, BoardHelpers.RankMasks[2]);
            Assert.AreEqual(0xff000000, BoardHelpers.RankMasks[3]);
            Assert.AreEqual(0xff00000000, BoardHelpers.RankMasks[4]);
            Assert.AreEqual(0xff0000000000, BoardHelpers.RankMasks[5]);
            Assert.AreEqual(0xff000000000000, BoardHelpers.RankMasks[6]);
            Assert.AreEqual(0xff00000000000000, BoardHelpers.RankMasks[7]);
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
            Assert.AreEqual(expected, BoardHelpers.GetDisplayBits(testVal));
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
            Assert.AreEqual(expected, BoardHelpers.GetDisplayBits(testVal));
        }

       

     



    }
}
