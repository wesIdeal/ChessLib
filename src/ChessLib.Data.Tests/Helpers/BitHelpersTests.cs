using NUnit.Framework;

namespace ChessLib.Data.Helpers.Tests
{
    [TestFixture]
    public class BitHelpersTests
    {
        [Test]
        public void GetSetBits_ShouldReturnASquareIndex_GivenOneSquareOfInput()
        {
            var count = 0;
            foreach (var sq in BoardHelpers.IndividualSquares)
            {
                var idx = BitHelpers.GetSetBits(sq);
                Assert.AreEqual(count, idx[0]);
                count++;
            }
        }
        [Test]
        public void GetSetBits_ShouldReturnASquareIndexes_GivenManySquaresOfInput()
        {
            var expectedR2 = new ushort[] { 8, 9, 10, 11, 12, 13, 14, 15 };
            var expectedR7 = new ushort[] { 48, 49, 50, 51, 52, 53, 54, 55 };
            var rank2Value = BoardHelpers.RankMasks[1];
            var rank2SetBits = BitHelpers.GetSetBits(rank2Value);
            Assert.AreEqual(expectedR2, rank2SetBits);
            Assert.AreEqual(expectedR7, BitHelpers.GetSetBits(BoardHelpers.RankMasks[6]));

        }


        [Test]
        public void GetSetBits_ShouldReturnASquareIndexes_Given2SquaresOfInput()
        {
            ulong a = 0b1001;
            var expected = new[] { 0, 3 };
            var bitIndices = BitHelpers.GetSetBits(a);
            Assert.AreEqual(expected, bitIndices);
        }

        [Test]
        public void GetSetBits_ShouldReturnASquareIndexes_Given4SquaresOfInput()
        {
            ulong a = 0b1111;
            var expected = new[] { 0, 1, 2, 3 };
            var bitIndices = BitHelpers.GetSetBits(a);
            Assert.AreEqual(expected, bitIndices);
        }

        [Test]
        public void SetBit_ShouldSetBit_GivenAnySquareInRange()
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
        public void ClearBit_ShouldSetBitIndexToZero()
        {
            ulong a = 0xff;
            var bitIndexToClear = 1;
            BitHelpers.ClearBit(ref a, bitIndexToClear);
            Assert.AreEqual(0xfd, a);
        }

        [Test]
        public void IsBitSet_ShouldReturnTrue_InputBitIndexIsSet()
        {
            ulong a = 0;
            ushort bitIndexToSet = 32;
            BitHelpers.SetBit(ref a, bitIndexToSet);
            Assert.IsTrue(BitHelpers.IsBitSet(a, bitIndexToSet));
        }

        [Test]
        public void IsBitSet_ShouldReturnFalse_InputBitIndexIsNotSet()
        {
            ulong a = 0;
            ushort bitIndexToSet = 32;
            BitHelpers.SetBit(ref a, bitIndexToSet);
            a = ~a;
            Assert.IsFalse(BitHelpers.IsBitSet(a, bitIndexToSet));
        }
    }
}
