using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.FENValidation;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using Moq;
using NUnit.Framework;
using Enums = EnumsNET.Enums;

namespace ChessLib.Core.Tests.Validation.FENValidation
{
    [TestFixture()]
    public class FENValidatorTests
    {
        private Mock<IFENRule> _fenRuleMock;
        private FENValidator _fenValidator;
        [SetUp]
        public void Setup()
        {
            _fenRuleMock = new Mock<IFENRule>();

        }

        [Test]
        public void FenStructureRule_ShouldReturnInvalidString_WhenFenIsEmpty()
        {
            var validator = new FENStructureRule();
            Assert.AreEqual(FENError.InvalidFENString, validator.Validate(string.Empty));
        }

        [Test]
        public void FENValidatorTest_ShouldNotThrowExceptionWhenValidFenString()
        {
            var noFenError = FENError.None;
            Assert.DoesNotThrow(() => AssertExceptionValueSet(noFenError));
        }

        [TestCaseSource(nameof(AllErrors))]
        public void FENValidatorTest_ShouldThrowExceptionWhenInvalidFenStringDetected(FENError testCase)
        {
            _fenValidator = new FENValidator(_fenRuleMock.Object);
            Assert.Throws(typeof(FENException), () => AssertExceptionValueSet(testCase));
        }

        private void AssertExceptionValueSet(FENError error)
        {
            _fenRuleMock = new Mock<IFENRule>();
            var mockedMethod = _fenRuleMock
                .Setup(x => x.Validate(It.IsAny<string>()))
                .Returns(error);
            _fenValidator = new FENValidator(_fenRuleMock.Object);
            try
            {
                _fenValidator.Validate(BoardConstants.FenStartingPosition);
            }
            catch (System.Exception exc)
            {
                var fenException = (FENException)exc;
                Assert.AreEqual(error, fenException.FENError, $"Expected fenException Error to have been {error}");
                throw;
            }
        }
        protected static IEnumerable<FENError> AllErrors => Enums.GetValues<FENError>().Where(e => e != FENError.None);
    }
}