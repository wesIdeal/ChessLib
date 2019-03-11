using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicBitboard;
using MagicBitboard.Helpers;
using NUnit.Framework;
namespace MagicBitboard.Tests
{
    [TestFixture]
    public class BoardInfoTests
    {
        public Bitboard bitBoard;
        const string fenScandi = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2";
        const string fenQueenAttacksd4 = "8/1k6/8/3q4/3P4/8/6K1/8 w - - 0 2";
        const string fenQueenIsBlockedFromAttackingd4 = "8/1k6/3q4/3P4/3P4/8/6K1/8 w - - 0 2";
        GameInfo giScandi;
        BoardInfo biScandi;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var dtStart = DateTime.Now;
            bitBoard = new Bitboard();
            var totalMs = DateTime.Now.Subtract(dtStart).TotalMilliseconds;
            Debug.WriteLine($"Bitboard made in {totalMs} ms");
        }
        [SetUp]
        public void Setup()
        {
            giScandi = new GameInfo(bitBoard, fenScandi);
            biScandi = giScandi.BoardInfo;
        }
        [Test]
        public void Should_Return_True_When_d5_Is_Attacked()
        {
            var d5 = MoveHelpers.SquareTextToIndex("d5");
            var isAttacked = biScandi.IsAttackedBy(MagicBitboard.Enums.Color.White, d5.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public void Should_Return_True_When_d5_Is_Attacked_2()
        {
            var gi = new GameInfo(bitBoard, fenQueenIsBlockedFromAttackingd4);
            var d5 = MoveHelpers.SquareTextToIndex("d5");
            var isAttacked = gi.BoardInfo.IsAttackedBy(MagicBitboard.Enums.Color.Black, d5.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public void Should_Return_False_When_d4_Is_Not_Attacked()
        {
            var d4 = MoveHelpers.SquareTextToIndex("d4");
            var isAttacked = biScandi.IsAttackedBy(MagicBitboard.Enums.Color.White, d4.Value);
            Assert.IsFalse(isAttacked);
        }

        [Test]
        public void Should_Return_False_When_d4_Is_Not_Attacked_2()
        {
            var gi = new GameInfo(bitBoard, fenQueenIsBlockedFromAttackingd4);
            var d4 = MoveHelpers.SquareTextToIndex("d4");
            var isAttacked = gi.BoardInfo.IsAttackedBy(MagicBitboard.Enums.Color.Black, d4.Value);
            Assert.IsFalse(isAttacked);
        }
    }
}
