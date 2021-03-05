using System.Collections.Generic;
using ChessLib.Data.Types.Enums;
using NUnit.Framework;

namespace ChessLib.BoardService.Tests.BoardRepresentation
{
    using BoardUnderTest = ChessLib.BoardService.BoardRepresentation.Board;
    [TestFixture]
    public class Board
    {
        private static BoardUnderTest board = new BoardUnderTest();

        [TestCaseSource(nameof(GetOccupancyTestCases))]
        public static void TestStandardBoardInitialization_White_Pawn(OccupancyTestCase testCase)
        {
            Assert.AreEqual(testCase.ExpectedPlacement, board.Occupancy[(int)testCase.Color][(int)testCase.Piece]);
        }

        public static IEnumerable<OccupancyTestCase> GetOccupancyTestCases()
        {
            yield return new OccupancyTestCase() { ExpectedPlacement = 0xff00, Piece = Piece.Pawn, Color = Color.White };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul << 1) | (1ul << 6), Piece = Piece.Knight, Color = Color.White };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul << 2) | (1ul << 5), Piece = Piece.Bishop, Color = Color.White };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul << 0) | (1ul << 7), Piece = Piece.Rook, Color = Color.White };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul << 3), Piece = Piece.Queen, Color = Color.White };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul<< 4), Piece = Piece.King, Color = Color.White };

            yield return new OccupancyTestCase() { ExpectedPlacement = 0xff000000000000, Piece = Piece.Pawn, Color = Color.Black };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul << 57) | (1ul << 62), Piece = Piece.Knight, Color = Color.Black };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul << 58) | (1ul << 61), Piece = Piece.Bishop, Color = Color.Black };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul << 56) | (1ul << 63), Piece = Piece.Rook, Color = Color.Black };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul << 59), Piece = Piece.Queen, Color = Color.Black };
            yield return new OccupancyTestCase() { ExpectedPlacement = (1ul << 60), Piece = Piece.King, Color = Color.Black };

        }
    }

    public class OccupancyTestCase
    {
        public Color Color { get; set; }
        public Piece Piece { get; set; }

        public ulong ExpectedPlacement { get; set; }
    }
}