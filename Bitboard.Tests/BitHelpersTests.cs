using MagicBitboard;
using NUnit.Framework;

namespace ChessLib.Tests
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
        public void SetBit()
        {
            ulong a = 0;
            var bitIndexToSet = 32;
            var expectedBitComparisson = (ulong)1 << bitIndexToSet;
            BitHelpers.SetBit(ref a, bitIndexToSet);
            Assert.AreEqual(expectedBitComparisson, a);
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
