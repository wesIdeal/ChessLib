using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.FENValidation.Rules.FENRules
{
    [TestFixture(TestOf = typeof(ActiveColorRule))]
    public static class ActiveColorRuleTests
    {
        private const string name = "FEN Validation: Active Color: ";

        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR z KQkq - 0 1", FENError.InvalidActiveColor,
            TestName = name + "Invalid 'z'")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR   KQkq - 0 1", FENError.InvalidActiveColor,
            TestName = name + "Invalid ' '")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", FENError.None,
            TestName = name + "Valid 'w'")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", FENError.None,
            TestName = name + "Valid 'b'")]
        public static void ValidateActiveColor(string fen, FENError expectedError)
        {
            var validator = new ActiveColorRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}