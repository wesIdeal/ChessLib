using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using NUnit.Framework;

// ReSharper disable PossibleInvalidOperationException

namespace ChessLib.Core.Tests.Helpers
{
    public class TestCase<TExpected, TInput>
    {
        public TestCase(TExpected expectedValue, TInput testMethodInput, params object[] additionalInputs)
        {
            ExpectedValue = expectedValue;
            TestMethodInputValue = testMethodInput;
            AdditionalInputs = additionalInputs;
        }

        public TestCase(TExpected expectedValue, TInput testMethodInput, string description,
            params object[] additionalInputs)
        {
            ExpectedValue = expectedValue;
            TestMethodInputValue = testMethodInput;
            AdditionalInputs = additionalInputs;
            Description = description;
        }

        public string Description { get; set; }

        public object[] AdditionalInputs { get; set; }

        public TExpected ExpectedValue { get; set; }
        public TInput TestMethodInputValue { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Description))
            {
                return Description;
            }

            return $"Testing {TestMethodInputValue}";
        }
    }

    [TestFixture(TestOf = typeof(BitHelpers))]
    public class BitHelpersTests
    {
        [TestCaseSource(nameof(GetBitScanTestCases))]
        public static void BitScanForwardTest(TestCase<ushort, ulong> testCase)
        {
            var actual = BitHelpers.BitScanForward(testCase.TestMethodInputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, $"{testCase} Actual was {actual}.");
        }

        protected static IEnumerable<TestCase<ushort, ulong>> GetBitScanTestCases()
        {
            for (ushort index = 0; index < 64; index++)
            {
                var testValue = 0ul;
                for (var antiIndex = index; antiIndex < 64; antiIndex++)
                {
                    testValue = testValue.SetBit(antiIndex);
                }

                yield return new TestCase<ushort, ulong>(index, testValue);
            }
        }


        [TestCaseSource(nameof(GetValueOfIndexTestCases))]
        public void GetBoardValueOfIndexTest(TestCase<ulong, ushort> testCase)
        {
            var actual = testCase.TestMethodInputValue.GetBoardValueOfIndex();
            Assert.AreEqual(testCase.ExpectedValue, actual, $"{testCase} Actual was {actual}.");
        }

        protected static IEnumerable<TestCase<ulong, ushort>> GetValueOfIndexTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = 1ul << square;
                yield return new TestCase<ulong, ushort>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetIsBitSetTestCases))]
        public void IsBitSetTest(TestCase<bool, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.IsBitSet((ushort) testCase.AdditionalInputs[0]);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<bool, ulong>> GetIsBitSetTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var setBit = BitHelpers.SetBit(0, square);
                yield return new TestCase<bool, ulong>(false, ~setBit, square);
                yield return new TestCase<bool, ulong>(true, setBit, square);
                yield return new TestCase<bool, ulong>(true, ulong.MaxValue, square);
                yield return new TestCase<bool, ulong>(false, 0, square);
            }
        }

        [TestCaseSource(nameof(GetSetBitsTestCases))]
        public void GetSetBitsTest(TestCase<ushort[], ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.GetSetBits();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ushort[], ulong>> GetSetBitsTestCases()
        {
            foreach (var index in BoardConstants.AllSquares)
            {
                var testValue = BitHelpers.SetBit(0, index);
                yield return new TestCase<ushort[], ulong>(new[] {index}, testValue);
            }

            yield return new TestCase<ushort[], ulong>(new ushort[] { }, 0);
            var allIndexes = Enumerable.Range(0, 64).Select(x => (ushort) x).ToArray();
            yield return new TestCase<ushort[], ulong>(allIndexes, ulong.MaxValue);
        }

        [TestCaseSource(nameof(GetSetBitTest))]
        public void SetBitTest(TestCase<ulong, ushort> testCase)
        {
            var actual = BitHelpers.SetBit(0, testCase.TestMethodInputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ushort>> GetSetBitTest()
        {
            foreach (var index in BoardConstants.AllSquares)
            {
                yield return new TestCase<ulong, ushort>(index.GetBoardValueOfIndex(), index);
            }
        }


        [TestCaseSource(nameof(GetCountSetBitCases))]
        public void CountSetBitsTest(TestCase<ushort, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.CountSetBits();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ushort, ulong>> GetCountSetBitCases()
        {
            var r = new Random(DateTime.Now.Millisecond);
            foreach (var _ in Enumerable.Range(0, 256))
            {
                var numberOfBitsToSet = r.Next(3, 63);
                var bitsToSet = Enumerable.Range(0, numberOfBitsToSet).Select(x => r.Next(0, 63))
                    .Select(x => (ushort) x)
                    .Distinct().ToArray();

                var testValue = bitsToSet.Aggregate(0ul, (current, bitToSet) => current.SetBit(bitToSet));

                yield return new TestCase<ushort, ulong>((ushort) bitsToSet.Count(), testValue);
            }
        }


        [TestCaseSource(nameof(GetShiftNTestCases))]
        public void ShiftNTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftN();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftNTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                ulong expected;
                if ((value & BoardConstants.Rank8) == value)
                {
                    expected = 0;
                }
                else
                {
                    expected = value << 8;
                }

                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftETestCases))]
        public void ShiftETest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftE();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                ulong expected;
                if ((value & BoardConstants.HFile) == value)
                {
                    expected = 0;
                }
                else
                {
                    expected = value << 1;
                }

                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftSTestCases))]
        public void ShiftSTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftS();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftSTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                ulong expected;
                if ((value & BoardConstants.Rank1) == value)
                {
                    expected = 0;
                }
                else
                {
                    expected = value >> 8;
                }

                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }


        [TestCaseSource(nameof(GetShiftWTestCases))]
        public void ShiftWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftW();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                ulong expected;
                if ((value & BoardConstants.AFile) == value)
                {
                    expected = 0;
                }
                else
                {
                    expected = value >> 1;
                }

                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }


        [TestCaseSource(nameof(GetShift2NTestCases))]
        public void Shift2NTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.Shift2N();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShift2NTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftN(square).ShiftN();
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShift2ETestCases))]
        public void Shift2ETest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.Shift2E();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShift2ETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftE(square).ShiftE();
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShift2STestCases))]
        public void Shift2STest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.Shift2S();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShift2STestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftS(square).ShiftS();
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShif2tWTestCases))]
        public void Shift2WTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.Shift2W();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShif2tWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftW(square).ShiftW();
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftNETestCases))]
        public void ShiftNETest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftNE();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftNETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftE(square).ShiftN();
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftSETestCases))]
        public void ShiftSETest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftSE();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftSETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftE(square).ShiftS();
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftSWTestCases))]
        public void ShiftSWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftSW();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftSWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftW(square).ShiftS();
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftNWTestCases))]
        public void ShiftNWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftNW();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftNWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftW(square).ShiftN();
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftNNETestCases))]
        public void ShiftNNETest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftNNE();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftNNETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = value.ShiftE().ShiftN().ShiftN();
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftENETestCases))]
        public void ShiftENETest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftENE();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftENETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = value.ShiftE().ShiftN().ShiftE();
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftESETestCases))]
        public void ShiftESETest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftESE();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftESETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = value.ShiftE().ShiftS().ShiftE();
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftSSETestCases))]
        public void ShiftSSETest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftSSE();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftSSETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = value.ShiftE().ShiftS().ShiftS();
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }


        [TestCaseSource(nameof(GetShiftSSWTestCases))]
        public void ShiftSSWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftSSW();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftSSWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = value.ShiftW().ShiftS().ShiftS();
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftWSWTestCases))]
        public void ShiftWSWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftWSW();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftWSWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = value.ShiftW().ShiftS().ShiftW();
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftWNWTestCases))]
        public void ShiftWNWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftWNW();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftWNWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = value.ShiftW().ShiftN().ShiftW();
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftNNWTestCases))]
        public void ShiftNNWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = testCase.TestMethodInputValue.ShiftNNW();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftNNWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = value.ShiftW().ShiftN().ShiftN();
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }
    }
}