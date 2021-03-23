using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Types.Enums;

namespace ChessLib.MagicBitboard.Tests.Bitwise
{
    using Constants = ChessLib.MagicBitboard.Bitwise.BoardConstants;
    using ServiceUnderTest = ChessLib.MagicBitboard.Bitwise.MovingPieceService;
    public class ShiftTestCase
    {
        public ShiftTestCase(ulong value, ulong expected, Func<ulong, ulong> method)
        {
            Value = value;
            Expected = expected;
            Method = method;
        }

        public Func<ulong, ulong> Method { get; set; }
        public ulong Expected { get; set; }
        public ulong Value { get; set; }
    }

    [TestFixture]
    public class MovingPieceService
    {
        #region Shifts

        private static IEnumerable<ShiftTestCase> GetShiftTestCases()
        {
            //N
            yield return new ShiftTestCase(1ul << 48, 1ul << 56, ServiceUnderTest.ShiftN);
            yield return new ShiftTestCase(1ul << 56, 0, ServiceUnderTest.ShiftN);
            //S
            yield return new ShiftTestCase(1ul << 8, 1ul, ServiceUnderTest.ShiftS);
            yield return new ShiftTestCase(1ul << 7, 0, ServiceUnderTest.ShiftS);
            //W
            yield return new ShiftTestCase(1ul << 25, 1ul << 24, ServiceUnderTest.ShiftW);
            yield return new ShiftTestCase(1ul << 24, 0, ServiceUnderTest.ShiftW);
            //E
            yield return new ShiftTestCase(1ul << 6, 1ul << 7, ServiceUnderTest.ShiftE);
            yield return new ShiftTestCase(1ul << 7, 0, ServiceUnderTest.ShiftE);

            //2W
            yield return new ShiftTestCase(1ul << 26, 1ul << 24, ServiceUnderTest.Shift2W);
            yield return new ShiftTestCase(1ul << 25, 0, ServiceUnderTest.Shift2W);

            //2E
            yield return new ShiftTestCase(1ul << 5, 1ul << 7, ServiceUnderTest.Shift2E);
            yield return new ShiftTestCase(1ul << 6, 0, ServiceUnderTest.Shift2E);

            //2N
            yield return new ShiftTestCase(1ul << 40, 1ul << 56, ServiceUnderTest.Shift2N);
            yield return new ShiftTestCase(1ul << 48, 0, ServiceUnderTest.Shift2N);

            //2S
            yield return new ShiftTestCase(1ul << 16, 1ul, ServiceUnderTest.Shift2S);
            yield return new ShiftTestCase(1ul << 15, 0, ServiceUnderTest.Shift2S);


            //NE
            yield return new ShiftTestCase(1ul << 54, 1ul << 63, ServiceUnderTest.ShiftNE);
            yield return new ShiftTestCase(1ul << 55, 0, ServiceUnderTest.ShiftNE);


            //NW
            yield return new ShiftTestCase(1ul << 49, 1ul << 56, ServiceUnderTest.ShiftNW);
            yield return new ShiftTestCase(1ul << 48, 0, ServiceUnderTest.ShiftNW);

            //SE
            yield return new ShiftTestCase(1ul << 14, 1ul << 7, ServiceUnderTest.ShiftSE);
            yield return new ShiftTestCase(1ul << 15, 0, ServiceUnderTest.ShiftSE);

            //SW
            yield return new ShiftTestCase(1ul << 9, 1ul << 0, ServiceUnderTest.ShiftSW);
            yield return new ShiftTestCase(1ul << 24, 0, ServiceUnderTest.ShiftSW);

            //NNE
            yield return new ShiftTestCase(1ul << 41, 1ul << 58, ServiceUnderTest.ShiftNNE);
            yield return new ShiftTestCase(1ul << 50, 0, ServiceUnderTest.ShiftNNE);


            //ENE
            yield return new ShiftTestCase(1ul << 28, 1ul << 38, ServiceUnderTest.ShiftENE);
            yield return new ShiftTestCase(1ul << 30, 0, ServiceUnderTest.ShiftENE);


            //NNW
            yield return new ShiftTestCase(1ul << 41, 1ul << 56, ServiceUnderTest.ShiftNNW);
            yield return new ShiftTestCase(1ul << 50, 0, ServiceUnderTest.ShiftNNW);

            //WNW
            yield return new ShiftTestCase(1ul << 28, 1ul << 34, ServiceUnderTest.ShiftWNW);
            yield return new ShiftTestCase(1ul << 25, 0, ServiceUnderTest.ShiftWNW);


            //SSE
            yield return new ShiftTestCase(1ul << 18, 1ul << 3, ServiceUnderTest.ShiftSSE);
            yield return new ShiftTestCase(1ul << 9, 0, ServiceUnderTest.ShiftSSE);


            //ESE
            yield return new ShiftTestCase(1ul << 28, 1ul << 22, ServiceUnderTest.ShiftESE);
            yield return new ShiftTestCase(1ul << 14, 0, ServiceUnderTest.ShiftESE);

            //SSW
            yield return new ShiftTestCase(1ul << 19, 1ul << 2, ServiceUnderTest.ShiftSSW);
            yield return new ShiftTestCase(1ul << 16, 0, ServiceUnderTest.ShiftSSW);

            //WSW
            yield return new ShiftTestCase(1ul << 22, 1ul << 12, ServiceUnderTest.ShiftWSW);
            yield return new ShiftTestCase(1ul << 17, 0, ServiceUnderTest.ShiftWSW);

        }

        [TestCaseSource(nameof(GetShiftTestCases))]
        [Test]
        public static void TestShift(ShiftTestCase test)
        {
            var startIndex = ServiceUnderTest.GetSetBits(test.Value)[0];
            var expectedIndex = test.Expected == 0 ? 0 : ServiceUnderTest.GetSetBits(test.Expected)[0];
            var actual = test.Method(test.Value);
            var actualIndex = test.Expected == 0 ? 0 : ServiceUnderTest.GetSetBits(actual)[0];
            var methodName = test.Method.GetMethodInfo().Name;

            Assert.AreEqual(test.Expected, actual, $"Shift method was {methodName}. Expected piece to be at {expectedIndex} starting from {startIndex}, but was at {actualIndex}.");
        }





        #endregion Shifts

        [Test]
        public void TestGetBoardValueOfIndex([Range(0, 63)] int range)
        {
            var sq = (ushort)range;
            ulong expected = 1ul << range;
            var actual = ServiceUnderTest.GetBoardValueOfIndex(sq);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestSetBit([Range(1, 8)] int bitPosition)
        {
            var expected = 1ul << bitPosition;
            var actual = ServiceUnderTest.SetBit(0, (ushort)bitPosition);
            Assert.AreEqual(expected, actual);
        }

        private static IEnumerable<ulong> GetPermutationsCase()
        {
            yield return 0;
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 8;
            yield return 9;
            yield return 10;
            yield return 11;
        }

        [TestCaseSource(nameof(GetPermutationsCase))]
        public void TestGetPermutations(ulong permutation)
        {
            var actual = ServiceUnderTest.GetAllPermutationsOfSetBits(ServiceUnderTest.GetSetBits(11), 0, 0).Distinct().OrderBy(x => x).ToArray();
            Assert.Contains(permutation, actual);
        }

        [Test]
        public void TestPawnStartRank_Black()
        {
            Assert.AreEqual(Constants.Rank7, ServiceUnderTest.GetPawnStartRankMask(Color.Black));
        }

        [Test]
        public void TestPawnStartRank_White()
        {
            Assert.AreEqual(Constants.Rank2, ServiceUnderTest.GetPawnStartRankMask(Color.White));
        }


    }
}
