using MagicBitboard;
using MagicBitboard.Enums;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MagicBitboard.Helpers.MoveHelpers;

namespace ChessLib.Tests.Helpers
{
    [TestFixture]
    public class MoveHelperTests
    {
        [Test]
        public void ShouldReturnCorrectDetailWhenBlackCastlesShort()
        {
            var mdExpected = new MoveDetail(4, 7, 6, 7, MagicBitboard.Piece.King, false, MagicBitboard.Enums.MoveType.Castle);
            var actual = GetAvailableMoveDetails("O-O", MagicBitboard.Enums.Color.Black);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual);
        }
        [Test]
        public void ShouldReturnCorrectDetailWhenWhiteCastlesShort()
        {
            var mdExpected = new MoveDetail(4, 0, 6, 0, MagicBitboard.Piece.King, false, MagicBitboard.Enums.MoveType.Castle);
            var actual = GetAvailableMoveDetails("O-O", MagicBitboard.Enums.Color.White);
            Assert.AreEqual(mdExpected, GetAvailableMoveDetails("O-O", MagicBitboard.Enums.Color.White));
        }
        [Test]
        public void ShouldReturnCorrectDetailWhenBlackCastlesLong()
        {
            var mdExpected = new MoveDetail(4, 7, 2, 7, MagicBitboard.Piece.King, false, MagicBitboard.Enums.MoveType.Castle);
            var actual = GetAvailableMoveDetails("O-O-O", MagicBitboard.Enums.Color.Black);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual);
        }
        [Test]
        public void ShouldReturnCorrectDetailWhenWhiteCastlesLong()
        {
            var mdExpected = new MoveDetail(4, 0, 2, 0, MagicBitboard.Piece.King, false, MagicBitboard.Enums.MoveType.Castle);
            var actual = GetAvailableMoveDetails("O-O-O", MagicBitboard.Enums.Color.White);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual);
        }
        [Test]
        public void ShouldReturnCorrectPiece_Pawn()
        {
            var moveFormat = new[] { "{0}xe4", "{0}4" };
            foreach (var fmt in moveFormat)
            {
                for (char i = 'a'; i <= 'h'; i++)
                {
                    var move = string.Format(fmt, i);
                    var actual = GetAvailableMoveDetails(move, MagicBitboard.Enums.Color.White);
                    Assert.AreEqual(Piece.Pawn, actual.Piece);
                    ValidateHasDestInfo(actual);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }
        [Test]
        public void ShouldReturnCorrectPiece()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}xe4", "{0}b4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceOfColor.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, MagicBitboard.Enums.Color.White);
                    Assert.AreEqual(expectedPiece, actual.Piece);
                    ValidateHasDestInfo(actual);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }
        [Test]
        public void ShouldReturnCorrectSourceFile()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}bxe4", "{0}bd4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceOfColor.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, MagicBitboard.Enums.Color.White);
                    Assert.AreEqual(1, actual.SourceFile);
                    ValidateHasDestInfo(actual);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }
        [Test]
        public void ShouldReturnCorrectSourceRank()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}1xe4", "{0}1d4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceOfColor.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, MagicBitboard.Enums.Color.White);
                    Assert.AreEqual(0, actual.SourceRank);
                    ValidateHasDestInfo(actual);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }

        [Test]
        public void ShouldReturnCorrectDestFile()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}bxe4", "{0}be4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceOfColor.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, MagicBitboard.Enums.Color.White);
                    Assert.AreEqual(4, actual.DestFile);
                    ValidateHasDestInfo(actual);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }
        [Test]
        public void ShouldReturnCorrectDestRank()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}1xe4", "{0}1d4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceOfColor.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, MagicBitboard.Enums.Color.White);
                    Assert.AreEqual(3, actual.DestRank);
                    ValidateHasDestInfo(actual);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }
        [Test]
        public void ShouldReturnCorrectPromotionObject()
        {
            var moveFormat = new[] { "fxe8={0}", "e8={0}" };
            var pieces = new[] { "N", "B", "R", "Q" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceOfColor.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, MagicBitboard.Enums.Color.White);
                    Assert.AreEqual(expectedPiece, actual.PromotionPiece);
                    Assert.AreEqual(MoveType.Promotion, actual.MoveType);
                    ValidateHasDestInfo(actual);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }


        public void ValidateHasDestInfo(MoveDetail m)
        {
            Assert.IsNotNull(m.DestFile);
            Assert.IsNotNull(m.DestRank);
        }
    }
}
