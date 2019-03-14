using MagicBitboard;
using MagicBitboard.Helpers;
using NUnit.Framework;

namespace MagicBitboard.Helpers.Tests
{
    [TestFixture]
    public class BitHelpersTests
    {
        [Test]
        public void Should_Return_Square_Index_By_Value()
        {
            var count = 0;
            foreach (var sq in BoardHelpers.IndividialSquares)
            {
                var idx = BitHelpers.GetSetBitIndexes(sq);
                Assert.AreEqual(new ushort[] { (ushort)count }, idx);
            }
        }
        [Test]
        public void Should_Return_Square_Indexes_By_Value()
        {
            var expectedR2 = new ushort[] { 8, 9, 10, 11, 12, 13, 14, 15 };
            var expectedR7 = new ushort[] { 48, 49, 50, 51, 52, 53, 54, 55 };
            var rank2Value = BoardHelpers.RankMasks[1];
            var rank2SetBits = BitHelpers.GetSetBitIndexes(rank2Value);
            Assert.AreEqual(expectedR2, rank2SetBits);
            Assert.AreEqual(expectedR7, BitHelpers.GetSetBitIndexes(BoardHelpers.RankMasks[6]));

        }


        [Test]
        public void BitIndicies()
        {
            ulong a = 0b1001;
            var expected = new[] { 0, 3 };
            var bitIndices = BitHelpers.GetSetBits(a);
            Assert.AreEqual(expected, bitIndices);
        }

        [Test]
        public void BitIndiciesAll()
        {
            ulong a = 0b1111;
            var expected = new[] { 0, 1, 2, 3 };
            var bitIndices = BitHelpers.GetSetBits(a);
            Assert.AreEqual(expected, bitIndices);
        }

        [Test]
        public void SetBitAll()
        {
            ulong a = 0;

            for (int i = 0; i < 63; i++)
            {
                a = 0;
                ulong expectedValue = (ulong)1 << i;
                BitHelpers.SetBit(ref a, i);
                Assert.AreEqual(expectedValue, a);
                Assert.IsTrue((a & expectedValue) == expectedValue);
            }
        }


        [Test]
        public void ClearBit()
        {
            ulong a = 0xff;
            var bitIndexToClear = 1;
            BitHelpers.ClearBit(ref a, bitIndexToClear);
            Assert.AreEqual(0xfd, a);
        }

        [Test]
        public void IsBitSet()
        {
            ulong a = 0;
            var bitIndexToSet = 32;
            BitHelpers.SetBit(ref a, bitIndexToSet);
            Assert.IsTrue(BitHelpers.IsBitSet(a, bitIndexToSet));
        }
    }
}
