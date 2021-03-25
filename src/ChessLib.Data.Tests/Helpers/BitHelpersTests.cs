using ChessLib.Core;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Helpers;
using ChessLib.Core.Types.Interfaces;

using NUnit.Framework;

namespace ChessLib.Data.Tests.Helpers
{
    [TestFixture]
    public static class BitHelpersTests
    {
        private const string FENScandi = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2";
        private const string FENQueenAttacksd4 = "8/1k6/8/3q4/3P4/8/6K1/8 w - - 0 2";
        private const string FENQueenIsBlockedFromAttackingSquared4 = "8/1k6/3q4/3P4/3P4/8/6K1/8 w - - 0 2";


        //[Test]
        //public static void SetBit_ShouldReturnValueWithBitSet()
        //{
        //    for (ushort i = 0; i < 64; i++)
        //    {
        //        var expected = 1ul << i;
        //        var actual = BitHelpers.SetBit(0, i);
        //        Assert.AreEqual(expected, actual);
        //    }
        //}

        //[Test]
        //public static void SetBit_ShouldSetReferencesBit()
        //{
        //    for (ushort i = 0; i < 64; i++)
        //    {
        //        var expected = 1ul << i;
        //        ulong actual = 0;
        //        BitHelpers.SetBit(ref actual, i);
        //        Assert.AreEqual(expected, actual);
        //    }
        //}

        [Test]
        public static void GetSetBits_ShouldReturnEmptyArrayGivenZero()
        {
            Assert.IsEmpty((0ul).GetSetBits());
        }

        [Test]
        public static void GetSetBits_ShouldReturnASquareIndex_GivenOneSquareOfInput()
        {
            var count = 0;
            foreach (ulong sq in BoardConstants.IndividualSquares)
            {
                var idx = sq.GetSetBits();
                Assert.AreEqual(count, idx[0]);
                count++;
            }
        }

        [Test]
        public static void GetSetBits_ShouldReturnASquareIndexes_GivenManySquaresOfInput()
        {
            var expectedR2 = new ushort[] { 8, 9, 10, 11, 12, 13, 14, 15 };
            var expectedR7 = new ushort[] { 48, 49, 50, 51, 52, 53, 54, 55 };
            var rank2Value = BoardConstants.RankMasks[1];
            var rank2SetBits = rank2Value.GetSetBits();
            Assert.AreEqual(expectedR2, rank2SetBits);
            Assert.AreEqual(expectedR7, BoardConstants.RankMasks[6].GetSetBits());

        }

        [Test]
        public static void GetSetBits_ShouldReturnASquareIndexes_Given2SquaresOfInput()
        {
            ulong a = 0b1001;
            var expected = new[] { 0, 3 };
            var bitIndices = a.GetSetBits();
            Assert.AreEqual(expected, bitIndices);
        }

        [Test]
        public static void GetSetBits_ShouldReturnASquareIndexes_Given4SquaresOfInput()
        {
            ulong a = 0b1111;
            var expected = new[] { 0, 1, 2, 3 };
            var bitIndices = a.GetSetBits();
            Assert.AreEqual(expected, bitIndices);
        }


    

       

        [TestCase(FENScandi, 27, Color.White, false)]
        [TestCase(FENQueenIsBlockedFromAttackingSquared4, 27, Color.Black, false)]
        [TestCase(FENQueenAttacksd4, 27, Color.Black, true)]
        [TestCase(FENScandi, 35, Color.White, true)]
        [TestCase(FENQueenIsBlockedFromAttackingSquared4, 35, Color.Black, true)]
        public static void IsSquareAttackedByColor(string fen, int square, Color attackingColor, bool expected)
        {
            IBoard boardInfo = new Board(fen);
            var isAttacked = Bitboard.Instance.IsSquareAttackedByColor((ushort)square, attackingColor, boardInfo.Occupancy);
            Assert.AreEqual(expected, isAttacked);
        }
    }
}
