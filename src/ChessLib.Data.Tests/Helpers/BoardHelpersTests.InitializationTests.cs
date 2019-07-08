using ChessLib.Data.Helpers;
using NUnit.Framework;
using System;
using System.Text;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Tests.Helpers
{
    [TestFixture]
    public partial class BoardHelpersTests
    {
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
                Assert.AreEqual(0x101010101010101, BoardHelpers.FileMasks[0], "'A' File Mask not initialized properly.");
                Assert.AreEqual(0x202020202020202, BoardHelpers.FileMasks[1], "'B' File Mask not initialized properly.");
                Assert.AreEqual(0x404040404040404, BoardHelpers.FileMasks[2], "'C' File Mask not initialized properly.");
                Assert.AreEqual(0x808080808080808, BoardHelpers.FileMasks[3], "'D' File Mask not initialized properly.");
                Assert.AreEqual(0x1010101010101010, BoardHelpers.FileMasks[4], "'E' File Mask not initialized properly.");
                Assert.AreEqual(0x2020202020202020, BoardHelpers.FileMasks[5], "'F' File Mask not initialized properly.");
                Assert.AreEqual(0x4040404040404040, BoardHelpers.FileMasks[6], "'G' File Mask not initialized properly.");
                Assert.AreEqual(0x8080808080808080, BoardHelpers.FileMasks[7], "'H' File Mask not initialized properly.");
            }

            [Test]
            public static void InitializeRankMasks_RankMasksProperlyInitialized()
            {
                Assert.AreEqual(0xff, BoardHelpers.RankMasks[0], "Rank 1 Mask not initialized properly.");
                Assert.AreEqual(0xff00, BoardHelpers.RankMasks[1], "Rank 2 Mask not initialized properly.");
                Assert.AreEqual(0xff0000, BoardHelpers.RankMasks[2], "Rank 3 Mask not initialized properly.");
                Assert.AreEqual(0xff000000, BoardHelpers.RankMasks[3], "Rank 4 Mask not initialized properly.");
                Assert.AreEqual(0xff00000000, BoardHelpers.RankMasks[4], "Rank 5 Mask not initialized properly.");
                Assert.AreEqual(0xff0000000000, BoardHelpers.RankMasks[5], "Rank 6 Mask not initialized properly.");
                Assert.AreEqual(0xff000000000000, BoardHelpers.RankMasks[6], "Rank 7 Mask not initialized properly.");
                Assert.AreEqual(0xff00000000000000, BoardHelpers.RankMasks[7], "Rank 8 Mask not initialized properly.");
            }

            [Test]
            public static void IndividualSquareValidityTest()
            {
                Assert.AreEqual(0x0000000000000001, BoardHelpers.IndividualSquares[0]);
                Assert.AreEqual(0x0000000000000002, BoardHelpers.IndividualSquares[1]);
                Assert.AreEqual(0x0000000000000004, BoardHelpers.IndividualSquares[2]);
                Assert.AreEqual(0x0000000000000008, BoardHelpers.IndividualSquares[3]);
                Assert.AreEqual(0x0000000000000010, BoardHelpers.IndividualSquares[4]);
                Assert.AreEqual(0x0000000000000020, BoardHelpers.IndividualSquares[5]);
                Assert.AreEqual(0x0000000000000040, BoardHelpers.IndividualSquares[6]);
                Assert.AreEqual(0x0000000000000080, BoardHelpers.IndividualSquares[7]);
                Assert.AreEqual(0x0000000000000100, BoardHelpers.IndividualSquares[8]);
                Assert.AreEqual(0x0000000000000200, BoardHelpers.IndividualSquares[9]);
                Assert.AreEqual(0x0000000000000400, BoardHelpers.IndividualSquares[10]);
                Assert.AreEqual(0x0000000000000800, BoardHelpers.IndividualSquares[11]);
                Assert.AreEqual(0x0000000000001000, BoardHelpers.IndividualSquares[12]);
                Assert.AreEqual(0x0000000000002000, BoardHelpers.IndividualSquares[13]);
                Assert.AreEqual(0x0000000000004000, BoardHelpers.IndividualSquares[14]);
                Assert.AreEqual(0x0000000000008000, BoardHelpers.IndividualSquares[15]);
                Assert.AreEqual(0x0000000000010000, BoardHelpers.IndividualSquares[16]);
                Assert.AreEqual(0x0000000000020000, BoardHelpers.IndividualSquares[17]);
                Assert.AreEqual(0x0000000000040000, BoardHelpers.IndividualSquares[18]);
                Assert.AreEqual(0x0000000000080000, BoardHelpers.IndividualSquares[19]);
                Assert.AreEqual(0x0000000000100000, BoardHelpers.IndividualSquares[20]);
                Assert.AreEqual(0x0000000000200000, BoardHelpers.IndividualSquares[21]);
                Assert.AreEqual(0x0000000000400000, BoardHelpers.IndividualSquares[22]);
                Assert.AreEqual(0x0000000000800000, BoardHelpers.IndividualSquares[23]);
                Assert.AreEqual(0x0000000001000000, BoardHelpers.IndividualSquares[24]);
                Assert.AreEqual(0x0000000002000000, BoardHelpers.IndividualSquares[25]);
                Assert.AreEqual(0x0000000004000000, BoardHelpers.IndividualSquares[26]);
                Assert.AreEqual(0x0000000008000000, BoardHelpers.IndividualSquares[27]);
                Assert.AreEqual(0x0000000010000000, BoardHelpers.IndividualSquares[28]);
                Assert.AreEqual(0x0000000020000000, BoardHelpers.IndividualSquares[29]);
                Assert.AreEqual(0x0000000040000000, BoardHelpers.IndividualSquares[30]);
                Assert.AreEqual(0x0000000080000000, BoardHelpers.IndividualSquares[31]);
                Assert.AreEqual(0x0000000100000000, BoardHelpers.IndividualSquares[32]);
                Assert.AreEqual(0x0000000200000000, BoardHelpers.IndividualSquares[33]);
                Assert.AreEqual(0x0000000400000000, BoardHelpers.IndividualSquares[34]);
                Assert.AreEqual(0x0000000800000000, BoardHelpers.IndividualSquares[35]);
                Assert.AreEqual(0x0000001000000000, BoardHelpers.IndividualSquares[36]);
                Assert.AreEqual(0x0000002000000000, BoardHelpers.IndividualSquares[37]);
                Assert.AreEqual(0x0000004000000000, BoardHelpers.IndividualSquares[38]);
                Assert.AreEqual(0x0000008000000000, BoardHelpers.IndividualSquares[39]);
                Assert.AreEqual(0x0000010000000000, BoardHelpers.IndividualSquares[40]);
                Assert.AreEqual(0x0000020000000000, BoardHelpers.IndividualSquares[41]);
                Assert.AreEqual(0x0000040000000000, BoardHelpers.IndividualSquares[42]);
                Assert.AreEqual(0x0000080000000000, BoardHelpers.IndividualSquares[43]);
                Assert.AreEqual(0x0000100000000000, BoardHelpers.IndividualSquares[44]);
                Assert.AreEqual(0x0000200000000000, BoardHelpers.IndividualSquares[45]);
                Assert.AreEqual(0x0000400000000000, BoardHelpers.IndividualSquares[46]);
                Assert.AreEqual(0x0000800000000000, BoardHelpers.IndividualSquares[47]);
                Assert.AreEqual(0x0001000000000000, BoardHelpers.IndividualSquares[48]);
                Assert.AreEqual(0x0002000000000000, BoardHelpers.IndividualSquares[49]);
                Assert.AreEqual(0x0004000000000000, BoardHelpers.IndividualSquares[50]);
                Assert.AreEqual(0x0008000000000000, BoardHelpers.IndividualSquares[51]);
                Assert.AreEqual(0x0010000000000000, BoardHelpers.IndividualSquares[52]);
                Assert.AreEqual(0x0020000000000000, BoardHelpers.IndividualSquares[53]);
                Assert.AreEqual(0x0040000000000000, BoardHelpers.IndividualSquares[54]);
                Assert.AreEqual(0x0080000000000000, BoardHelpers.IndividualSquares[55]);
                Assert.AreEqual(0x0100000000000000, BoardHelpers.IndividualSquares[56]);
                Assert.AreEqual(0x0200000000000000, BoardHelpers.IndividualSquares[57]);
                Assert.AreEqual(0x0400000000000000, BoardHelpers.IndividualSquares[58]);
                Assert.AreEqual(0x0800000000000000, BoardHelpers.IndividualSquares[59]);
                Assert.AreEqual(0x1000000000000000, BoardHelpers.IndividualSquares[60]);
                Assert.AreEqual(0x2000000000000000, BoardHelpers.IndividualSquares[61]);
                Assert.AreEqual(0x4000000000000000, BoardHelpers.IndividualSquares[62]);
                Assert.AreEqual(0x8000000000000000, BoardHelpers.IndividualSquares[63]);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 64; i++)
                {
                    sb.Append(
                        $"0x{Convert.ToString((long)BoardHelpers.IndividualSquares[i], 16)}, {(i != 0 && i % 7 == 0 ? "\r\n" : "")}");
                }
                Console.WriteLine(sb.ToString());
            }

            [Test]
            public static void GetFile_ShouldReturnCorrectFile()
            {
                for (int i = 0; i < 64; i++)
                {
                    var expected = (File)(i % 8);
                    Assert.AreEqual(expected, i.GetFile());
                }
            }
            [Test]
            public static void GetRank_ShouldReturnCorrectRank()
            {
                for (int i = 0; i < 64; i++)
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
                    Assert.AreEqual(c, f.ToInt());
                    c++;
                }
            }

            [Test]
            public static void RankToIntFunctionality_ShouldConvertRankEnumValueToInt()
            {
                var c = 0;
                foreach (var r in (Rank[])Enum.GetValues(typeof(Rank)))
                {
                    Assert.AreEqual(c, r.ToInt(), $"Rank {r.ToString()} should convert to {c}");
                    c++;
                }
            }

            [Test]
            public static void ToHexDisplay_ShouldGiveProperHexStringRepresentation()
            {
                var expectedPad2 = "0x10";
                var expectedPad4 = "0x0010";
                var expectedNoHexId = "10";
                Assert.AreEqual(expectedPad2, 0x10ul.ToHexDisplay(true, true, 2));
                Assert.AreEqual(expectedPad4, 0x10ul.ToHexDisplay(true, true, 4));
                Assert.AreEqual(expectedNoHexId, 0x10ul.ToHexDisplay(false));
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
                Assert.AreEqual(expected, actual, "Should not be any squares between squares E+W of ona another (d1-e1)");
            }

            [Test]
            public static void InBetween_ShouldReturnZero_GivenTwoNSOneAnother()
            {
                var expected = (ulong)0x00;
                var actual = BoardHelpers.InBetween(1, 9);
                Assert.AreEqual(expected, actual, "Should not be any squares between squares N+S of ona another (b1-b2)");
            }

            [Test]
            public static void InBetween_ShouldReturnCorrectValue_GivenRandomPositions()
            {
                var expected = (ulong)0x00;
                var actual = BoardHelpers.InBetween(3, 8);
                Assert.AreEqual(expected, actual, $"Should not be any squares between {3.IndexToSquareDisplay()}{8.IndexToSquareDisplay()}");

                expected = 0x200;
                actual = BoardHelpers.InBetween(16, 2);
                Assert.AreEqual(expected, actual, $"Did not return correct value for in between {16.IndexToSquareDisplay()} - {2.IndexToSquareDisplay()}");

                expected = 0x2040800000000;
                actual = BoardHelpers.InBetween(28, 56);
                Assert.AreEqual(expected, actual, $"Did not return correct value for in between {28.IndexToSquareDisplay()} - {56.IndexToSquareDisplay()}");

            }

        }

    }
}
