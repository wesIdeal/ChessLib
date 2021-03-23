#region

using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;
using ChessLib.Types.Enums;
using NUnit.Framework;

#endregion

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{
    [TestFixture]
    public class Rook
    {
        private const bool UseRandom = true;
        private static readonly int SquaresInTestCase = 64;
        private static readonly int BoardsInTestCase = 20;
        protected static IEnumerable<ushort> AllSquares => Enumerable.Range(0, 64).Select(x => (ushort)x);

        [TestCaseSource(nameof(GetRookTestCases), new object[] { UseRandom })]
        public static void TestRookMoves(MoveTestCase testCase)
        {
            var actual = BitBoard.GetPseudoLegalMoves(testCase.SquareIndex, Piece.Rook, testCase.Color, testCase.OpponentObstructions);
            Assert.AreEqual(testCase.Expected, actual, testCase.ToString());
        }

        private static readonly Bitboard BitBoard = Bitboard.Instance;

        public static IEnumerable<MoveTestCase> GetRookTestCases(bool useRandom)
        {
            var squares = useRandom ? GetRandomSquares() : AllSquares;
            Console.WriteLine("Received Random Numbers");
            foreach (var square in squares)
            {
                var obstructionBoards = BitBoard.Rook.MoveObstructionBoards[square];
                var boards = useRandom ? GetRandomObstructionBoards(obstructionBoards) : obstructionBoards;
                foreach (var obstructionBoard in boards)
                {
                    yield return new MoveTestCase(square, Color.Black, obstructionBoard.Occupancy,
                        obstructionBoard.Occupancy, obstructionBoard.MoveBoard);
                }
            }
        }

        private static IEnumerable<MoveObstructionBoard> GetRandomObstructionBoards(
            MoveObstructionBoard[] obstructionBoards)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var obstructionBoardsLength = obstructionBoards.Length;
            var boards = new List<MoveObstructionBoard>();
            for (var i = 0; i < BoardsInTestCase; i++)
            {
                var randomBoardIndex = (ushort)random.Next(0, obstructionBoardsLength);
                boards.Add(obstructionBoards[randomBoardIndex]);
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
            var rankFill = (ulong)0xff << (rank * 8);
            var fileFill = (ulong)0x101010101010101 << file;
            var boardVal = MovingPieceService.GetBoardValueOfIndex(squareIndex);

            var mask = (rankFill | fileFill) ^ boardVal;
            var actual = BitBoard.GetPseudoLegalMoves(squareIndex, Piece.Rook, Color.Black, 0);
            Assert.AreEqual(mask, actual);
        }
    }
}