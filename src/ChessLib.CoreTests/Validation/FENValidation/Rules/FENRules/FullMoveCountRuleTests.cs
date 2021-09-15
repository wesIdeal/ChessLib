using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.FENValidation.Rules.FENRules
{
    [TestFixture(TestOf = typeof(FullMoveCountRule))]
    public static class FullMoveCountRuleTests
    {
        private const string name = "FEN Validation: Full Move Count: ";

        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 -1", FENError.FullMoveCounter,
            TestName = name + "Negative")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0  ", FENError.FullMoveCounter,
            TestName = name + "None")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0  -", FENError.FullMoveCounter,
            TestName = name + "Dash")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", FENError.None,
            TestName = name + "Valid - 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 12", FENError.None,
            TestName = name + "Valid - 12")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 0", FENError.None,
            TestName = name + "Valid - 0")]
        public static void FullMoveCountValidatorTests(string fen, FENError expectedError)
        {
            var validator = new FullMoveCountRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}