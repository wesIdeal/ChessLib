using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data.Magic.Init;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{

    [TestFixture]
    public class Bishop
    {
        private static Bitboard _b = Bitboard.Instance;
        private const bool UseRandom = true;
        private static int SquaresInTestCase = 64;
        private static int BoardsInTestCase = 100;
        protected static IEnumerable<ushort> AllSquares => Enumerable.Range(0, 64).Select(x => (ushort)x);

        [TestCaseSource(nameof(GetBishopTestCases), new object[] { UseRandom })]
        public static void TestBishopMoves(MoveTestCase testCase)
        {
            var actual = _b.GetPseudoLegalMoves(testCase.SquareIndex, Piece.Bishop, testCase.Color, testCase.Occupancy);
            Assert.AreEqual(testCase.Expected, actual, testCase.ToString());
        }

        public static IEnumerable<MoveTestCase> GetBishopTestCases(bool useRandom)
        {

            var squares = useRandom ? GetRandomSquares() : AllSquares;
            Console.WriteLine("Received Random Numbers");
            foreach (var square in squares)
            {
                MoveObstructionBoard[] blockerBoardSet = _b.Bishop.BlockerBoards[square];
                var boards = useRandom ? GetRandomBlockerBoards(blockerBoardSet) : blockerBoardSet;
                foreach (var blockerBoard in boards)
                {
                    yield return new MoveTestCase(square, Color.Black, blockerBoard.Occupancy,
                        blockerBoard.Occupancy, blockerBoard.MoveBoard);
                }
            }
        }
        private static readonly Bitboard BitBoard = Bitboard.Instance;



        private static IEnumerable<MoveObstructionBoard> GetRandomBlockerBoards(MoveObstructionBoard[] blockerBoardSet)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var blockerBoardCount = blockerBoardSet.Length;
            var boards = new List<MoveObstructionBoard>();
            for (var i = 0; i < BoardsInTestCase; i++)
            {
                var randomBoardIndex = (ushort)random.Next(0, blockerBoardCount);
                boards.Add(blockerBoardSet[randomBoardIndex]);
            }

            return boards.Distinct();
        }


        private static IEnumerable<ushort> GetRandomSquares()
        {
            var random = new Random(DateTime.Now.Millisecond);
            for (var i = 0; i < SquaresInTestCase; i++)
            {
                yield return (ushort)random.Next(0, 64);
            }
        }

    }
}
