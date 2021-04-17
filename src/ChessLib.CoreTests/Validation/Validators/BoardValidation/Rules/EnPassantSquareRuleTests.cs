using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.BoardValidation;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;
using EnumsNET;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.BoardValidation.Rules
{
    [TestFixture()]
    public class EnPassantSquareRuleTests
    {
        [TestCaseSource(nameof(GetIsValidEnPassantSquareTestCases))]
        public void TestIBoardStateValidation(TestCase<BoardExceptionType, ushort?> testCase)
        {
            var enPassantSquareRule = new EnPassantSquareIndexRule();
            var actual = enPassantSquareRule.Validate(testCase.TestMethodInputValue, (Color)testCase.AdditionalInputs.Single());
            Assert.AreEqual(testCase.ExpectedValue, actual);
        }

        //
        protected static IEnumerable<TestCase<BoardExceptionType, ushort?>> GetIsValidEnPassantSquareTestCases()
        {
            foreach (var color in Enums.GetValues<Color>())
            {
                var rank3 = BoardConstants.Rank3;
                var rank6 = BoardConstants.Rank6;
                foreach (var square in BoardConstants.AllSquares)
                {
                    var squareValue = square.GetBoardValueOfIndex();
                    var isValid = color == Color.White ? (rank6 & squareValue) == squareValue : (rank3 & squareValue) == squareValue;
                    var boardException = isValid ? BoardExceptionType.None : BoardExceptionType.BadEnPassant;
                    yield return new TestCase<BoardExceptionType, ushort?>(boardException, square, $"{color.AsString()} {square.IndexToSquareDisplay()}", color);
                }
                yield return new TestCase<BoardExceptionType, ushort?>(BoardExceptionType.None, null, $"{color.AsString()} [null]", color);
            }
        }
    }
}