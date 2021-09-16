using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.FENValidation;
using EnumsNET;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.FENValidation
{
    [TestFixture(TestOf = typeof(FENValidator))]
    public class FENValidatorTests
    {
        [SetUp]
        public void Setup()
        {
            _fenRuleMock = new Mock<IFENRule>();
        }

        private Mock<IFENRule> _fenRuleMock;
        private FENValidator _fenValidator;
        public const string name = "FEN Validation: ";

        [TestCase(FENError.None, TestName = name + "Should not throw exception with valid FEN")]
        public void FENValidatorTest_ShouldNotThrowExceptionWhenValidFenString(FENError error )
        {
            Assert.DoesNotThrow(() => AssertExceptionValueSet(error));
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
            _fenRuleMock
                .Setup(x => x.Validate(It.IsAny<string>()))
                .Returns(error);
            _fenValidator = new FENValidator(_fenRuleMock.Object);
            try
            {
                _fenValidator.Validate(BoardConstants.FenStartingPosition);
            }
            catch (Exception exc)
            {
                var fenException = (FENException)exc;
                Assert.AreEqual(error, fenException.FENError, $"Expected fenException Error to have been {error}");
                throw;
            }
        }

        protected static IEnumerable<TestCaseData> AllErrors => Enums.GetValues<FENError>()
            .Where(e => e != FENError.None)
            .Select(x => new TestCaseData(x).SetName($"FEN Validation: Rule Validator Returns Error {x} "));
    }
}