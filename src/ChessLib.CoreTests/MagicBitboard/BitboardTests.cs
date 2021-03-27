using System.Collections.Generic;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Tests.Helpers;
using NUnit.Framework;

namespace ChessLib.Core.Tests.MagicBitboard
{
    [TestFixture()]
    public class BitboardTests
    {
        [TestCaseSource(nameof(GetPiecesAttackingSquareTestCases))]
        public void PiecesAttackingSquareTest(TestCase<ulong, Board> testCase)
        {
            ushort square = (ushort)testCase.AdditionalInputs[0];
            var actual =
                Bitboard.Instance.PiecesAttackingSquare(testCase.InputValue.Occupancy,
                    square);
            Assert.AreEqual(testCase.ExpectedValue, actual);
        }
        protected static IEnumerable<TestCase<ulong, Board>> GetPiecesAttackingSquareTestCases()
        {
            yield return new TestCase<ulong, Board>(0x4000ul,
                new Board("2r5/3r2k1/p3b3/1p3p2/2pPpP2/P1N1P3/1P4RP/2R3K1 b - - 1 34"), "", (ushort)54);
        }
    }
}