using System;
using System.Text;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using EnumsNET;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Helpers
{
    [TestFixture]
    public partial class BoardHelpersTests
    {
        private static readonly FenTextToBoard FenReader = new FenTextToBoard();

        [TestCase("8/1k6/8/8/4q3/8/6K1/8 w - - 0 1", "Single check from Queen, has evasions")]
        [TestCase("8/8/8/8/8/7k/6q1/7K w - - 0 1", "Single check from Queen. Checkmate")]
        [TestCase("8/8/8/8/6kr/8/8/7K w - - 0 1", "Single check from rook, has evasions")]
        [TestCase("8/1k6/8/8/8/8/7b/6qK w - - 2 2", "Single check from Queen and Bishop. Checkmate.")]
        [TestCase("8/8/8/8/7k/8/6q1/7K w - - 0 1", "Single check from Queen, can capture.")]
        [TestCase("8/8/8/8/7k/8/6b1/7K w - - 0 1", "Single check from Bishop, can capture.")]
        [TestCase("8/8/8/8/7k/8/6p1/7K w - - 0 1", "Single check from pawn, can capture.")]
        [TestCase("8/8/8/8/7k/6n1/8/7K w - - 0 1", "Single check from Knight, can capture.")]
        [TestCase("8/8/8/8/8/4k1q1/3n4/5K2 w - - 0 1", "Single check from knight, checkmate.")]
        [TestCase("8/1k6/2b5/8/8/5q1r/5pp1/6K1 w - - 0 1", "Single check from pawn, checkmate.")]
        public static void GetCheckType_ShouldReturnSingleForSingleChecks(string fen, string description = "")
        {
            var board = FenReader.Translate(fen);
            var result = BoardHelpers.GetCheckType(board.Occupancy, board.ActivePlayer, out _);
            var message = GetCheckmateTypeDescription(fen, description, result);
            Console.WriteLine(message);
            Assert.AreEqual(BoardHelpers.CheckType.Single, result, message);
        }

        [TestCase("8/8/8/8/7k/7q/6p1/7K w - - 0 1", "Double check, Queen and Pawn. Can evade.")]
        [TestCase("8/8/8/8/7k/4b2q/6p1/7K w - - 0 1", "Double check, Queen and Pawn. Checkmate.")]
        [TestCase("7k/6P1/4B2Q/7K/8/8/8/8 b - - 0 1", "Double check, Queen and Pawn. Checkmate.")]
        [TestCase("8/1k6/8/8/4q2n/8/6K1/8 w - - 0 1", "Double check, Queen and Knight. Can evade.")]
        [TestCase("8/8/8/8/6kr/8/8/4r2K w - - 0 1", "Double check from 2 rooks, checkmate.")]
        [TestCase("8/1k6/8/8/7n/8/5qr1/6K1 w - - 0 1", "Double check from Queen and Rook. Can evade.")]
        [TestCase("8/1k6/8/8/8/8/5q1b/6K1 w - - 0 1", "Double check from Queen and Bishop. Can capture and evade.")]
        [TestCase("8/8/8/8/7k/6n1/6p1/7K w - - 0 1", "Double check from Knight+Pawn, can capture and evade.")]
        public static void GetCheckType_ShouldReturnDoubleForDoubleChecks(string fen, string description = "")
        {
            var board = FenReader.Translate(fen);

            var result = BoardHelpers.GetCheckType(board.Occupancy, board.ActivePlayer, out _);
            var message = GetCheckmateTypeDescription(fen, description, result);
            Console.WriteLine(message);
            Assert.AreEqual(BoardHelpers.CheckType.Double, result, message);
        }

        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "Initial position.")]
        [TestCase("rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq - 0 1", "Initial position -> 1. c4.")]
        [TestCase("rnbqkbnr/pppp1ppp/8/4p3/2P5/8/PP1PPPPP/RNBQKBNR w KQkq - 0 2", "Initial position-> 1. c4 e5.")]
        public static void GetCheckType_ShouldReturnNoneForNoChecks(string fen,
            string description = "No checks expected.")
        {
            var board = FenReader.Translate(fen);
            var result = BoardHelpers.GetCheckType(board.Occupancy, board.ActivePlayer, out _);
            var message = GetCheckmateTypeDescription(fen, description, result);
            Console.WriteLine(message);
            Assert.AreEqual(BoardHelpers.CheckType.None, result, message);
        }

        private static string GetCheckmateTypeDescription(string fen, string description, BoardHelpers.CheckType result)
        {
            var message =
                $"{description}{Environment.NewLine}From position: {fen}{Environment.NewLine}Result was: {result.AsString()}";
            return message;
        }

        //[TestCase("5Q2/7k/1R6/7P/6K1/8/8/8 b - - 0 62", false)]
        //[TestCase("4k1K1/6P1/8/7q/8/8/8/8 w - - 10 57", false)]
        //[TestCase("6K1/4k1P1/8/7q/8/8/8/8 w - - 10 57", false)]
        //[TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", false)]
        //[TestCase("rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq d3 0 1", false)]
        //[TestCase("8/8/8/8/8/8/5Qk1/4K3 b - - 0 1", false)]
        //[TestCase("8/8/8/8/8/8/5QkQ/4K3 b - - 0 1", true)]
        //[TestCase("8/8/8/8/3b4/8/3Q2k1/4K3 b - - 0 1", false)]
        //[TestCase("8/8/8/8/3b2B1/5N1Q/6k1/4K3 b - - 0 1", true)]
        //[TestCase("3qk3/5Q1p/8/p1p1N3/Pp2bP1P/1P1r4/8/4RnK1 b - - 6 38", true)]
        //[TestCase("7R/pp4p1/2p3Bk/5P2/7P/8/PP4p1/4K3 b - - 1 55", true)]
        //[TestCase("4R3/2p3pk/pp3p2/5n1p/2P2P1P/P5r1/1P4q1/3QR2K w - - 6 41", true)]
        //[TestCase("2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23", false)]
        //[TestCase("2bq1rk1/3p1Bpp/p1p3N1/1rb2Pp1/1pQ5/P5N1/1PP3PP/R3R2K b - - 0 23", false)]
        //public static void IsCheckmate(string fen, bool expected)
        //{
        //    var pieces = FENHelpers.BoardFromFen(fen, out Color activePlayer, out _, out _, out _, out _, false);
        //    var actualResult = BoardHelpers.IsCheckmate(pieces, activePlayer);
        //    Assert.AreEqual(expected, actualResult);
        //}

        //[TestCase("7R/pp4p1/2p3Bk/5P2/7P/8/PP4p1/4K3 b - - 1 55")]
        //public static void KingShouldNotHaveEvasions(string fen)
        //{
        //    var pieces = FENHelpers.BoardFromFen(fen, out Color activePlayer, out _, out _, out _, out _, false);
        //    var actualResult = BoardHelpers.DoesKingHaveEvasions(pieces, activePlayer);
        //    Assert.IsFalse(actualResult);
        //}

        //[TestCase("7R/pp4p1/2p3Bk/5P2/7P/8/PP4p1/4K3 b - - 1 55")]
        //public static void KingShouldNotHaveLegalMoves(string fen)
        //{
        //    var pieces = FENHelpers.BoardFromFen(fen, out Color activePlayer, out _, out _, out _, out _, false);
        //    var actualResult = BoardHelpers.GetValidKingMoves(pieces, activePlayer);
        //    Assert.IsEmpty(actualResult);
        //}


        [TestCase("rnbqkbnr/p1pppppp/8/8/Pp6/8/1PPPPPPP/RNBQKBNR b KQkq a3 0 1", true)]
        [TestCase("rnbqkbnr/p1pppppp/8/1p6/P7/8/1PPPPPPP/RNBQKBNR b KQkq a3 0 1", false)]
        [TestCase("rnbqkbnr/pppppp1p/8/8/6pP/8/PPPPPPP1/RNBQKBNR b KQkq h3 0 1", true)]
        [TestCase("rnbqkbnr/pppppp1p/6p1/8/7P/8/PPPPPPP1/RNBQKBNR b KQkq h3 0 1", false)]
        [TestCase("rnbqkbnr/1ppp1ppp/8/pP2p3/8/8/P1PPPPPP/RNBQKBNR w KQkq a6 0 3", true)]
        [TestCase("rnbqkbnr/1ppp1ppp/8/p3p3/1PP5/8/P2PPPPP/RNBQKBNR w KQkq - 0 3", false)]
        [TestCase("rnbqkbnr/ppp2ppp/8/2Ppp3/8/8/PP1PPPPP/RNBQKBNR w KQkq d6 0 3", true)]
        [TestCase("rnbqkbnr/ppp2ppp/8/3pp3/2P1P3/8/PP1P1PPP/RNBQKBNR w KQkq - 0 3", false)]
        [TestCase("rnbqkbnr/pppp1pp1/8/6Pp/2P1p3/8/PP1PPP1P/RNBQKBNR w KQkq h6 0 4", true)]
        [TestCase("rnbqkbnr/pppp1pp1/8/7p/2P1p1P1/1P6/P2PPP1P/RNBQKBNR w KQkq - 0 4", false)]
        [TestCase("rnbqkbnr/pppp1ppp/8/8/2PPp1P1/8/PP2PP1P/RNBQKBNR b KQkq d3 0 3", true)]
        [TestCase("rnbqkbnr/ppp2ppp/3p4/4p3/2PP2P1/8/PP2PP1P/RNBQKBNR b KQkq - 0 3", false)]
        public static void TestEnPassantIsAvailable(string fen, bool expected)
        {
            var board = FenReader.Translate(fen);
            Assert.AreEqual(expected, board.IsEnPassantCaptureAvailable());
        }

        [TestFixture]
        public static class InitializationTests
        {
            [Test]
            public static void TestRankCompliment()
            {
                Assert.AreEqual(0, BoardHelpers.RankCompliment(7));
                Assert.AreEqual(1, BoardHelpers.RankCompliment(6));
                Assert.AreEqual(2, BoardHelpers.RankCompliment(5));
                Assert.AreEqual(3, BoardHelpers.RankCompliment(4));
                Assert.AreEqual(4, BoardHelpers.RankCompliment(3));
                Assert.AreEqual(5, BoardHelpers.RankCompliment(2));
                Assert.AreEqual(6, BoardHelpers.RankCompliment(1));
                Assert.AreEqual(7, BoardHelpers.RankCompliment(0));
            }

            [Test]
            public static void InitializeFileMasks_FileMasksProperlyInitialized()
            {
                Assert.AreEqual(0x101010101010101, BoardConstants.FileMasks[0],
                    "'A' File Mask not initialized properly.");
                Assert.AreEqual(0x202020202020202, BoardConstants.FileMasks[1],
                    "'B' File Mask not initialized properly.");
                Assert.AreEqual(0x404040404040404, BoardConstants.FileMasks[2],
                    "'C' File Mask not initialized properly.");
                Assert.AreEqual(0x808080808080808, BoardConstants.FileMasks[3],
                    "'D' File Mask not initialized properly.");
                Assert.AreEqual(0x1010101010101010, BoardConstants.FileMasks[4],
                    "'E' File Mask not initialized properly.");
                Assert.AreEqual(0x2020202020202020, BoardConstants.FileMasks[5],
                    "'F' File Mask not initialized properly.");
                Assert.AreEqual(0x4040404040404040, BoardConstants.FileMasks[6],
                    "'G' File Mask not initialized properly.");
                Assert.AreEqual(0x8080808080808080, BoardConstants.FileMasks[7],
                    "'H' File Mask not initialized properly.");
            }

            [Test]
            public static void InitializeRankMasks_RankMasksProperlyInitialized()
            {
                Assert.AreEqual(0xff, BoardConstants.RankMasks[0], "Rank 1 Mask not initialized properly.");
                Assert.AreEqual(0xff00, BoardConstants.RankMasks[1], "Rank 2 Mask not initialized properly.");
                Assert.AreEqual(0xff0000, BoardConstants.RankMasks[2], "Rank 3 Mask not initialized properly.");
                Assert.AreEqual(0xff000000, BoardConstants.RankMasks[3], "Rank 4 Mask not initialized properly.");
                Assert.AreEqual(0xff00000000, BoardConstants.RankMasks[4], "Rank 5 Mask not initialized properly.");
                Assert.AreEqual(0xff0000000000, BoardConstants.RankMasks[5], "Rank 6 Mask not initialized properly.");
                Assert.AreEqual(0xff000000000000, BoardConstants.RankMasks[6], "Rank 7 Mask not initialized properly.");
                Assert.AreEqual(0xff00000000000000, BoardConstants.RankMasks[7],
                    "Rank 8 Mask not initialized properly.");
            }

            [Test]
            public static void IndividualSquareValidityTest()
            {
                Assert.AreEqual(0x0000000000000001, BoardConstants.IndividualSquares[0]);
                Assert.AreEqual(0x0000000000000002, BoardConstants.IndividualSquares[1]);
                Assert.AreEqual(0x0000000000000004, BoardConstants.IndividualSquares[2]);
                Assert.AreEqual(0x0000000000000008, BoardConstants.IndividualSquares[3]);
                Assert.AreEqual(0x0000000000000010, BoardConstants.IndividualSquares[4]);
                Assert.AreEqual(0x0000000000000020, BoardConstants.IndividualSquares[5]);
                Assert.AreEqual(0x0000000000000040, BoardConstants.IndividualSquares[6]);
                Assert.AreEqual(0x0000000000000080, BoardConstants.IndividualSquares[7]);
                Assert.AreEqual(0x0000000000000100, BoardConstants.IndividualSquares[8]);
                Assert.AreEqual(0x0000000000000200, BoardConstants.IndividualSquares[9]);
                Assert.AreEqual(0x0000000000000400, BoardConstants.IndividualSquares[10]);
                Assert.AreEqual(0x0000000000000800, BoardConstants.IndividualSquares[11]);
                Assert.AreEqual(0x0000000000001000, BoardConstants.IndividualSquares[12]);
                Assert.AreEqual(0x0000000000002000, BoardConstants.IndividualSquares[13]);
                Assert.AreEqual(0x0000000000004000, BoardConstants.IndividualSquares[14]);
                Assert.AreEqual(0x0000000000008000, BoardConstants.IndividualSquares[15]);
                Assert.AreEqual(0x0000000000010000, BoardConstants.IndividualSquares[16]);
                Assert.AreEqual(0x0000000000020000, BoardConstants.IndividualSquares[17]);
                Assert.AreEqual(0x0000000000040000, BoardConstants.IndividualSquares[18]);
                Assert.AreEqual(0x0000000000080000, BoardConstants.IndividualSquares[19]);
                Assert.AreEqual(0x0000000000100000, BoardConstants.IndividualSquares[20]);
                Assert.AreEqual(0x0000000000200000, BoardConstants.IndividualSquares[21]);
                Assert.AreEqual(0x0000000000400000, BoardConstants.IndividualSquares[22]);
                Assert.AreEqual(0x0000000000800000, BoardConstants.IndividualSquares[23]);
                Assert.AreEqual(0x0000000001000000, BoardConstants.IndividualSquares[24]);
                Assert.AreEqual(0x0000000002000000, BoardConstants.IndividualSquares[25]);
                Assert.AreEqual(0x0000000004000000, BoardConstants.IndividualSquares[26]);
                Assert.AreEqual(0x0000000008000000, BoardConstants.IndividualSquares[27]);
                Assert.AreEqual(0x0000000010000000, BoardConstants.IndividualSquares[28]);
                Assert.AreEqual(0x0000000020000000, BoardConstants.IndividualSquares[29]);
                Assert.AreEqual(0x0000000040000000, BoardConstants.IndividualSquares[30]);
                Assert.AreEqual(0x0000000080000000, BoardConstants.IndividualSquares[31]);
                Assert.AreEqual(0x0000000100000000, BoardConstants.IndividualSquares[32]);
                Assert.AreEqual(0x0000000200000000, BoardConstants.IndividualSquares[33]);
                Assert.AreEqual(0x0000000400000000, BoardConstants.IndividualSquares[34]);
                Assert.AreEqual(0x0000000800000000, BoardConstants.IndividualSquares[35]);
                Assert.AreEqual(0x0000001000000000, BoardConstants.IndividualSquares[36]);
                Assert.AreEqual(0x0000002000000000, BoardConstants.IndividualSquares[37]);
                Assert.AreEqual(0x0000004000000000, BoardConstants.IndividualSquares[38]);
                Assert.AreEqual(0x0000008000000000, BoardConstants.IndividualSquares[39]);
                Assert.AreEqual(0x0000010000000000, BoardConstants.IndividualSquares[40]);
                Assert.AreEqual(0x0000020000000000, BoardConstants.IndividualSquares[41]);
                Assert.AreEqual(0x0000040000000000, BoardConstants.IndividualSquares[42]);
                Assert.AreEqual(0x0000080000000000, BoardConstants.IndividualSquares[43]);
                Assert.AreEqual(0x0000100000000000, BoardConstants.IndividualSquares[44]);
                Assert.AreEqual(0x0000200000000000, BoardConstants.IndividualSquares[45]);
                Assert.AreEqual(0x0000400000000000, BoardConstants.IndividualSquares[46]);
                Assert.AreEqual(0x0000800000000000, BoardConstants.IndividualSquares[47]);
                Assert.AreEqual(0x0001000000000000, BoardConstants.IndividualSquares[48]);
                Assert.AreEqual(0x0002000000000000, BoardConstants.IndividualSquares[49]);
                Assert.AreEqual(0x0004000000000000, BoardConstants.IndividualSquares[50]);
                Assert.AreEqual(0x0008000000000000, BoardConstants.IndividualSquares[51]);
                Assert.AreEqual(0x0010000000000000, BoardConstants.IndividualSquares[52]);
                Assert.AreEqual(0x0020000000000000, BoardConstants.IndividualSquares[53]);
                Assert.AreEqual(0x0040000000000000, BoardConstants.IndividualSquares[54]);
                Assert.AreEqual(0x0080000000000000, BoardConstants.IndividualSquares[55]);
                Assert.AreEqual(0x0100000000000000, BoardConstants.IndividualSquares[56]);
                Assert.AreEqual(0x0200000000000000, BoardConstants.IndividualSquares[57]);
                Assert.AreEqual(0x0400000000000000, BoardConstants.IndividualSquares[58]);
                Assert.AreEqual(0x0800000000000000, BoardConstants.IndividualSquares[59]);
                Assert.AreEqual(0x1000000000000000, BoardConstants.IndividualSquares[60]);
                Assert.AreEqual(0x2000000000000000, BoardConstants.IndividualSquares[61]);
                Assert.AreEqual(0x4000000000000000, BoardConstants.IndividualSquares[62]);
                Assert.AreEqual(0x8000000000000000, BoardConstants.IndividualSquares[63]);
                var sb = new StringBuilder();
                for (var i = 0; i < 64; i++)
                {
                    sb.Append(
                        $"0x{Convert.ToString((long)BoardConstants.IndividualSquares[i], 16)}, {(i != 0 && i % 7 == 0 ? "\r\n" : "")}");
                }

                Console.WriteLine(sb.ToString());
            }


            [Test]
            public static void GetRank_ShouldReturnCorrectRank()
            {
                for (var i = 0; i < 64; i++)
                {
                    var expected = (Rank)(i / 8);
                    Assert.AreEqual(expected, i.GetRank());
                }
            }

            [Test]
            public static void FileToIntFunctionality_ShouldConvertFileEnumValueToInt()
            {
                var c = 0;
                foreach (var f in (File[])Enum.GetValues(typeof(File)))
                {
                    Assert.AreEqual(c, (int)f);
                    c++;
                }
            }

            [Test]
            public static void RankToIntFunctionality_ShouldConvertRankEnumValueToInt()
            {
                var c = 0;
                foreach (var r in (Rank[])Enum.GetValues(typeof(Rank)))
                {
                    Assert.AreEqual(c, (int)r, $"Rank {r.ToString()} should convert to {c}");
                    c++;
                }
            }


            [Test]
            public static void InBetween_ShouldReturnCorrectValue_GivenA1H8()
            {
                var expected = 0x40201008040200;
                var actual = BoardHelpers.InBetween(0, 63);
                Assert.AreEqual(expected, actual, "Should return correct value for a1-h8");
            }

            [Test]
            public static void InBetween_ShouldReturnCorrectValue_GivenH8A1()
            {
                var expected = 0x40201008040200;
                var actual = BoardHelpers.InBetween(63, 0);
                Assert.AreEqual(expected, actual, "Should return correct value for h8-a1");
            }

            [Test]
            public static void InBetween_ShouldReturnZero_GivenTwoEWOneAnother()
            {
                var expected = (ulong)0x00;
                var actual = BoardHelpers.InBetween(3, 4);
                Assert.AreEqual(expected, actual,
                    "Should not be any squares between squares E+W of one another (d1-e1)");
            }

            [Test]
            public static void InBetween_ShouldReturnZero_GivenTwoNSOneAnother()
            {
                var expected = (ulong)0x00;
                var actual = BoardHelpers.InBetween(1, 9);
                Assert.AreEqual(expected, actual,
                    "Should not be any squares between squares N+S of ona another (b1-b2)");
            }

            [Test]
            public static void InBetween_ShouldReturnCorrectValue_GivenRandomPositions()
            {
                var expected = (ulong)0x00;
                var actual = BoardHelpers.InBetween(3, 8);
                Assert.AreEqual(expected, actual,
                    $"Should not be any squares between {3.ToSquareString()}{8.ToSquareString()}");

                expected = 0x200;
                actual = BoardHelpers.InBetween(16, 2);
                Assert.AreEqual(expected, actual,
                    $"Did not return correct value for in between {16.ToSquareString()} - {2.ToSquareString()}");

                expected = 0x2040800000000;
                actual = BoardHelpers.InBetween(28, 56);
                Assert.AreEqual(expected, actual,
                    $"Did not return correct value for in between {28.ToSquareString()} - {56.ToSquareString()}");
            }
        }
    }
}