using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using ChessLib.MagicBitboard;

namespace ChessLib.Data.Tests.Boards
{
    [TestFixture]
    public class BoardInformationService
    {
        BoardInfo bi = new BoardInfo();
        [Test]
        public void GetSANSourceString_King()
        {
            var poc = new PieceOfColor() { Color = Color.Black, Piece = Piece.King };
            var expected = "K";
            Assert.AreEqual(expected, bi.GetSANSourceString(new MoveExt(0), poc));
        }

        [Test]
        public void GetSANSourceString_Pawn()
        {
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var expected = "e";
            Assert.AreEqual(expected, bi.GetSANSourceString(MoveHelpers.GenerateMove(12, 28), poc));
        }

        [Test]
        public void GetSANSourceString_PieceWithNoDisambiguityRequired()
        {
            var boardInfo = new BoardInfo("7k/8/8/3bR3/8/8/7K/8 w - - 0 1");
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Rook };
            var expected = "R";
            Assert.AreEqual(expected, boardInfo.GetSANSourceString(MoveHelpers.GenerateMove(36, 35), poc));
        }

        [Test]
        public void GetSANSourceString_PieceWithFileDisambiguityRequired()
        {
            var boardInfo = new BoardInfo("7k/8/8/3bR3/8/3R4/7K/8 w - - 0 1");
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Rook };
            var expected = "Re";
            Assert.AreEqual(expected, boardInfo.GetSANSourceString(MoveHelpers.GenerateMove(36, 35), poc));
        }

        [Test]
        public void GetSANSourceString_PieceWithRankAndFileDisambiguityRequired()
        {
            var boardInfo = new BoardInfo("2k5/8/8/1b6/4Q2Q/8/7K/7Q w - - 0 1");
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Queen };
            var expected = "Qh4";
            Assert.AreEqual(expected, boardInfo.GetSANSourceString(MoveHelpers.GenerateMove(31, 4), poc));
        }

        [Test]
        public void GetSANSourceString_PieceWithRankDisambiguityRequired()
        {
            var boardInfo = new BoardInfo("7k/8/3R4/3b4/8/3R4/7K/8 w - - 0 1");
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Rook };
            var expected = "R6";
            Assert.AreEqual(expected, boardInfo.GetSANSourceString(MoveHelpers.GenerateMove(43, 35), poc));
        }

        [Test]
        public void MoveToSAN_PawnMove1()
        {
            var boardInfo = new BoardInfo();
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var expected = "e3";
            Assert.AreEqual(expected, boardInfo.MoveToSAN(MoveHelpers.GenerateMove(12, 20), poc, null));
        }

        [Test]
        public void MoveToSAN_PawnMove2()
        {
            var boardInfo = new BoardInfo();
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var expected = "e4";
            Assert.AreEqual(expected, boardInfo.MoveToSAN(MoveHelpers.GenerateMove(12, 28), poc, null));
        }

        [Test]
        public void MoveToSAN_PawnCapture1Attacker()
        {
            var boardInfo = new BoardInfo("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1");
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var expected = "exd5";
            Assert.AreEqual(expected, boardInfo.MoveToSAN(MoveHelpers.GenerateMove(28, 35)));
        }

        [Test]
        public void MoveToSAN_PawnCapture2Attackers()
        {
            var boardInfo = new BoardInfo("rnbqkbnr/1pp1pppp/p7/3p4/2P1P3/8/PP1P1PPP/RNBQKBNR w KQkq - 0 1");
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var expected = "exd5";
            Assert.AreEqual(expected, boardInfo.MoveToSAN(MoveHelpers.GenerateMove(28, 35)));
            expected = "cxd5";
            Assert.AreEqual(expected, boardInfo.MoveToSAN(MoveHelpers.GenerateMove(26, 35)));
        }

        [Test]
        public void MoveToSAN_PawnPromotion()
        {
            var boardInfo = new BoardInfo("8/4P3/8/8/8/8/6k1/4K3 w - - 0 1");
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var expected = "e8=Q";
            Assert.AreEqual(expected, boardInfo.MoveToSAN(MoveHelpers.GenerateMove(52, 60, MoveType.Promotion, PromotionPiece.Queen)));

        }

        [Test]
        public void MoveToSAN_PawnPromotionAfterCapture()
        {
            var boardInfo = new BoardInfo("3q4/4P3/8/8/8/8/6k1/4K3 w - - 0 1");
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var expected = "exd8=Q";
            Assert.AreEqual(expected, boardInfo.MoveToSAN(MoveHelpers.GenerateMove(52, 59, MoveType.Promotion, PromotionPiece.Queen)));

        }

        [Test]
        public void MoveToSAN_SimpleCheck()
        {
            var boardInfo = new BoardInfo("8/8/8/8/5Q2/8/6k1/4K3 w - - 0 1");
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var expected = "Qe4+";
            Assert.AreEqual(expected, boardInfo.MoveToSAN(MoveHelpers.GenerateMove(29, 28)));
        }
    }
}
