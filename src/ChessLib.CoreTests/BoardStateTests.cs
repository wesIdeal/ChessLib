using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using EnumsNET;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests
{
    [TestFixture(TestOf = typeof(BoardState), Category = "Base Functionality")]
    public class BoardStateTests
    {
        [TestFixture(TestOf = typeof(BoardState), Category = "Base Functionality", Description = "En Passant")]
        public class EnPassantTests
        {
            [SetUp]
            public void Setup()
            {
                _boardState = new Mock<BoardState>();
                _boardState.Setup(x => x.ValidateEnPassantSquare(It.IsAny<ushort?>())).Verifiable();
            }

            private Mock<BoardState> _boardState;

            [Test(Description = "En Passant")]
            public void EnPassantSet_ShouldCallValidate()
            {
                _boardState.Object.ActivePlayer = Color.Black;
                _boardState.Object.EnPassantIndex = null;
                _boardState.Verify(x => x.ValidateEnPassantSquare(It.IsAny<ushort?>()), Times.Once);
            }

            [Test(Description = "En Passant")]
            public void EnPassant_ShouldSetSquareValueWhenSquareIsValidForEnPassant_Black()
            {
                var boardState = new BoardState { ActivePlayer = Color.White };
                var boardStateEnPassantIndex = "e6".SquareTextToIndex();
                boardState.EnPassantIndex = boardStateEnPassantIndex;
                Assert.AreEqual(boardStateEnPassantIndex, boardState.EnPassantIndex);
            }

            [Test(Description = "En Passant")]
            public void EnPassant_ShouldSetSquareValueWhenSquareIsValidForEnPassant_White()
            {
                var boardState = new BoardState { ActivePlayer = Color.Black };
                var boardStateEnPassantIndex = "e3".SquareTextToIndex();
                boardState.EnPassantIndex = boardStateEnPassantIndex;
                Assert.AreEqual(boardStateEnPassantIndex, boardState.EnPassantIndex);
            }

            [Test(Description = "En Passant")]
            public void EnPassant_ShouldReturnNullWhenSetToNull()
            {
                var boardState = new BoardState { ActivePlayer = Color.Black, EnPassantIndex = null };
                Assert.IsNull(boardState.EnPassantIndex);
            }
        }

        [TestCaseSource(nameof(GetPieceCapturedTestCases))]
        public void PieceCapturedTests(TestCase<bool, Piece?> testCase)
        {
            var boardState = new BoardState { ActivePlayer = Color.Black };
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
            var boardState = new BoardState { GameState = testCase.TestMethodInputValue };
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
            var boardState = new BoardState { CastlingAvailability = testCase.TestMethodInputValue };
            Assert.AreEqual(testCase.ExpectedValue, boardState.CastlingAvailability);
        }

        protected static IEnumerable<TestCase<CastlingAvailability, CastlingAvailability>> GetCastlingAbilityTestCases()
        {

            var max = 15ul;

            var permutations = MovingPieceService.GetAllPermutationsOfSetBits(max.GetSetBits(), 0, 0);
            var castlingCartesian = permutations.Distinct()
                .Select(x => (CastlingAvailability)x)
                .Select(x =>
                    new TestCase<CastlingAvailability, CastlingAvailability>(x,
                        x, $"{FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(x)}")).ToArray();
            return castlingCartesian;
        }

        [TestCaseSource(nameof(GetHalfMoveClockTestCases))]
        public void TestHalfMoveClockTestCases(TestCase<bool, byte> testCase)
        {
            var boardState = new BoardState { HalfMoveClock = testCase.TestMethodInputValue };

            Assert.AreEqual(testCase.TestMethodInputValue, boardState.HalfMoveClock, testCase.ToString());
        }

        protected static IEnumerable<TestCase<bool, byte>> GetHalfMoveClockTestCases()
        {
            return Enumerable.Range(0, byte.MaxValue + 2).Select(x => new TestCase<bool, byte>(true, (byte)x));
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
            return Enumerable.Range(0, 513).Select(x => new TestCase<bool, uint>(x < 512, (uint)x));
        }
    }
}