using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicBitboard;
using MagicBitboard.Helpers;
using NUnit.Framework;
namespace MagicBitboard.Helpers.Tests
{
    [TestFixture]
    public class BoardHelpersTests
    {
        [Test]
        public void TestRankCompliment()
        {
            Assert.AreEqual(0, BoardHelpers.RankCompliment(7));
            Assert.AreEqual(1, BoardHelpers.RankCompliment(6));
            Assert.AreEqual(2, BoardHelpers.RankCompliment(5));
            Assert.AreEqual(3, BoardHelpers.RankCompliment(4));
            Assert.AreEqual(4, BoardHelpers.RankCompliment(3));
            Assert.AreEqual(5, BoardHelpers.RankCompliment(2));
            Assert.AreEqual(6, BoardHelpers.RankCompliment(1));
            Assert.AreEqual(7, BoardHelpers.RankCompliment(0));
        }
        [Test]
        public void InitializeFileMasks_FileMasksProperlyInitialized()
        {
            Assert.AreEqual(0x101010101010101, BoardHelpers.FileMasks[0], $"'A' File Mask not initialized properly.");
            Assert.AreEqual(0x202020202020202, BoardHelpers.FileMasks[1], $"'B' File Mask not initialized properly.");
            Assert.AreEqual(0x404040404040404, BoardHelpers.FileMasks[2], $"'C' File Mask not initialized properly.");
            Assert.AreEqual(0x808080808080808, BoardHelpers.FileMasks[3], $"'D' File Mask not initialized properly.");
            Assert.AreEqual(0x1010101010101010, BoardHelpers.FileMasks[4], $"'E' File Mask not initialized properly.");
            Assert.AreEqual(0x2020202020202020, BoardHelpers.FileMasks[5], $"'F' File Mask not initialized properly.");
            Assert.AreEqual(0x4040404040404040, BoardHelpers.FileMasks[6], $"'G' File Mask not initialized properly.");
            Assert.AreEqual(0x8080808080808080, BoardHelpers.FileMasks[7], $"'H' File Mask not initialized properly.");
        }

        [Test]
        public void InitializeRankMasks_RankMasksProperlyInitialized()
        {
            Assert.AreEqual(0xff, BoardHelpers.RankMasks[0], $"Rank 1 Mask not initialized properly.");
            Assert.AreEqual(0xff00, BoardHelpers.RankMasks[1], $"Rank 2 Mask not initialized properly.");
            Assert.AreEqual(0xff0000, BoardHelpers.RankMasks[2], $"Rank 3 Mask not initialized properly.");
            Assert.AreEqual(0xff000000, BoardHelpers.RankMasks[3], $"Rank 4 Mask not initialized properly.");
            Assert.AreEqual(0xff00000000, BoardHelpers.RankMasks[4], $"Rank 5 Mask not initialized properly.");
            Assert.AreEqual(0xff0000000000, BoardHelpers.RankMasks[5], $"Rank 6 Mask not initialized properly.");
            Assert.AreEqual(0xff000000000000, BoardHelpers.RankMasks[6], $"Rank 7 Mask not initialized properly.");
            Assert.AreEqual(0xff00000000000000, BoardHelpers.RankMasks[7], $"Rank 8 Mask not initialized properly.");
        }

    }
}
