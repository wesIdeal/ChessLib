using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using NUnit.Framework;
using System.Linq;

namespace MagicBitboard.Helpers.Tests
{
    [TestFixture]
    public static class MoveHelpersTests
    {
        [Test]
        public static void ShouldReturnCorrectDetailWhenBlackCastlesShort()
        {
            var move = "O-O";
            var mdExpected = new MoveDetail(7, 4, 7, 6, Piece.King, Color.Black, "O-O", false, MoveType.Castle);
            var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.Black);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual, move);
        }
        [Test]
        public static void ShouldReturnCorrectDetailWhenWhiteCastlesShort()
        {
            var move = "O-O";
            var mdExpected = new MoveDetail(0, 4, 0, 6, Piece.King, Color.White, "O-O", false, MoveType.Castle);
            var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.White);
            Assert.AreEqual(mdExpected, actual);
        }
        [Test]
        public static void ShouldReturnCorrectDetailWhenBlackCastlesLong()
        {
            var move = "O-O-O";
            var mdExpected = new MoveDetail(7, 4, 7, 2, Piece.King, Color.Black, "O-O-O", false, MoveType.Castle);
            var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.Black);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual, move);
        }
        [Test]
        public static void ShouldReturnCorrectDetailWhenWhiteCastlesLong()
        {
            var move = "O-O-O";
            var mdExpected = new MoveDetail(0, 4, 0, 2, Piece.King, Color.White, "O-O-O", false, MoveType.Castle);
            var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.White);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual, move);
        }
        [Test]
        public static void ShouldReturnCorrectPiece_Pawn()
        {
            var moveFormat = new[] { "{0}xe4", "{0}4" };
            foreach (var fmt in moveFormat)
            {
                for (char i = 'a'; i <= 'h'; i++)
                {
                    var move = string.Format(fmt, i);
                    var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.White);
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
        public static void ShouldReturnCorrectPiece()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}xe4", "{0}b4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.White);
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
        public static void ShouldReturnCorrectSourceFile()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}bxe4", "{0}bd4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.White);
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
        public static void ShouldReturnCorrectSourceRank()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}1xe4", "{0}1d4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.White);
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
        public static void ShouldReturnCorrectDestFile()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}bxe4", "{0}be4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(4, actual.DestinationFile);
                    ValidateHasDestInfo(actual, move);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }
        [Test]
        public static void ShouldReturnCorrectDestRank()
        {
            var pieces = new[] { "N", "B", "R", "Q", "K" };
            var moveFormat = new[] { "{0}1xe4", "{0}1d4" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceHelpers.GetPiece(piece);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(3, actual.DestinationRank);
                    ValidateHasDestInfo(actual, move);
                    if (fmt.Contains('x'))
                    {
                        Assert.IsTrue(actual.IsCapture);
                    }
                }
            }
        }
        [Test]
        public static void ShouldReturnCorrectPromotionObject()
        {
            var moveFormat = new[] { "fxe8={0}", "e8={0}" };
            var pieces = new[] { "N", "B", "R", "Q" };
            foreach (var piece in pieces)
            {
                var expectedPiece = PieceHelpers.GetPromotionPieceFromChar(piece[0]);
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveHelpers.GetAvailableMoveDetails(move, Color.White);
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

        public static void ValidateHasDestInfo(MoveDetail m, string moveText)
        {
            Assert.IsNotNull(m.DestinationRank, $"Destination rank should be specified for move {moveText}");
            Assert.IsNotNull(m.DestinationFile, $"Destination file should be specified for move {moveText}");

        }
    }
}
