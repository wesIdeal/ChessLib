using MagicBitboard;
using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MagicBitboard.Helpers.MoveHelpers;

namespace MagicBitboard.Helpers.Tests
{
    [TestFixture]
    public class MoveHelpersTests
    {
        [Test]
        public void ShouldReturnCorrectDetailWhenBlackCastlesShort()
        {
            var move = "O-O";
            var mdExpected = new MoveDetail(Color.Black, 4, 7, 6, 7, Piece.King, false, MoveType.Castle);
            var actual = GetAvailableMoveDetails(move, Color.Black);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual, move);
        }
        [Test]
        public void ShouldReturnCorrectDetailWhenWhiteCastlesShort()
        {
            var move = "O-O";
            var mdExpected = new MoveDetail(Color.White, 4, 0, 6, 0, Piece.King, false, MoveType.Castle);
            var actual = GetAvailableMoveDetails(move, Color.White);
            Assert.AreEqual(mdExpected, actual);
        }
        [Test]
        public void ShouldReturnCorrectDetailWhenBlackCastlesLong()
        {
            var move = "O-O-O";
            var mdExpected = new MoveDetail(Color.Black, 4, 7, 2, 7, Piece.King, false, MoveType.Castle);
            var actual = GetAvailableMoveDetails(move, Color.Black);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual, move);
        }
        [Test]
        public void ShouldReturnCorrectDetailWhenWhiteCastlesLong()
        {
            var move = "O-O-O";
            var mdExpected = new MoveDetail(Color.White, 4, 0, 2, 0, Piece.King, false, MoveType.Castle);
            var actual = GetAvailableMoveDetails(move, Color.White);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual, move);
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
                    var actual = GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(Piece.Pawn, actual.Piece);
                    ValidateHasDestInfo(actual, move);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture, $"Capture flag should be set on pawn capture for move {move}");
                        Assert.IsNotNull(actual.SourceFile, $"Source file should be set on pawn capture for move {move}");
                        Assert.IsNotNull(actual.SourceRank, $"Source rank should be set on pawn capture for move {move}");
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
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(expectedPiece, actual.Piece);
                    ValidateHasDestInfo(actual, move);
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
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(1, actual.SourceFile);
                    ValidateHasDestInfo(actual, move);
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
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(0, actual.SourceRank);
                    ValidateHasDestInfo(actual, move);
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
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(4, actual.DestFile);
                    ValidateHasDestInfo(actual, move);
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
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(3, actual.DestRank);
                    ValidateHasDestInfo(actual, move);
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
                var expectedPiece = PieceHelpers.GetPromotionPieceFromChar(piece[0]);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(expectedPiece, actual.PromotionPiece);
                    Assert.AreEqual(MoveType.Promotion, actual.MoveType);
                    ValidateHasDestInfo(actual, move);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }


        public void ValidateHasDestInfo(MoveDetail m, string moveText)
        {
            Assert.IsNotNull(m.DestRank, $"Destination rank should be specified for move {moveText}");
            Assert.IsNotNull(m.DestFile, $"Destination file should be specified for move {moveText}");

        }
    }
}
