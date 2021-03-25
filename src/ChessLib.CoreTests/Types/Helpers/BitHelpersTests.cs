using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Helpers;
using NUnit.Framework;
// ReSharper disable PossibleInvalidOperationException

namespace ChessLib.Core.Tests.Types.Helpers
{
    public class BitHelpersTestCase
    {
        public BitHelpersTestCase(ulong testedValue, bool expected)
        {
            ExpectedBool = expected;
            TestedValue = testedValue;
        }

        public BitHelpersTestCase(ulong testedValue, ulong? expectedValue)
        {
            TestedValue = testedValue;
            ExpectedValue = expectedValue;
        }

        public ulong TestedValue2 { get; set; }
        public ulong TestedValue { get; set; }
        public ulong? ExpectedValue { get; set; }

        public bool ExpectedBool { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Description}{Environment.NewLine}Expected {ExpectedValue} when testing {TestedValue}.";
        }
    }

    [TestFixture]
    public class BitHelpersTests
    {
        [TestCaseSource(nameof(GetBitScanTestCases))]
        public static void BitScanForwardTest(BitHelpersTestCase testCase)
        {
            var actual = BitHelpers.BitScanForward(testCase.TestedValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, $"{testCase} Actual was {actual}.");
        }

        protected static IEnumerable<BitHelpersTestCase> GetBitScanTestCases()
        {
            for (ushort index = 0; index < 64; index++)
            {
                var testValue = 0ul;
                for (var antiIndex = index; antiIndex < 64; antiIndex++)
                {
                    testValue = testValue.SetBit(antiIndex);
                }

                yield return new BitHelpersTestCase(testValue, index);
            }
        }


        [TestCaseSource(nameof(GetValueOfIndexTestCases))]
        public void GetBoardValueOfIndexTest(BitHelpersTestCase testCase)
        {
            var actual = ((ushort) testCase.TestedValue).GetBoardValueOfIndex();
            Assert.AreEqual(testCase.ExpectedValue, actual, $"{testCase} Actual was {actual}.");
        }

        protected static IEnumerable<BitHelpersTestCase> GetValueOfIndexTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = 1ul << square;
                yield return new BitHelpersTestCase(square, expected);
            }
        }

        [TestCaseSource(nameof(GetIsBitSetTestCases))]
        public void IsBitSetTest(BitHelpersTestCase testCase)
        {
            var actual = testCase.TestedValue.IsBitSet((ushort) testCase.TestedValue2);
            Assert.AreEqual(testCase.ExpectedBool, actual, testCase.ToString());
        }

        protected static IEnumerable<BitHelpersTestCase> GetIsBitSetTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var setBit = BitHelpers.SetBit(0, square);
                yield return new BitHelpersTestCase(~setBit, false) {TestedValue2 = square};
                yield return new BitHelpersTestCase(setBit, true) {TestedValue2 = square};
                yield return new BitHelpersTestCase(ulong.MaxValue, true) {TestedValue2 = square};
                yield return new BitHelpersTestCase(0, false) {TestedValue2 = square};
            }
        }

        [TestCaseSource(nameof(GetSetBitsTestCases))]
        public void GetSetBitsTest(BitHelpersTestCase testCase)
        {
            var actual = testCase.TestedValue.GetSetBits();
            var testedValue = testCase.TestedValue;
            if (testedValue != 0 && testedValue != ulong.MaxValue)
            {
                Assert.AreEqual(testCase.ExpectedValue.Value, actual.Single());
            }
            else
            {
                if (testedValue == 0)
                {
                    Assert.IsEmpty(actual);
                }
                else //(testedValue == ulong.MaxValue)
                {
                    Assert.AreEqual(64, actual.Length);
                }
            }
        }

        protected static IEnumerable<BitHelpersTestCase> GetSetBitsTestCases()
        {
            foreach (var index in BoardConstants.AllSquares)
            {
                var testValue = BitHelpers.SetBit(0, index);
                yield return new BitHelpersTestCase(testValue, index);
            }

            yield return new BitHelpersTestCase(0, null);
            yield return new BitHelpersTestCase(ulong.MaxValue, ulong.MaxValue);
        }

        [TestCaseSource(nameof(GetSetBitTest))]
        public void SetBitTest(BitHelpersTestCase testCase)
        {
            var actual = BitHelpers.SetBit(0, (ushort) testCase.TestedValue);
            Assert.AreEqual(testCase.ExpectedValue.Value, actual);
        }

        protected static IEnumerable<BitHelpersTestCase> GetSetBitTest()
        {
            foreach (var index in BoardConstants.AllSquares)
            {
                yield return new BitHelpersTestCase(index, index.GetBoardValueOfIndex());
            }
        }

        [Test]
        public void ClearBitTest()
        {
            Assert.Fail();
        }


        [Test]
        public void ClearBitTest1()
        {
            Assert.Fail();
        }

        [TestCaseSource(nameof(GetCountSetBitCases))]
        public void CountSetBitsTest(BitHelpersTestCase testCase)
        {
            var actual = testCase.TestedValue.CountSetBits();
            Assert.AreEqual(testCase.ExpectedValue.Value, actual);
        }

        protected static IEnumerable<BitHelpersTestCase> GetCountSetBitCases()
        {
            var r = new Random(DateTime.Now.Millisecond);
            foreach (var number in Enumerable.Range(0, 256))
            {
                var numberOfBitsToSet = r.Next(3, 63);
                var bitsToSet = Enumerable.Range(0, numberOfBitsToSet).Select(x => r.Next(0, 63)).Select(x => (ushort) x)
                    .Distinct().ToArray();
               
                var testValue = bitsToSet.Aggregate(0ul, (current, bitToSet) => current.SetBit(bitToSet));

                yield return new BitHelpersTestCase(testValue, (ulong?) bitsToSet.Count());
            }
        }


        [Test]
        public void ShiftNTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftETest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftSTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftWTest()
        {
            Assert.Fail();
        }

        [Test]
        public void Shift2NTest()
        {
            Assert.Fail();
        }

        [Test]
        public void Shift2ETest()
        {
            Assert.Fail();
        }

        [Test]
        public void Shift2STest()
        {
            Assert.Fail();
        }

        [Test]
        public void Shift2WTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftNETest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftSETest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftSWTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftNWTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftNNETest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftENETest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftESETest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftSSETest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftSSWTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftWSWTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftWNWTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ShiftNNWTest()
        {
            Assert.Fail();
        }
    }
}