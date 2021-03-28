using System.Collections.Generic;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Core.Tests.MagicBitboard
{
    [TestFixture(Category = "Bitboard")]
    public class BitboardTests
    {
        [TestCaseSource(nameof(GetPiecesAttackingSquareTestCases_SpecificColor))]
        [TestCaseSource(nameof(GetPiecesAttackingSquareTestCases_AllColors))]
        public void PiecesAttackingSquareTest(TestCase<ulong, Board> testCase)
        {
            var square = (ushort)testCase.AdditionalInputs[0];
            var attackerColor = (Color?)testCase.AdditionalInputs[1];
            var actual =
                Bitboard.Instance.PiecesAttackingSquareByColor(testCase.InputValue.Occupancy,
                    square, attackerColor);
            Assert.AreEqual(testCase.ExpectedValue, actual);
        }

        protected static IEnumerable<TestCase<ulong, Board>> GetPiecesAttackingSquareTestCases_SpecificColor()
        {
            var description = "Specific Color";
            yield return new TestCase<ulong, Board>(0x4000ul,
                new Board("2r5/3r2k1/p3b3/1p3p2/2pPpP2/P1N1P3/1P4RP/2R3K1 b - - 1 34"),
                $"{description} White Rook attacks (Black attacks from Queenside)", (ushort)54, Color.White);
            yield return new TestCase<ulong, Board>(0x800000000, new Board("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} White Pawn on d4 attacks the c6 square.", (ushort)42, Color.White);
            yield return new TestCase<ulong, Board>(0x200000000, new Board("4k3/8/8/1p6/8/3P4/8/4K3 w - - 0 1"),
                $"{description} Black Pawn on b5 attacks the c4 square.", (ushort)26, Color.Black);
            yield return new TestCase<ulong, Board>(0x80000, new Board("4k3/8/8/1p6/8/3P4/8/4K3 w - - 0 1"),
                $"{description} White Pawn on d3 attacks the c4 square.", (ushort)26, Color.White);
            yield return new TestCase<ulong, Board>(0x00, new Board("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} Nothing attacking d6", (ushort)43, Color.White);
            yield return new TestCase<ulong, Board>(0x400000000, new Board("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} Black Pawn attacks d4", (ushort)27, Color.Black);
            yield return new TestCase<ulong, Board>(0x8000140000000000, new Board("4k2Q/8/2B1R3/8/8/8/8/4K3 b - - 0 1"),
                $"{description} 3 White Pieces attack Black King", (ushort)60, Color.White);
            yield return new TestCase<ulong, Board>(0x140000000000, new Board("4k2q/8/2B1R3/8/8/8/8/4K3 b - - 0 1"),
                $"{description} Two White pieces attack the Black Queen, one Black Queen flanks from the Kingside.",
                (ushort)60,
                Color.White);
        }

        protected static IEnumerable<TestCase<ulong, Board>> GetPiecesAttackingSquareTestCases_AllColors()
        {
            var description = "All Colors";
            yield return new TestCase<ulong, Board>(0x8000000004000ul,
                new Board("2r5/3r2k1/p3b3/1p3p2/2pPpP2/P1N1P3/1P4RP/2R3K1 b - - 1 34"),
                $"{description} Black and White Rook Attack g7", (ushort)54, null);
            yield return new TestCase<ulong, Board>(0x800000000, new Board("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} White Pawn on d4 attacks the c6 square.", (ushort)42, null);
            yield return new TestCase<ulong, Board>(0x00, new Board("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} Nothing attacking d6", (ushort)43, null);
            yield return new TestCase<ulong, Board>(0x200080000, new Board("4k3/8/8/1p6/8/3P4/8/4K3 w - - 0 1"),
                $"{description} White Pawn on d3 / Black Pawn on b5 both attack the c4 square.", (ushort)26, null);
            yield return new TestCase<ulong, Board>(0x400000000, new Board("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} Black Pawn attacks d4", (ushort)27, null);
            yield return new TestCase<ulong, Board>(0x8000140000000000, new Board("4k2Q/8/2B1R3/8/8/8/8/4K3 b - - 0 1"),
                $"{description} 3 White Pieces attack Black King", (ushort)60, null);
            yield return new TestCase<ulong, Board>(0x8000140000000000, new Board("4k2q/8/2B1R3/8/8/8/8/4K3 b - - 0 1"),
                $"{description} Two White pieces attack the Black Queen, one Black Queen flanks from the Kingside.",
                (ushort)60,
                null);
        }
    }
}