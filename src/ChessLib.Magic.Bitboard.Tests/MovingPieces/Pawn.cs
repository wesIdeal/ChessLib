using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using EnumsNET;
using NUnit.Framework;

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{
    [TestFixture]
    public class Pawn
    {
        private readonly Bitboard bb = Bitboard.Instance;
        private static MovingPieceService movingPieceService = new MovingPieceService();
        private static readonly IEnumerable<MoveTestCase> TestCases = InitializeTestCases();

        private static IEnumerable<MoveTestCase> GetWhitePawnTestCases =>
            TestCases.Where(x => x.Color == Color.White).OrderBy(x => x.SquareIndex);

        private static IEnumerable<MoveTestCase> GetBlackPawnTestCases =>
            TestCases.Where(x => x.Color == Color.Black).OrderByDescending(x => x.SquareIndex);

        [Test]
        [TestCaseSource(nameof(GetWhitePawnTestCases))]
        public void TestWhitePawnMobility(MoveTestCase testCases)
        {
            var actualMoveset = bb.GetMoves(testCases.SquareIndex, Piece.Pawn,
                testCases.Color, testCases.PlayerBlocker, testCases.OpponentBlocker);
            Assert.AreEqual(testCases.Expected, actualMoveset, testCases.ToString());
        }

        [Test]
        [TestCaseSource(nameof(GetBlackPawnTestCases))]
        public void TestBlackPawnMobility(MoveTestCase testCases)
        {
            var actualMoveset = bb.GetMoves(testCases.SquareIndex, Piece.Pawn,
                testCases.Color, testCases.PlayerBlocker, testCases.OpponentBlocker);
            Assert.AreEqual(testCases.Expected, actualMoveset, testCases.ToString());
        }

        [Test(Description = "Capture validity tests")]
        [TestCaseSource(nameof(GetPlayerBlockersInAttackSquareCases))]
        public void ShouldNotCaptureOwnPieces(MoveTestCase testCase)
        {
            var actualMoveset = bb.GetMoves(testCase.SquareIndex, Piece.Pawn,
                testCase.Color, testCase.PlayerBlocker, testCase.OpponentBlocker);
            Assert.AreEqual(testCase.Expected, actualMoveset, testCase.ToString());
        }


        private static IEnumerable<MoveTestCase> InitializeTestCases()
        {
            var testCases = new List<MoveTestCase>();
            foreach (var color in Enums.GetValues<Color>())
            {
                for (ushort squareIndex = 8; squareIndex < 56; squareIndex++)
                {
                    //Completely blocked, one piece
                    testCases.AddRange(AllMovesBlockedByOnePiece(squareIndex, color));
                    //Completely blocked, two pieces
                    testCases.AddRange(GetDoublePieceMoveBlock(squareIndex, color));
                    //Blocked from double-square push
                    testCases.AddRange(GetDoubleSquarePushBlocker(squareIndex, color));
                    //Unblocked
                    testCases.Add(GetBaseTestCase(squareIndex, color));

                    //Blocked with a    vailable capture North East
                    testCases.AddRange(GetBlockedWithAvailableCaptureNE(squareIndex, color));
                    //Blocked with available capture North West
                    testCases.AddRange(GetBlockedWithAvailableCaptureNW(squareIndex, color));
                    //Blocked with all available captures
                    testCases.AddRange(GetBlockedWithAvailableCaptures(squareIndex, color));

                    //Blocked from double-square push, capture North East
                    testCases.AddRange(GetDoubleSquarePushBlockerWithNECaptures(squareIndex, color));
                    //Blocked from double-square push, capture North West
                    testCases.AddRange(GetDoubleSquarePushBlockerWithNWCaptures(squareIndex, color));
                    //Blocked from double-square push, all available captures 
                    testCases.AddRange(GetDoubleSquarePushBlockerWithAllCaptures(squareIndex, color));

                    //Blocked from double-square push, capture North East
                    testCases.Add(GetUnblockedWithNECaptures(squareIndex, color));
                    //Blocked from double-square push, capture North West
                    testCases.Add(GetUnblockedWithNWCaptures(squareIndex, color));
                    //Blocked from double-square push, all available captures 
                    testCases.Add(GetUnblockedWithAllCaptures(squareIndex, color));
                }
            }

            return testCases;
        }

        /// <summary>
        ///     Get test cases to validate inability to capture own pieces
        /// </summary>
        private static IEnumerable<MoveTestCase> GetPlayerBlockersInAttackSquareCases()
        {
            foreach (var color in Enums.GetValues<Color>())
            {
                for (ushort squareIndex = 8; squareIndex < 56; squareIndex++)
                {
                    var captures = GetAllCapturesFromSquare(squareIndex, color);
                    var baseTest = GetBaseTestCase(squareIndex, color);
                    baseTest.PlayerBlocker |= captures;
                    yield return baseTest;
                }
            }
        }

        /// <summary>
        ///     Get test cases for moves / attacks with no blockers
        /// </summary>
        private static MoveTestCase GetUnblockedWithAllCaptures(ushort squareIndex, Color color)
        {
            var allCaptures = GetAllCapturesFromSquare(squareIndex, color);
            var unblockedTest = GetBaseTestCase(squareIndex, color);
            unblockedTest.OpponentBlocker |= allCaptures;
            unblockedTest.Expected |= allCaptures;
            return unblockedTest;
        }

        /// <summary>
        ///     Get test cases for all moves and relative NW captures
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private static MoveTestCase GetUnblockedWithNWCaptures(ushort squareIndex, Color color)
        {
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            var nwShift = GetRelativeNWShift(color)(squareValue);
            var unblockedTest = GetBaseTestCase(squareIndex, color);
            unblockedTest.OpponentBlocker |= nwShift;
            unblockedTest.Expected |= nwShift;
            return unblockedTest;
        }

        /// <summary>
        ///     Get test cases for all moves and relative NE captures
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private static MoveTestCase GetUnblockedWithNECaptures(ushort squareIndex, Color color)
        {
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            var neShift = GetRelativeNEShift(color)(squareValue);
            var unblockedTest = GetBaseTestCase(squareIndex, color);
            unblockedTest.OpponentBlocker |= neShift;
            unblockedTest.Expected |= neShift;
            return unblockedTest;
        }

        /// <summary>
        ///     Get moves when square 2 squares ahead is populated and all captures available
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private static IEnumerable<MoveTestCase> GetDoubleSquarePushBlockerWithAllCaptures(ushort squareIndex,
            Color color)
        {
            var testCases = GetDoubleSquarePushBlocker(squareIndex, color).ToArray();
            var ShiftNE = GetRelativeNEShift(color);
            var ShiftNW = GetRelativeNWShift(color);
            var ShiftN = GetShift(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            for (var i = 0; i < testCases.Length; i++)
            {
                var tc = testCases[i];
                var ShiftAllCaptures = ShiftNE(squareValue) | ShiftNW(squareValue);
                tc.OpponentBlocker |= ShiftAllCaptures;
                tc.Expected |= ShiftAllCaptures | ShiftN(squareValue);
                yield return tc;
            }
        }

        private static IEnumerable<MoveTestCase> GetDoubleSquarePushBlockerWithNWCaptures(ushort squareIndex,
            Color color)
        {
            var testCases = GetDoubleSquarePushBlocker(squareIndex, color).ToArray();
            var ShiftNW = GetRelativeNWShift(color);
            var ShiftN = GetShift(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            for (var i = 0; i < testCases.Length; i++)
            {
                var tc = testCases[i];
                var nwShift = ShiftNW(squareValue);
                var shifts = ShiftN(squareValue) | nwShift;
                tc.OpponentBlocker |= shifts;
                tc.Expected = nwShift;
                yield return tc;
            }
        }

        private static IEnumerable<MoveTestCase> GetDoubleSquarePushBlockerWithNECaptures(ushort squareIndex,
            Color color)
        {
            var testCases = GetDoubleSquarePushBlocker(squareIndex, color).ToArray();
            var ShiftNE = GetRelativeNEShift(color);
            var ShiftN = GetShift(color);
            var ShiftN2x = GetDoubleShift(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            for (var i = 0; i < testCases.Length; i++)
            {
                var tc = testCases[i];
                var neShift = ShiftNE(squareValue);
                tc.OpponentBlocker |= neShift;
                tc.Expected |= neShift | ShiftN(squareValue);
                yield return tc;
            }
        }

        private static IEnumerable<MoveTestCase> GetBlockedWithAvailableCaptures(ushort squareIndex, Color color)
        {
            var testCases = AllMovesBlockedByOnePiece(squareIndex, color).ToArray();
            var ShiftNE = GetRelativeNEShift(color);
            var ShiftNW = GetRelativeNWShift(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            for (var i = 0; i < testCases.Length; i++)
            {
                var tc = testCases[i];
                var neShift = ShiftNE(squareValue) | ShiftNW(squareValue);
                tc.OpponentBlocker |= neShift;
                tc.Expected |= neShift;
                yield return tc;
            }
        }

        private static IEnumerable<MoveTestCase> GetBlockedWithAvailableCaptureNE(ushort squareIndex, Color color)
        {
            var testCases = AllMovesBlockedByOnePiece(squareIndex, color).ToArray();
            var ShiftNE = GetRelativeNEShift(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            for (var i = 0; i < testCases.Length; i++)
            {
                var tc = testCases[i];
                var neShift = ShiftNE(squareValue);
                tc.OpponentBlocker |= neShift;
                tc.Expected |= neShift;
                yield return tc;
            }
        }

        private static IEnumerable<MoveTestCase> GetBlockedWithAvailableCaptureNW(ushort squareIndex, Color color)
        {
            var testCases = AllMovesBlockedByOnePiece(squareIndex, color).ToArray();
            var ShiftNE = GetRelativeNWShift(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            for (var i = 0; i < testCases.Length; i++)
            {
                var tc = testCases[i];
                var neShift = ShiftNE(squareValue);
                tc.OpponentBlocker |= neShift;
                tc.Expected |= neShift;
                yield return tc;
            }
        }

        private static MoveTestCase GetBaseTestCase(ushort squareIndex, Color color)
        {
            var blocker = (ulong) 0x00;
            var expected = GetPawnShift(squareIndex, color);
            return new MoveTestCase(squareIndex, color, blocker, 0, expected);
        }

        private static ulong GetPawnShift(ushort squareIndex, Color color)
        {
            var startingRank = MovingPieceService.GetPawnStartRankMask(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            var Shift1xMethod = GetShift(color);
            var Shift2xMethod = GetDoubleShift(color);
            return ((startingRank & squareValue) != 0 ? Shift2xMethod(squareValue) : 0) | Shift1xMethod(squareValue);
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

        private static IEnumerable<MoveTestCase> GetDoubleSquarePushBlocker(ushort squareIndex, Color color)
        {
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            var blocker = GetDoubleShift(color)(squareValue);
            var Shift1x = GetShift(color);
            var expected = Shift1x(squareValue);
            return GetBlockingTestCasesForEachSide(squareIndex, color, blocker, expected);
        }

        private static IEnumerable<MoveTestCase> AllMovesBlockedByOnePiece(ushort squareIndex, Color color)
        {
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            var ShiftRelativeNorth = GetShift(color)(squareValue);
            ulong expected = 0;
            return GetBlockingTestCasesForEachSide(squareIndex, color, ShiftRelativeNorth, expected);
        }


        /// <summary>
        ///     Get Blockers directly in front of square and in front of that.
        /// </summary>
        private static IEnumerable<MoveTestCase> GetDoublePieceMoveBlock(ushort squareIndex, Color color)
        {
            var blocker = GetAllPossibleBlockersFromSquare(squareIndex, color);
            return GetBlockingTestCasesForEachSide(squareIndex, color, blocker, 0);
        }

        /// <summary>
        ///     Gets Blockers from both opponent blocks and player blocks
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="color"></param>
        /// <param name="blocker"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        private static MoveTestCase[] GetBlockingTestCasesForEachSide(ushort squareIndex, Color color, ulong blocker,
            ulong expected)
        {
            return new[]
            {
                new MoveTestCase(squareIndex, color, 0, blocker, expected),
                new MoveTestCase(squareIndex, color, blocker, 0, expected)
            };
        }

        public static ulong GetAllCapturesFromSquare(ushort squareIndex, Color color)
        {
            var ShiftNE = GetRelativeNEShift(color);
            var ShiftNW = GetRelativeNWShift(color);
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            return ShiftNE(squareValue) | ShiftNW(squareValue);
        }

        private static ulong GetAllPossibleBlockersFromSquare(ushort squareIndex, Color color)
        {
            var squareValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            return GetShift(color)(squareValue) | GetDoubleShift(color)(squareValue);
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