using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;

namespace ChessLib.Data.Tests
{
    [TestFixture]
    public static class MoveTranslatorServiceTests
    {
        private static void ValidateHasDestInfo(MoveDetail m, string moveText)
        {
            Assert.IsNotNull(m.DestinationRank, $"Destination rank should be specified for move {moveText}");
            Assert.IsNotNull(m.DestinationFile, $"Destination file should be specified for move {moveText}");
        }

        [Test]
        public static void ShouldReturnCorrectDetailWhenBlackCastlesShort()
        {
            var move = "O-O";
            var mdExpected = new MoveDetail(7, 4, 7, 6, Piece.King, Color.Black, "O-O", false, MoveType.Castle);
            var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.Black);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual, move);
        }
        [Test]
        public static void ShouldReturnCorrectDetailWhenWhiteCastlesShort()
        {
            var move = "O-O";
            var mdExpected = new MoveDetail(0, 4, 0, 6, Piece.King, Color.White, "O-O", false, MoveType.Castle);
            var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.White);
            Assert.AreEqual(mdExpected, actual);
        }
        [Test]
        public static void ShouldReturnCorrectDetailWhenBlackCastlesLong()
        {
            var move = "O-O-O";
            var mdExpected = new MoveDetail(7, 4, 7, 2, Piece.King, Color.Black, "O-O-O", false, MoveType.Castle);
            var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.Black);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual, move);
        }
        [Test]
        public static void ShouldReturnCorrectDetailWhenWhiteCastlesLong()
        {
            var move = "O-O-O";
            var mdExpected = new MoveDetail(0, 4, 0, 2, Piece.King, Color.White, "O-O-O", false, MoveType.Castle);
            var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.White);
            Assert.AreEqual(mdExpected, actual);
            ValidateHasDestInfo(actual, move);
        }

        [Test]
        public static void ShouldFindCorrectSource_PawnMove_Capture_Promotion()
        {
            var expected = 53;
            var actual = MoveTranslatorService.GetAvailableMoveDetails("fxe8=Q+", Color.White);
            Assert.AreEqual(expected, actual.SourceIndex);
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
                    var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(Piece.Pawn, actual.Piece);
                    ValidateHasDestInfo(actual, move);
                    if (fmt.Contains("x"))
                    {
                        Assert.IsTrue(actual.IsCapture, $"Capture flag should be set on pawn capture for move {move}");
                        Assert.IsNotNull(actual.SourceFile, $"Source file should be set on pawn capture for move {move}");
                        Assert.IsNotNull(actual.SourceRank, $"Source rank should be set on pawn capture for move {move}");
                    }
                }
            }
        }

        [Test]
        public static void GetAvailableMoveDetails_ShouldThrowException_IfMoveIsLessThan2Chars()
        {
            var expectedMessage = "Invalid move. Must have at least 2 characters.";
            Assert.Throws<Exception>(() =>
            {
                try
                {
                    MoveTranslatorService.GetAvailableMoveDetails("e", Color.Black);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(expectedMessage, e.Message);
                    throw;
                }
            });


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
                    var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.White);
                    Assert.AreEqual(expectedPiece, actual.Piece);
                    ValidateHasDestInfo(actual, move);
                    if (fmt.Contains("x"))
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
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.White);
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
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.White);
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
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.White);
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
                foreach (var fmt in moveFormat)
                {
                    var move = string.Format(fmt, piece);
                    var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.White);
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
                    var actual = MoveTranslatorService.GetAvailableMoveDetails(move, Color.White);
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
        /// <summary>
        /// Should not get pawn source-- let the move validate that, as pawn source is variable, depending on board state
        /// </summary>
        [Test]
        public static void GetAvailableMoveDetails_ShouldNotGetPawnSource()
        {
            var moveW = "e4";
            var moveB = "e5";
            Assert.AreEqual(null, MoveTranslatorService.GetAvailableMoveDetails(moveW, Color.White).SourceIndex);
            Assert.AreEqual(null, MoveTranslatorService.GetAvailableMoveDetails(moveB, Color.Black).SourceIndex);
        }

        [Test]
        public static void ShouldFindCorrectSource_PawnMove_NoCapture_StartingPosition()
        {
            var boardInfo = new BoardInfo() { ActivePlayer = Color.Black };
            var moveTranslatorService = new MoveTranslatorService(boardInfo);
            var md = new MoveDetail
            {
                Color = Color.Black
            };
            boardInfo.ActivePlayer = Color.Black;
            for (ushort destIndex = 40; destIndex >= 32; destIndex--)
            {
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestinationFile = (ushort)(destIndex % 8);
                md.DestinationRank = (ushort)(destIndex / 8);
                var expectedSource = MoveHelpers.GenerateMove((ushort)(48 + (destIndex % 8)), destIndex);
                var actual = moveTranslatorService.GenerateMoveFromText(destIndex.IndexToSquareDisplay());
                Assert.AreEqual(expectedSource.Move, actual.Move);
            }

            md.Color = Color.White;
            boardInfo.ActivePlayer = Color.White;
            for (ushort destIndex = 31; destIndex >= 16; destIndex--)
            {
                moveTranslatorService.InitializeBoard();
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestinationFile = (ushort)(destIndex % 8);
                md.DestinationRank = (ushort)(destIndex / 8);
                var source = (ushort)(8 + (destIndex % 8));
                var expectedSource = MoveHelpers.GenerateMove(source, destIndex);
                var actual = moveTranslatorService.GenerateMoveFromText(destIndex.IndexToSquareDisplay());
                Assert.AreEqual(expectedSource.Move, actual.Move);
            }
        }


        [TestCase("r1b2rk1/p3np1p/1pn1pp1q/2b5/2B1N3/3Q1NP1/PPP2P1P/1K1R3R w - - 1 15", null, 30, Piece.Pawn, Color.White, "g4", 22)]
        [TestCase("r4rk1/p4p1p/1p4n1/2b1pbNP/1nB1Nq2/8/PPP2PQ1/1K1R3R b - - 0 21", null, 54, Piece.King, Color.Black, "Kg7", 62)]
        [TestCase("4k3/8/8/8/8/8/8/4K3 w - - 0 1", null, 3, Piece.King, Color.White, "Kd1", 4)]
        [TestCase(FENHelpers.FENInitial, null, 21, Piece.Knight, Color.White, "Nf3", 6)]
        [TestCase("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1", null, 56, Piece.Queen, Color.White, "Qa8+", 0)]
        [TestCase("4k3/8/8/8/8/8/8/R1qbK3 w - - 0 1", null, 56, Piece.Rook, Color.White, "Ra8+", 0)]
        [TestCase("rnb1kbnr/pp3ppp/4p3/2pq4/3P4/8/PPPN1PPP/R1BQKBNR w KQkq - 0 5", null, 21, Piece.Knight, Color.White, "Ngf3", 6)]
        [TestCase("4r1k1/1p1qrb2/p5p1/3p1p2/P1pb1P2/4N2R/1PBN1KP1/3QR3 w - - 4 31", null, 5, Piece.Knight, Color.White, "Nf1", 11)]
        [TestCase("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2", null, 26, Piece.Bishop, Color.White, "Bc4", 5)]
        public static void ShouldMoveSource(string fen, int? sourceIdx, int destIdx, Piece p, Color c, string moveText, int expected)
        {
            var moveTranslatorService = new MoveTranslatorService(new BoardInfo(fen) { ActivePlayer = c });
            //var md = new MoveDetail((ushort?)sourceIdx, (ushort)destIdx, p, c, moveText).GetAvailableMoveDetails();
            var actualMove = moveTranslatorService.GenerateMoveFromText(moveText);
            Assert.AreEqual(expected, actualMove.SourceIndex);
        }

        [TestCase("rnb1kbnr/1p1ppppp/p7/1qp5/3PP3/P1P5/1P3PPP/RNBQKBNR w KQkq - 1 5", "Ba6")]
        public static void FindPieceSourceIndex_ShouldThrowException(string fen, string moveText)
        {
            var moveTranslatorService = new MoveTranslatorService(new BoardInfo(fen));

            Assert.Throws(typeof(MoveException),
                () => { moveTranslatorService.GenerateMoveFromText(moveText); });
        }
    }
}
