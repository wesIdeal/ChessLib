#region

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;
using Moq;
using NUnit.Framework;

#endregion

namespace ChessLib.Core.Tests.Validation.Validators.BoardValidation.Rules
{
    [TestFixture]
    public class EnPassantSquareRuleTests
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();
        private EnPassantPositionRule rule;
        private const string c4Fen = "rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq c3 0 1";
        private const string c4e5Fen = "rnbqkbnr/pppp1ppp/8/4p3/2P5/8/PP1PPPPP/RNBQKBNR w KQkq e6 0 2";

        [SetUp]
        public void SetUp()
        {
            //Debug.WriteLine("Setting up ep tests.");
            rule = new EnPassantPositionRule();

        }

        [TestCase(c4Fen)]
        [TestCase(c4e5Fen)]
        public void Validate_ShouldCallCorrectSquareMethodWithCorrectArguments(string fen)
        {
            var board = fenTextToBoard.Translate(fen);
            var mock = new Mock<EnPassantPositionRule>(){CallBase = true};
            
            mock.Setup(x => x.IsEnPassantSquareCorrectForActive( 
                It.Is<Color>( c=>c == board.ActivePlayer), 
                It.Is<ushort>(t => t == board.EnPassantIndex.Value))).Returns(false).Verifiable();
            
            mock.Object.Validate(board);
            mock.Verify();
        }
        [Test]
        public void Validate_ShouldNotCallPlacementMethodWhenSquareMethodReturnsFalse()
        {
            var board = fenTextToBoard.Translate(c4Fen);
            var mock = new Mock<EnPassantPositionRule>();

            mock.Setup(x => x.IsEnPassantSquareCorrectForActive(
                It.Is<Color>(c => c == board.ActivePlayer),
                It.Is<ushort>(t => t == board.EnPassantIndex.Value))).Returns(false);
            mock.Setup(x=>x.IsPawnPresentNorthOfEnPassantSquare(It.IsAny<ulong>(), It.IsAny<ushort>())).Verifiable();
            mock.Object.Validate(board);
            mock.Verify(x=>x.IsPawnPresentNorthOfEnPassantSquare(It.IsAny<ulong>(), It.IsAny<ushort>()), Times.Never);
        }
        [Test]
        public void Validate_ShouldCallPlacementMethodWhenSquareMethodReturnsTrue()
        {
            var board = fenTextToBoard.Translate(c4e5Fen);
            var mock = new Mock<EnPassantPositionRule>(){CallBase = true};

            mock.Setup(x => x.IsEnPassantSquareCorrectForActive(
                It.Is<Color>(c => c == board.ActivePlayer),
                It.Is<ushort>(t => t == board.EnPassantIndex.Value))).Returns(true);
            mock.Setup(x => x.IsPawnPresentNorthOfEnPassantSquare(It.IsAny<ulong>(), It.IsAny<ushort>())).Returns(true).Verifiable();
            mock.Object.Validate(board);
            mock.Verify(x => x.IsPawnPresentNorthOfEnPassantSquare(It.IsAny<ulong>(), It.IsAny<ushort>()), Times.Once);
        }

        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(true, true, true)]
        public void Validate_ShouldReturn_And_OfTwoMethods(bool output1, bool output2, bool expected)
        {
            var expectedValidationError = expected ? BoardExceptionType.None : BoardExceptionType.BadEnPassant;
            var board = fenTextToBoard.Translate(c4e5Fen);
            var mock = new Mock<EnPassantPositionRule>() { CallBase = true };
            mock.Setup(x => x.IsEnPassantSquareCorrectForActive(It.IsAny<Color>(), It.IsAny<ushort>()))
                .Returns(output1);
            mock.Setup(x => x.IsPawnPresentNorthOfEnPassantSquare(It.IsAny<ulong>(), It.IsAny<ushort>()))
                .Returns(output2);
            var actual = mock.Object.Validate(board);
            Assert.AreEqual(expectedValidationError, actual);
        }

       [TestCase("rnbqkbnr/pppppppp/8/8/8/5N2/PPPPPPPP/RNBQKB1R b KQkq - 1 1")]
        public void Validate_ShouldNotCallAnyValidationsIfEPIndexIsNull(string fen)
        {
            var expectedValidationError = BoardExceptionType.None;
            var board = fenTextToBoard.Translate(fen);
            var mock = new Mock<EnPassantPositionRule>() { CallBase = true };
            mock.Setup(x => x.IsEnPassantSquareCorrectForActive(It.IsAny<Color>(), It.IsAny<ushort>()))
                .Verifiable();
            mock.Setup(x => x.IsPawnPresentNorthOfEnPassantSquare(It.IsAny<ulong>(), It.IsAny<ushort>()))
                .Verifiable();
            var actual = mock.Object.Validate(board);
            mock.Verify(x=> x.IsEnPassantSquareCorrectForActive(It.IsAny<Color>(), It.IsAny<ushort>()), Times.Never);
            mock.Verify(x=> x.IsPawnPresentNorthOfEnPassantSquare(It.IsAny<ulong>(), It.IsAny<ushort>()), Times.Never);
            Assert.AreEqual(expectedValidationError, actual);
        }

        [TestCaseSource(nameof(GetSquareTestCases))]

        public void IsEnPassantSquareCorrectForActive_ShouldTestForInvalidSquareAndActiveColor(EnPassantSquareLocationTestCase testCase)
        {
            var result = rule.IsEnPassantSquareCorrectForActive(testCase.ActivePlayerColor, testCase.SquareIndex);
            Assert.AreEqual(testCase.ExpectedValue, result, testCase.Description);
        }
        [TestCaseSource(nameof(GetPawnPlacementTestCases))]
        public void IsPawnPresentNorthOfEnPassantSquare_ShouldTestForProperPawnPlacement(EnPassantPawnLocationTestCase testCase)
        {
            var result = rule.IsPawnPresentNorthOfEnPassantSquare(testCase.Occupancy, testCase.SquareIndex);
            Assert.AreEqual(testCase.ExpectedValue, result, testCase.Description);
        }

        protected static IEnumerable<EnPassantPawnLocationTestCase> GetPawnPlacementTestCases()
        {
            yield return new EnPassantPawnLocationTestCase("d5".SquareTextToIndex().GetBoardValueOfIndex(), "d6".SquareTextToIndex(), true,
                "d5 pawn, d6 en passant square");
            yield return new EnPassantPawnLocationTestCase("e4".SquareTextToIndex().GetBoardValueOfIndex(), "e3".SquareTextToIndex(), true,
                "d5 pawn, d6 en passant square");
            yield return new EnPassantPawnLocationTestCase("d5".SquareTextToIndex().GetBoardValueOfIndex(), "e6".SquareTextToIndex(), false,
                "Bad En Passant: d5 pawn, e6 en passant square");
            yield return new EnPassantPawnLocationTestCase("e4".SquareTextToIndex().GetBoardValueOfIndex(), "d3".SquareTextToIndex(), false,
                "Bad En Passant: e4 pawn, d3 en passant square");

        }

        public static IEnumerable<EnPassantSquareLocationTestCase> GetSquareTestCases()
        {
            yield return new EnPassantSquareLocationTestCase("d5".SquareTextToIndex(), Color.Black, false,
                "Non-en passant square(d5), black to move.");
            yield return new EnPassantSquareLocationTestCase("d5".SquareTextToIndex(), Color.White, false,
                "Non-en passant square(d5), white to move.");
            yield return new EnPassantSquareLocationTestCase("h8".SquareTextToIndex(), Color.Black, false,
                "Non-en passant square(h8), black to move.");
            yield return new EnPassantSquareLocationTestCase("h1".SquareTextToIndex(), Color.White, false,
                "Non-en passant square(h1), white to move.");
            yield return new EnPassantSquareLocationTestCase("e3".SquareTextToIndex(), Color.White, false,
                "En passant square(e3), wrong color (white to move).");
            yield return new EnPassantSquareLocationTestCase("e6".SquareTextToIndex(), Color.Black, false,
                "En passant square(e6), wrong color (black to move).");
            yield return new EnPassantSquareLocationTestCase("e3".SquareTextToIndex(), Color.Black, true,
                "En passant square(e3), correct color (black to move).");
            yield return new EnPassantSquareLocationTestCase("e6".SquareTextToIndex(), Color.White, true,
                "En passant square(e6), correct color (white to move).");
        }
    }
}