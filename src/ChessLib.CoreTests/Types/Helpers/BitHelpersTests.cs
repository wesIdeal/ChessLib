using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Helpers;
using NUnit.Framework;
// ReSharper disable PossibleInvalidOperationException

namespace ChessLib.Core.Tests.Types.Helpers
{
    public class TestCase<TExpected, TInput>
    {
        public TestCase(TExpected expectedValue, TInput input1, params object[] additionalInputs)
        {
            ExpectedValue = expectedValue;
            InputValue = input1;
            AdditionalInputs = additionalInputs;
        }

        public object[] AdditionalInputs { get; set; }

        public TExpected ExpectedValue { get; set; }
        public TInput InputValue { get; set; }

        public override string ToString()
        {

            return $"Testing {InputValue}";
        }
    }

    [TestFixture(TestOf = typeof(BitHelpers))]
    public class BitHelpersTests
    {
        [TestCaseSource(nameof(GetBitScanTestCases))]
        public static void BitScanForwardTest(TestCase<ushort, ulong> testCase)
        {
            var actual = BitHelpers.BitScanForward(testCase.InputValue);
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
            var actual = BitHelpers.GetBoardValueOfIndex(testCase.InputValue);
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
            var actual = BitHelpers.IsBitSet(testCase.InputValue, (ushort)testCase.AdditionalInputs[0]);
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
            var actual = BitHelpers.GetSetBits(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ushort[], ulong>> GetSetBitsTestCases()
        {
            foreach (var index in BoardConstants.AllSquares)
            {
                var testValue = BitHelpers.SetBit(0, index);
                yield return new TestCase<ushort[], ulong>(new[] { index }, testValue);
            }

            yield return new TestCase<ushort[], ulong>(new ushort[] { }, 0);
            var allIndexes = Enumerable.Range(0, 64).Select(x => (ushort)x).ToArray();
            yield return new TestCase<ushort[], ulong>(allIndexes, ulong.MaxValue);
        }

        [TestCaseSource(nameof(GetSetBitTest))]
        public void SetBitTest(TestCase<ulong, ushort> testCase)
        {
            var actual = BitHelpers.SetBit(0, testCase.InputValue);
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
            var actual = BitHelpers.CountSetBits(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ushort, ulong>> GetCountSetBitCases()
        {
            var r = new Random(DateTime.Now.Millisecond);
            foreach (var _ in Enumerable.Range(0, 256))
            {
                var numberOfBitsToSet = r.Next(3, 63);
                var bitsToSet = Enumerable.Range(0, numberOfBitsToSet).Select(x => r.Next(0, 63)).Select(x => (ushort)x)
                    .Distinct().ToArray();

                var testValue = bitsToSet.Aggregate(0ul, (current, bitToSet) => current.SetBit(bitToSet));

                yield return new TestCase<ushort, ulong>((ushort)bitsToSet.Count(), testValue);
            }
        }


        [TestCaseSource(nameof(GetShiftNTestCases))]
        public void ShiftNTest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftN(testCase.InputValue);
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
            var actual = BitHelpers.ShiftE(testCase.InputValue);
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
            var actual = BitHelpers.ShiftS(testCase.InputValue);
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
            var actual = BitHelpers.ShiftW(testCase.InputValue);
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
            var actual = BitHelpers.Shift2N(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShift2NTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftN(BitHelpers.ShiftN(square));
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShift2ETestCases))]
        public void Shift2ETest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.Shift2E(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShift2ETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftE(BitHelpers.ShiftE(square));
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShift2STestCases))]
        public void Shift2STest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.Shift2S(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShift2STestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftS(BitHelpers.ShiftS(square));
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShif2tWTestCases))]
        public void Shift2WTest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.Shift2W(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShif2tWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftW(BitHelpers.ShiftW(square));
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftNETestCases))]
        public void ShiftNETest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftNE(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftNETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftN(BitHelpers.ShiftE(square));
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftSETestCases))]
        public void ShiftSETest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftSE(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftSETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftS(BitHelpers.ShiftE(square));
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftSWTestCases))]
        public void ShiftSWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftSW(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftSWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftS(BitHelpers.ShiftW(square));
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftNWTestCases))]
        public void ShiftNWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftNW(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftNWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var expected = BitHelpers.ShiftN(BitHelpers.ShiftW(square));
                yield return new TestCase<ulong, ulong>(expected, square);
            }
        }

        [TestCaseSource(nameof(GetShiftNNETestCases))]
        public void ShiftNNETest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftNNE(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftNNETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = BitHelpers.ShiftN(BitHelpers.ShiftN(BitHelpers.ShiftE(value)));
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftENETestCases))]
        public void ShiftENETest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftENE(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftENETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = BitHelpers.ShiftE(BitHelpers.ShiftN(BitHelpers.ShiftE(value)));
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftESETestCases))]
        public void ShiftESETest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftESE(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftESETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = BitHelpers.ShiftE(BitHelpers.ShiftS(BitHelpers.ShiftE(value)));
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftSSETestCases))]
        public void ShiftSSETest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftSSE(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftSSETestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = BitHelpers.ShiftS(BitHelpers.ShiftS(BitHelpers.ShiftE(value)));
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }


        [TestCaseSource(nameof(GetShiftSSWTestCases))]
        public void ShiftSSWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftSSW(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftSSWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = BitHelpers.ShiftS(BitHelpers.ShiftS(BitHelpers.ShiftW(value)));
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftWSWTestCases))]
        public void ShiftWSWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftWSW(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftWSWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = BitHelpers.ShiftW(BitHelpers.ShiftS(BitHelpers.ShiftW(value)));
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftWNWTestCases))]
        public void ShiftWNWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftWNW(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftWNWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = BitHelpers.ShiftW(BitHelpers.ShiftN(BitHelpers.ShiftW(value)));
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }

        [TestCaseSource(nameof(GetShiftNNWTestCases))]
        public void ShiftNNWTest(TestCase<ulong, ulong> testCase)
        {
            var actual = BitHelpers.ShiftNNW(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }
        protected static IEnumerable<TestCase<ulong, ulong>> GetShiftNNWTestCases()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var value = square.GetBoardValueOfIndex();
                var expected = BitHelpers.ShiftN(BitHelpers.ShiftN(BitHelpers.ShiftW(value)));
                yield return new TestCase<ulong, ulong>(expected, value);
            }
        }
    }
}