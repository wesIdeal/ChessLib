using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.FENValidation.Rules.FENRules
{
    [TestFixture(TestOf = typeof(FENStructureRule))]
    public class FENStructureRuleTests
    {
        private const string name = "FEN Validation: FEN Structure: ";

        private static readonly FENStructureRule validator = new FENStructureRule();

        [TestCase("", FENError.InvalidFENString, TestName = name + "Empty String")]
        [TestCase(null, FENError.InvalidFENString, TestName = name + "null")]
        [TestCase("fen s struct", FENError.InvalidFENString, TestName = name + "Non-FEN String")]
        [TestCase("fen s struct", FENError.InvalidFENString, TestName = name + "Non-FEN String")]
        [TestCase(BoardConstants.FenStartingPosition, FENError.None, TestName = name + "Normal Starting Position")]
        public static void ValidateStructure(string fen, FENError expectedError)
        {
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }

        [Test]
        public void ValidateTest_ShouldFailIfFENHasLessThanTheRequiredSections()
        {
            var tooFewSections =
                $"a{FENStructureRule.SeperationCharacter}b{FENStructureRule.SeperationCharacter}";
            var actual = validator.Validate(tooFewSections);
            Assert.AreEqual(FENError.InvalidFENString, actual,
                "Should return InvalidFENString when too few sections are supplied.");
        }

        [Test]
        public void ValidateTest_ShouldIgnoreExtraWhitespace()
        {
            var whiteSpaceGalore =
                "1  2      3   4  5 6   ";
            var actual = validator.Validate(whiteSpaceGalore);
            Assert.AreEqual(FENError.None, actual, "Should ignore extra whitespace.");
        }


        [Test]
        public void ValidateTest_ShouldFailIfFENHasMoreThanTheRequiredSections()
        {
            var tooManySections =
                $"a{FENStructureRule.SeperationCharacter}b{FENStructureRule.SeperationCharacter}3{FENStructureRule.SeperationCharacter}4{FENStructureRule.SeperationCharacter}5{FENStructureRule.SeperationCharacter}6{FENStructureRule.SeperationCharacter}7";
            var actual = validator.Validate(tooManySections);
            Assert.AreEqual(FENError.InvalidFENString, actual,
                "Should return InvalidFENString when too many sections are supplied.");
        }

        [Test]
        public void CastlingStringValidation_ShouldReturnNoCastling_WhenStringIsNull()
        {
            var result = CastlingAvailabilityRule.ValidateCastlingAvailabilityString(null);
            Assert.AreEqual(result, FENError.CastlingNoStringPresent);
        }

        [Test]
        public void CastlingStringValidation_ShouldReturnError_WhenCastlingCharIsUnrecognized()
        {
            var result = CastlingAvailabilityRule.ValidateCastlingAvailabilityString("w");
            Assert.AreEqual(result, FENError.CastlingUnrecognizedChar);
        }

        [Test]
        public void CastlingStringValidation_ShouldReturnError_WhenCharacterIsRepeated()
        {
            var result = CastlingAvailabilityRule.ValidateCastlingAvailabilityString("kk");
            Assert.AreEqual(result, FENError.CastlingStringRepetition);
        }


        [Test]
        public void ValidateTest_ShouldReturnNoErrorIndicatedWhenFenIsCorrect()
        {
            var actual = validator.Validate(BoardConstants.FenStartingPosition);
            Assert.AreEqual(FENError.None, actual,
                "Should return InvalidFENString when too many sections are supplied.");
        }
    }
}