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
    public class Rook
    {
        private const bool UseRandom = true;
        private static int SquaresInTestCase = 64;
        private static int BoardsInTestCase = 100;
        protected static IEnumerable<ushort> AllSquares => Enumerable.Range(0, 64).Select(x => (ushort)x);

        //[TestCaseSource(nameof(GetRookTestCases), new object[] { UseRandom })]
        //public static void TestRookMoves(MoveTestCase testCase)
        //{
        //    var actual = BitBoard.GetMoves(testCase.SquareIndex, Piece.Rook, testCase.Color, testCase.PlayerBlocker,
        //        testCase.OpponentBlocker);
        //    Assert.AreEqual(testCase.Expected, actual, testCase.ToString());
        //}
        private static readonly Bitboard BitBoard = Bitboard.Instance;

        public static IEnumerable<MoveTestCase> GetRookTestCases(bool useRandom)
        {

            var squares = useRandom ? GetRandomSquares() : AllSquares;
            Console.WriteLine("Received Random Numbers");
            foreach (var square in squares)
            {
                MoveObstructionBoard[] blockerBoardSet = BitBoard.Rook.BlockerBoards[square];
                var boards = useRandom ? GetRandomBlockerBoards(blockerBoardSet) : blockerBoardSet;
                foreach (var blockerBoard in boards)
                {
                    yield return new MoveTestCase(square, Color.Black, blockerBoard.Occupancy,
                        blockerBoard.Occupancy, blockerBoard.MoveBoard);
                }
            }
        }

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


        [TestCaseSource(nameof(AllSquares))]
        public void RookMovesShouldBeInitialized(ushort squareIndex)
        {
            var rank = MovingPieceService.RankFromIdx(squareIndex);
            var file = MovingPieceService.FileFromIdx(squareIndex);
            var rankFill = ((ulong)0xff << (rank * 8));
            var fileFill = ((ulong)0x101010101010101 << file);
            var boardVal = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            
            var mask = (rankFill | fileFill) ^ boardVal;
            var actual = BitBoard.GetMoves(squareIndex, Piece.Rook, Color.Black, 0, 0);
            Assert.AreEqual(mask, actual);
        }
    }

   
}
