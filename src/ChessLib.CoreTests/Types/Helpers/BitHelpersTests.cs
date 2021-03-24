using NUnit.Framework;
using ChessLib.Core.Types.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Core.Types.Helpers.Tests
{
    public class BitHelpersTestCase
    {
        public ushort TestedValue { get; set; }
        public ulong ExpectedValue { get; set; }
    }
    [TestFixture()]
    public class BitHelpersTests
    {
        [TestCaseSource(nameof(GetBitScanTestCases))]
        public static void BitScanForwardTest()
        {
            for (ushort expected = 0; expected < 64; expected++)
            {
                var testValue = BitHelpers.SetBit(0, expected) | 0x8000000000000000; //always set the MSB for testing
                Assert.AreEqual(expected, BitHelpers.BitScanForward(testValue));
            }
        }

        protected static IEnumerable<BitHelpersTestCase> GetBitScanTestCases()
        {
            for (ushort index = 0; index < 64; index++)
            {
                var testValue = BitHelpers.SetBit(0, index);
                yield return new BitHelpersTestCase()
                {
                    ExpectedValue = testValue | 0x8000000000000000,
                    TestedValue = index
                };
            }
        }

        [Test()]
        public void GetBoardValueOfIndexTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void FlipVerticallyTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void FlipIndexVerticallyTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ToBoardValueTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsIndexOnFileTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsIndexOnRankTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsBitSetTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetSetBitsTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetBitTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetBitTest1()
        {
            Assert.Fail();
        }

        [Test()]
        public void ClearBitTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ClearBitTest1()
        {
            Assert.Fail();
        }

        [Test()]
        public void CountSetBitsTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftNTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftSTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftWTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Shift2NTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Shift2ETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Shift2STest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Shift2WTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftNETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftSETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftSWTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftNWTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftNNETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftENETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftESETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftSSETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftSSWTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftWSWTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftWNWTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ShiftNNWTest()
        {
            Assert.Fail();
        }
    }
}