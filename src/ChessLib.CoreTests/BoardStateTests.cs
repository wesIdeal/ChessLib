using System;
using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using EnumsNET;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;
using Moq;

namespace ChessLib.Core.Tests
{
    [TestFixture(TestOf = typeof(BoardState))]
    public class BoardStateTests
    {

        [Test()]
        public void EnPassant_ShouldThrowExceptionWhenSquareIsInvalidForEnPassant()
        {
            var boardState = new BoardState { ActivePlayer = Color.Black };
            var mock = new Mock<IEnPassantSquareRule>();
            var ruleMock = mock.Setup(m => m.IsValidEnPassantSquare(It.IsAny<ushort?>(), It.IsAny<Color>())).Returns(false);
            boardState.EnPassantSquareRule = mock.Object;
            Assert.Throws(typeof(BoardException), delegate { boardState.EnPassantIndex = null; });
        }

        [Test()]
        public void EnPassant_ShouldSetSquareValueWhenSquareIsValidForEnPassant_Black()
        {
            var boardState = new BoardState { ActivePlayer = Color.Black };
            var mock = new Mock<IEnPassantSquareRule>();
            var ruleMock = mock.Setup(m => m.IsValidEnPassantSquare(It.IsAny<ushort?>(), It.IsAny<Color>())).Returns(true);
            var boardStateEnPassantIndex = "e6".SquareTextToIndex();
            boardState.EnPassantIndex = boardStateEnPassantIndex;
            Assert.AreEqual(boardStateEnPassantIndex, boardState.EnPassantIndex);
        }

        [Test()]
        public void EnPassant_ShouldSetSquareValueWhenSquareIsValidForEnPassant_White()
        {
            var boardState = new BoardState { ActivePlayer = Color.White };
            var mock = new Mock<IEnPassantSquareRule>();
            var ruleMock = mock.Setup(m => m.IsValidEnPassantSquare(It.IsAny<ushort?>(), It.IsAny<Color>())).Returns(true);
            var boardStateEnPassantIndex = "e3".SquareTextToIndex();
            boardState.EnPassantIndex = boardStateEnPassantIndex;
            Assert.AreEqual(boardStateEnPassantIndex, boardState.EnPassantIndex);
        }

        [Test()]
        public void EnPassant_ShouldReturnNullWhenSetToNull()
        {
            var boardState = new BoardState { ActivePlayer = Color.Black };
            var mock = new Mock<IEnPassantSquareRule>();
            mock.Setup(m => m.IsValidEnPassantSquare(It.IsAny<ushort?>(), It.IsAny<Color>())).Returns(true);
            boardState.EnPassantIndex = (ushort?)null;
            Assert.IsNull(boardState.EnPassantIndex);
        }

        [TestCaseSource(nameof(GetPieceCapturedTestCases))]
        public void PieceCapturedTests(TestCase<bool, Piece?> testCase)
        {
            var boardState = new BoardState() {ActivePlayer = Color.Black};
            if (!testCase.ExpectedValue)
            {
                Assert.Throws(typeof(ArgumentException),
                    delegate { boardState.PieceCaptured = testCase.InputValue.Value; }, "King should throw exception.");
            }

            else
            {
                boardState.PieceCaptured = testCase.InputValue;
                Assert.AreEqual(testCase.InputValue, boardState.PieceCaptured); 
            }
        }

        protected static IEnumerable<TestCase<bool, Piece?>> GetPieceCapturedTestCases()
        {
            foreach (var piece in Enums.GetValues<Piece>())
            {
                yield return new TestCase<bool, Piece?>(piece == Piece.King ? false : true, piece);
            }

            yield return new TestCase<bool, Piece?>(true, null);
        }

        [Test()]
        public void EqualsTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ValidateEnPassantSquareTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void EqualsTest1()
        {
            Assert.Fail();
        }

        [Test()]
        public void InitializeMasksTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ToStringTest()
        {
            Assert.Fail();
        }
    }
}