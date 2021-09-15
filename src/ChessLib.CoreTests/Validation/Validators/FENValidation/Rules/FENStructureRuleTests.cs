using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.FENValidation.Rules
{
    [TestFixture]
    public class FENStructureRuleTests
    {
        private readonly FENStructureRule _fenStructureRule = new FENStructureRule();

        [Test]
        public void ValidateTest_ShouldFailIfFENHasLessThanTheRequiredSections()
        {
            var tooFewSections =
                $"a{FENStructureRule.SeperationCharacter}b{FENStructureRule.SeperationCharacter}";
            var actual = _fenStructureRule.Validate(tooFewSections);
            Assert.AreEqual(FENError.InvalidFENString, actual,
                "Should return InvalidFENString when too few sections are supplied.");
        }

        [Test]
        public void ValidateTest_ShouldIgnoreExtraWhitespace()
        {
            var whiteSpaceGalore =
                "1  2      3   4  5 6   ";
            var actual = _fenStructureRule.Validate(whiteSpaceGalore);
            Assert.AreEqual(FENError.None, actual, "Should ignore extra whitespace.");
        }



        [Test]
        public void ValidateTest_ShouldFailIfFENHasMoreThanTheRequiredSections()
        {
            var tooManySections =
                $"a{FENStructureRule.SeperationCharacter}b{FENStructureRule.SeperationCharacter}3{FENStructureRule.SeperationCharacter}4{FENStructureRule.SeperationCharacter}5{FENStructureRule.SeperationCharacter}6{FENStructureRule.SeperationCharacter}7";
            var actual = _fenStructureRule.Validate(tooManySections);
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
            var actual = _fenStructureRule.Validate(BoardConstants.FenStartingPosition);
            Assert.AreEqual(FENError.None, actual,
                "Should return InvalidFENString when too many sections are supplied.");
        }
    }
}