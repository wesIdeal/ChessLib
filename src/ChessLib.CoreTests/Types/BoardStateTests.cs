using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using EnumsNET;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Types
{
    [TestFixture(TestOf = typeof(BoardState), Category = "Base Functionality")]
    [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
    public class BoardStateTests
    {
        [TestCaseSource(nameof(GetPieceCapturedTestCases))]
        public void PieceCapturedTests(TestCase<bool, Piece?> testCase)
        {
            var boardState = new BoardState {ActivePlayer = Color.Black};
            if (!testCase.ExpectedValue)
            {
                Assert.Throws(typeof(ArgumentException),
                    delegate { boardState.PieceCaptured = testCase.TestMethodInputValue.Value; },
                    "King should throw exception.");
            }

            else
            {
                boardState.PieceCaptured = testCase.TestMethodInputValue;
                Assert.AreEqual(testCase.TestMethodInputValue, boardState.PieceCaptured);
            }
        }

        [Test]
        public void IsEndOfGame_WhenStalemate_ReturnsTrue()
        {
            var board = new Mock<BoardState>();
            board.SetupGet(t => t.GameState).Returns(GameState.StaleMate);
            Assert.IsTrue(board.Object.IsEndOfGame);
        }

        [Test]
        public void IsEndOfGame_WhenCheckmate_ReturnsTrue()
        {
            var board = new Mock<BoardState>();
            board.SetupGet(t => t.GameState).Returns(GameState.Checkmate);
            Assert.IsTrue(board.Object.IsEndOfGame);
        }

        [Test]
        public void IsEndOfGame_WhenNotStalemateOrCheckmate_ReturnsFalse()
        {
            var board = new Mock<BoardState>();
            board.SetupGet(t => t.GameState).Returns(GameState.None);
            Assert.IsFalse(board.Object.IsEndOfGame);
        }

        protected static IEnumerable<TestCase<bool, Piece?>> GetPieceCapturedTestCases()
        {
            foreach (var piece in Enums.GetValues<Piece>())
            {
                yield return new TestCase<bool, Piece?>(piece != Piece.King, piece);
            }

            yield return new TestCase<bool, Piece?>(true, null);
        }

        [TestCaseSource(nameof(GetGameStateTestCases))]
        public void GetGameStateTest(TestCase<bool, GameState> testCase)
        {
            var boardState = new BoardState {GameState = testCase.TestMethodInputValue};
            Assert.AreEqual(testCase.TestMethodInputValue, boardState.GameState, testCase.ToString());
        }

        protected static IEnumerable<TestCase<bool, GameState>> GetGameStateTestCases()
        {
            foreach (var gameState in Enums.GetValues<GameState>())
            {
                yield return new TestCase<bool, GameState>(true, gameState, gameState.AsString());
            }
        }

        [TestCaseSource(nameof(GetCastlingAbilityTestCases))]
        public void GetCastlingAbilityTest(TestCase<CastlingAvailability, CastlingAvailability> testCase)
        {
            var boardState = new BoardState {CastlingAvailability = testCase.TestMethodInputValue};
            Assert.AreEqual(testCase.ExpectedValue, boardState.CastlingAvailability);
        }

        protected static IEnumerable<TestCase<CastlingAvailability, CastlingAvailability>> GetCastlingAbilityTestCases()
        {
            var max = 15ul;

            var permutations = MovingPieceService.GetAllPermutationsOfSetBits(max.GetSetBits(), 0, 0);
            var castlingCartesian = permutations.Distinct()
                .Select(x => (CastlingAvailability) x)
                .Select(x =>
                    new TestCase<CastlingAvailability, CastlingAvailability>(x,
                        x, $"{FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(x)}")).ToArray();
            return castlingCartesian;
        }

        [TestCaseSource(nameof(GetHalfMoveClockTestCases))]
        public byte TestHalfMoveClockTestCases(int testCase)
        {
            var boardState = new BoardState {HalfMoveClock = (byte)testCase};
            return boardState.HalfMoveClock;
        }

        protected static IEnumerable GetHalfMoveClockTestCases()
        {
            foreach(var tc in Enumerable.Range(0, byte.MaxValue + 2))
            {
                yield return new TestCaseData(tc).Returns((byte)tc).SetDescription(tc.ToSquareString());
            }
               

        }

        [TestCaseSource(nameof(GetFullMoveCounterTestCases))]
        public void FullMoveCounterTest(TestCase<bool, uint> testCase)
        {
            var boardState = new BoardState();
            if (!testCase.ExpectedValue)
            {
                Assert.Throws(typeof(FullMoveCountExceededException),
                    delegate { boardState.FullMoveCounter = testCase.TestMethodInputValue; },
                    string.Format(FullMoveCountExceededException.MessageFormat, testCase.TestMethodInputValue));
            }
            else
            {
                boardState.FullMoveCounter = testCase.TestMethodInputValue;
                Assert.AreEqual(testCase.TestMethodInputValue, boardState.FullMoveCounter,
                    testCase.TestMethodInputValue.ToString());
            }
        }

        protected static IEnumerable<TestCase<bool, uint>> GetFullMoveCounterTestCases()
        {
            return Enumerable.Range(0, 513).Select(x => new TestCase<bool, uint>(x < 512, (uint) x));
        }
    }
}