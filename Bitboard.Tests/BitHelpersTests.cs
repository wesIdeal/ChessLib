using MagicBitboard;
using MagicBitboard.Helpers;
using NUnit.Framework;

namespace Bitboard.Tests
{
    [TestFixture]
    public class BitHelpersTests
    {
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
