using NUnit.Framework;

namespace ChessLib.Data.Helpers.Tests
{
    [TestFixture]
    public static class BitHelpersTests
    {
        [Test]
        public static void BitScanForward_ShouldReturnIndexOfBit_WhereBitIs1()
        {
            for (ushort expected = 0; expected < 64; expected++)
            {
                var testValue = BitHelpers.SetBit(0, expected) | 0x8000000000000000; //always set the MSB for testing
                Assert.AreEqual(expected, BitHelpers.BitScanForward(testValue));
            }
        }
        [Test]
        public static void SetBit_ShouldReturnValueWithBitSet()
        {
            for (ushort i = 0; i < 64; i++)
            {
                var expected = 1ul << i;
                var actual = BitHelpers.SetBit(0, i);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public static void SetBit_ShouldSetReferencesBit()
        {
            for (ushort i = 0; i < 64; i++)
            {
                var expected = 1ul << i;
                ulong actual = 0;
                BitHelpers.SetBit(ref actual, i);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public static void GetSetBits_ShouldReturnASquareIndex_GivenOneSquareOfInput()
        {
            var count = 0;
            foreach (ulong sq in BoardHelpers.IndividualSquares)
            {
                var idx = BitHelpers.GetSetBits(sq);
                Assert.AreEqual(count, idx[0]);
                count++;
            }
        }

        [Test]
        public static void GetSetBits_ShouldReturnASquareIndexes_GivenManySquaresOfInput()
        {
            var expectedR2 = new ushort[] { 8, 9, 10, 11, 12, 13, 14, 15 };
            var expectedR7 = new ushort[] { 48, 49, 50, 51, 52, 53, 54, 55 };
            var rank2Value = BoardHelpers.RankMasks[1];
            var rank2SetBits = BitHelpers.GetSetBits(rank2Value);
            Assert.AreEqual(expectedR2, rank2SetBits);
            Assert.AreEqual(expectedR7, BitHelpers.GetSetBits(BoardHelpers.RankMasks[6]));

        }

        [Test]
        public static void GetSetBits_ShouldReturnASquareIndexes_Given2SquaresOfInput()
        {
            ulong a = 0b1001;
            var expected = new[] { 0, 3 };
            var bitIndices = BitHelpers.GetSetBits(a);
            Assert.AreEqual(expected, bitIndices);
        }

        [Test]
        public static void GetSetBits_ShouldReturnASquareIndexes_Given4SquaresOfInput()
        {
            ulong a = 0b1111;
            var expected = new[] { 0, 1, 2, 3 };
            var bitIndices = BitHelpers.GetSetBits(a);
            Assert.AreEqual(expected, bitIndices);
        }


        [Test]
        public static void ClearBit_ShouldSetRefBitIndexToZero()
        {
            ulong a = 0xff;
            var bitIndexToClear = 1;
            BitHelpers.ClearBit(ref a, bitIndexToClear);
            Assert.AreEqual(0xfd, a);
        }


        [Test]
        public static void ClearBit_ShouldSetBitIndexToZero()
        {
            ulong a = 0xff;
            var bitIndexToClear = 1;
            var actual = BitHelpers.ClearBit(a, bitIndexToClear);
            Assert.AreEqual(0xfd, actual);
        }

        [Test]
        public static void IsBitSet_ShouldReturnTrue_InputBitIndexIsSet()
        {
            ulong a = 0;
            ushort bitIndexToSet = 32;
            BitHelpers.SetBit(ref a, bitIndexToSet);
            Assert.IsTrue(BitHelpers.IsBitSet(a, bitIndexToSet));
        }

        [Test]
        public static void IsBitSet_ShouldReturnFalse_InputBitIndexIsNotSet()
        {
            ulong a = 0;
            ushort bitIndexToSet = 32;
            BitHelpers.SetBit(ref a, bitIndexToSet);
            a = ~a;
            Assert.IsFalse(BitHelpers.IsBitSet(a, bitIndexToSet));
        }
    }
}
