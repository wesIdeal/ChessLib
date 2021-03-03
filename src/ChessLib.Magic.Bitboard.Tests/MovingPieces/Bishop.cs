#region

using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Storage;
using NUnit.Framework;

#endregion

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{
    [TestFixture]
    public class Bishop
    {
        private static readonly Bitboard _b = Bitboard.Instance;
        private const bool UseRandom = true;
        private static readonly int SquaresInTestCase = 64;
        private static readonly int BoardsInTestCase = 100;
        protected static IEnumerable<ushort> AllSquares => Enumerable.Range(0, 64).Select(x => (ushort) x);

        [TestCaseSource(nameof(GetBishopTestCases), new object[] {UseRandom})]
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
                var obstructionBoards = _b.Bishop.MoveObstructionBoards[square];
                var randomObstructionBoards = useRandom ? GetRandomObstructionBoards(obstructionBoards) : obstructionBoards;
                foreach (var obstructionBoard in randomObstructionBoards)
                {
                    yield return new MoveTestCase(square, Color.Black, obstructionBoard.Occupancy,
                        obstructionBoard.Occupancy, obstructionBoard.MoveBoard);
                }
            }
        }

        private static readonly Bitboard BitBoard = Bitboard.Instance;


        private static IEnumerable<MoveObstructionBoard> GetRandomObstructionBoards(
            MoveObstructionBoard[] obstructionBoardSet)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var obstructionBoardCount = obstructionBoardSet.Length;
            var boards = new List<MoveObstructionBoard>();
            for (var i = 0; i < BoardsInTestCase; i++)
            {
                var randomBoardIndex = (ushort) random.Next(0, obstructionBoardCount);
                boards.Add(obstructionBoardSet[randomBoardIndex]);
            }

            return boards.Distinct();
        }


        private static IEnumerable<ushort> GetRandomSquares()
        {
            var random = new Random(DateTime.Now.Millisecond);
            for (var i = 0; i < SquaresInTestCase; i++)
            {
                yield return (ushort) random.Next(0, 64);
            }
        }
    }
}