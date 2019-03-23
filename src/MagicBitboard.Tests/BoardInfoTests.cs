using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicBitboard;
using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using NUnit.Framework;
using static MagicBitboard.Helpers.MoveHelpers;

namespace MagicBitboard.Helpers.Tests
{
    [TestFixture]
    public class BoardInfoTests
    {

        const string fenScandi = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2";
        const string fenQueenAttacksd4 = "8/1k6/8/3q4/3P4/8/6K1/8 w - - 0 2";
        const string fenQueenIsBlockedFromAttackingd4 = "8/1k6/3q4/3P4/3P4/8/6K1/8 w - - 0 2";
        GameInfo giScandi;
        BoardInfo biScandi;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var dtStart = DateTime.Now;
            var totalMs = DateTime.Now.Subtract(dtStart).TotalMilliseconds;
            Console.WriteLine($"Bitboard made in {totalMs} ms");
        }
        [SetUp]
        public void Setup()
        {
            giScandi = new GameInfo(fenScandi);
            biScandi = giScandi.BoardInfo;
        }
        [Test]
        public void Should_Return_True_When_d5_Is_Attacked()
        {
            var d5 = BoardHelpers.SquareTextToIndex("d5");
            var isAttacked = biScandi.IsAttackedBy(MagicBitboard.Enums.Color.White, d5.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public void Should_Return_True_When_d5_Is_Attacked_2()
        {
            var gi = new GameInfo(fenQueenIsBlockedFromAttackingd4);
            var d5 = BoardHelpers.SquareTextToIndex("d5");
            var isAttacked = gi.BoardInfo.IsAttackedBy(MagicBitboard.Enums.Color.Black, d5.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public void Should_Return_False_When_d4_Is_Not_Attacked()
        {
            var d4 = BoardHelpers.SquareTextToIndex("d4");
            var isAttacked = biScandi.IsAttackedBy(MagicBitboard.Enums.Color.White, d4.Value);
            Assert.IsFalse(isAttacked);
        }

        [Test]
        public void Should_Return_False_When_d4_Is_Not_Attacked_2()
        {
            var gi = new GameInfo(fenQueenIsBlockedFromAttackingd4);
            var d4 = BoardHelpers.SquareTextToIndex("d4");
            var isAttacked = gi.BoardInfo.IsAttackedBy(MagicBitboard.Enums.Color.Black, d4.Value);
            Assert.IsFalse(isAttacked);
        }

        [Test]
        public void Should_Return_True_When_d4_Is_Attacked()
        {
            var gi = new GameInfo(fenQueenAttacksd4);
            var d4 = BoardHelpers.SquareTextToIndex("d4");
            var isAttacked = gi.BoardInfo.IsAttackedBy(MagicBitboard.Enums.Color.Black, d4.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public void ShouldGetCorrectMoveWhenPromotionIsSent()
        {
            //for (ushort i = 48; i < 56; i++)
            //{
            //    for (var pieceIdx = PromotionPiece.Knight; pieceIdx < PromotionPiece.Queen; pieceIdx++)
            //    {
            //        var expected = MoveHelpers.GenerateMove(i, (ushort)(i + 8), MoveType.Promotion, pieceIdx);
            //        var input = DisplayHelpers.IndexToSquareDisplay((ushort)(i + 8)) + $"={PieceHelpers.GetCharFromPromotionPiece(pieceIdx)}";
            //        Assert.AreEqual(expected, MoveHelpers.GenerateMoveFromText(input, Color.White));
            //    }
            //}
            //biEnPassent.ActivePlayer = Color.Black;
            //for (ushort i = 8; i < 16; i++)
            //{
            //    for (var pieceIdx = PromotionPiece.Knight; pieceIdx < PromotionPiece.Queen; pieceIdx++)
            //    {
            //        var expected = MoveHelpers.GenerateMove(i, (ushort)(i - 8), MoveType.Promotion, pieceIdx);
            //        var input = DisplayHelpers.IndexToSquareDisplay((ushort)(i - 8)) + $"={PieceHelpers.GetCharFromPromotionPiece(pieceIdx)}";
            //        Assert.AreEqual(expected, MoveHelpers.GenerateMoveFromText(input, Color.Black));
            //    }
            //}
        }
        [Test]
        public void ShouldFailWhenNoPawnIsIncapableOfPromotion()
        {
            var fen = "8/PPPP1PPP/8/2k5/8/2K5/pppp1ppp/8 w - - 0 1";
            var bi = FENHelpers.BoardInfoFromFen(fen);
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.White;
                bi.ValidateMove(bi.GenerateMoveFromText("e8=Q"));
            });
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.Black;
                bi.ValidateMove(bi.GenerateMoveFromText("e1=Q"));
            });
        }

        [Test]
        public void ShouldFailWhenAPieceBlocksPromotion()
        {
            var fen = "4q3/PPPPPPPP/8/2k5/8/2K5/pppppppp/4Q3 w - - 0 1";
            var bi = FENHelpers.BoardInfoFromFen(fen);
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.White;
                bi.ValidateMove(bi.GenerateMoveFromText("e8=Q"));
            });
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.Black;
                bi.ValidateMove(bi.GenerateMoveFromText("e1=Q"));
            });
        }

        [Test]
        public void ShouldFindCorrectSource_PawnMove_NoCapture_StartingPosition()
        {
            const ulong blackPawnOcc = 0xff000000000000;
            const ulong whitePawnOcc = 0x0000000000ff00;
            var boardInfo = new GameInfo().BoardInfo;
            var md = new MoveDetail();
            md.Color = Color.Black;
            for (ushort destIndex = 40; destIndex >= 32; destIndex--)
            {
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestFile = (ushort)(destIndex % 8);
                md.DestRank = (ushort)(destIndex / 8);
                var expectedSource = md.DestRank > 32 ? md.DestRank + 1 : md.DestRank + 2;
                var actual = boardInfo.FindPawnMoveSourceIndex(md, blackPawnOcc);
            }
            md.Color = Color.White;

            for (ushort destIndex = 31; destIndex >= 16; destIndex--)
            {
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestFile = (ushort)(destIndex % 8);
                md.DestRank = (ushort)(destIndex / 8);
                var expectedSource = md.DestRank > 16 ? md.DestRank + 2 : md.DestRank + 1;
                var actual = boardInfo.FindPawnMoveSourceIndex(md, whitePawnOcc);
            }
        }
    }
}
