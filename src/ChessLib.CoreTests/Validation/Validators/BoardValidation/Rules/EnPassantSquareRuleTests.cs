using NUnit.Framework;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Types.Enums;
using EnumsNET;

namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules.Tests
{
    [TestFixture()]
    public class EnPassantSquareRuleTests
    {
        [Test()]
        public void ValidateTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ValidateEnPassantSquareTest()
        {
            Assert.Fail();
        }

        [TestCaseSource(nameof(GetIsValidEnPassantSquareTestCases))]
        public void IsValidEnPassantSquareTest(TestCase<bool, ushort?> testCase)
        {
            var enPassantSquareRule = new EnPassantSquareRule();
            var actual = enPassantSquareRule.IsValidEnPassantSquare(testCase.InputValue, (Color)testCase.AdditionalInputs.Single());
            Assert.AreEqual(testCase.ExpectedValue, actual);
        }

        //
        protected static IEnumerable<TestCase<bool, ushort?>> GetIsValidEnPassantSquareTestCases()
        {
            foreach (var color in Enums.GetValues<Color>())
            {
                var rank3 = BoardConstants.Rank3;
                var rank6 = BoardConstants.Rank6;
                foreach (var square in BoardConstants.AllSquares)
                {
                    var squareValue = square.GetBoardValueOfIndex();
                    var isValid = color == Color.White ? (rank6 & squareValue) == squareValue : (rank3 & squareValue) == squareValue;
                    yield return new TestCase<bool, ushort?>(isValid, square, $"{color.AsString()} {square.IndexToSquareDisplay()}", color);
                }
                yield return new TestCase<bool, ushort?>(true, (ushort?)null, $"{color.AsString()} [null]", color);
            }
        }
    }
}