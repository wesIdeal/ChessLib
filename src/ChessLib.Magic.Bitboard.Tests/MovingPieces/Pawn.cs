#region

using System;
using System.Collections.Generic;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;
using EnumsNET;
using NUnit.Framework;

#endregion

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{
    [TestFixture]
    public class Pawn
    {
        private readonly Bitboard _bitBoard = Bitboard.Instance;


        [TestCaseSource(nameof(GetAllPawnGetTestCases))]
        public void TestAllPawnAttacksAndMoves(MoveTestCase testCases)
        {
            var actualMoves = _bitBoard.GetPseudoLegalMoves(testCases.SquareIndex, Piece.Pawn,
                testCases.Color, testCases.Occupancy);
            Assert.AreEqual(testCases.Expected, actualMoves, testCases.ToString());
        }

        public static IEnumerable<MoveTestCase> GetAllPawnGetTestCases()
        {
            var testCases = new List<MoveTestCase>();
            foreach (var color in Enums.GetValues<Color>())
            {
                for (ushort squareIndex = 8; squareIndex < 56; squareIndex++)
                {
                    testCases.AddRange(GetTestCases(color, squareIndex));
                }
            }

            return testCases;
        }


        private static IEnumerable<MoveTestCase> GetTestCases(Color color, ushort squareIndex)
        {
            var moveSet = GetPawnShift(squareIndex, color);
            var setSquares = MovingPieceService.GetSetBits(moveSet);
            var permutations = MovingPieceService.GetAllPermutationsOfSetBits(setSquares, 0, 0);
            var testCases = new List<MoveTestCase>();
            foreach (var block in permutations)
            {
                var obstructionBoards = GetMovesFromObstructions(color, block, squareIndex);

                testCases.Add(new MoveTestCase(squareIndex, color, 0, obstructionBoards.Occupancy,
                    obstructionBoards.MoveBoard));
            }

            return testCases;
        }

        private static MoveObstructionBoard GetMovesFromObstructions(Color color, ulong blockers, ushort squareIndex)
        {
            ulong moves = 0;
            var sqValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            var shiftN = GetShift(color)(sqValue);
            var shiftDoubleN = IsStartingRank(color, squareIndex) ? GetDoubleShift(color)(sqValue) : (ulong?) null;
            var attacks = GetAllCapturesFromSquare(squareIndex, color);
            if ((shiftN & blockers) == 0)
            {
                moves |= shiftN;
                if (shiftDoubleN.HasValue && (shiftDoubleN.Value & blockers) == 0)
                {
                    moves |= shiftDoubleN.Value;
                }
            }

            moves |= attacks & blockers;
            return new MoveObstructionBoard(blockers, moves);
        }


        private static bool IsStartingRank(Color color, ushort squareIndex)
        {
            var rank = MovingPieceService.RankFromIdx(squareIndex);
            if (color == Color.White)
            {
                return rank == 1;
            }

            return rank == 6;
        }


        private static ulong GetPawnShift(ushort squareIndex, Color color)
        {
            var startingRank = MovingPieceService.GetPawnStartRankMask(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            var shift1XMethod = GetShift(color);
            var shift2XMethod = GetDoubleShift(color);
            return ((startingRank & squareValue) != 0 ? shift2XMethod(squareValue) : 0) | shift1XMethod(squareValue);
        }

        private static Func<ulong, ulong> GetRelativeNEShift(Color color)
        {
            if (color == Color.Black)
            {
                return MovingPieceService.ShiftSW;
            }

            return MovingPieceService.ShiftNE;
        }

        private static Func<ulong, ulong> GetRelativeNWShift(Color color)
        {
            if (color == Color.Black)
            {
                return MovingPieceService.ShiftSE;
            }

            return MovingPieceService.ShiftNW;
        }


        private static ulong GetAllCapturesFromSquare(ushort squareIndex, Color color)
        {
            var shiftNE = GetRelativeNEShift(color);
            var shiftNW = GetRelativeNWShift(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            return shiftNE(squareValue) | shiftNW(squareValue);
        }


        private static Func<ulong, ulong> GetDoubleShift(Color color)
        {
            if (color == Color.Black)
            {
                return MovingPieceService.Shift2S;
            }

            return MovingPieceService.Shift2N;
        }

        private static Func<ulong, ulong> GetShift(Color color)
        {
            if (color == Color.Black)
            {
                return MovingPieceService.ShiftS;
            }

            return MovingPieceService.ShiftN;
        }
    }
}