using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.FENValidation.Rules.FENRules
{
    [TestFixture(TestOf = typeof(HalfmoveClockRule))]
    public class HalfmoveClockRuleTests
    {
        private const string name = "FEN Validation: Halfmove Clock: ";

        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - -1 1", FENError.HalfmoveClock,
            TestName = name + "Negative Number")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -   1", FENError.HalfmoveClock,
            TestName = name + "No Number")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - - 1", FENError.HalfmoveClock,
            TestName = name + "Dash")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", FENError.None,
            TestName = name + "Valid - 0")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 50 1", FENError.None,
            TestName = name + "Valid - 50")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 4 1", FENError.None,
            TestName = name + "Valid - 4")]
        public static void ValidateHalfmoveClock(string fen, FENError expectedError)
        {
            var validator = new HalfmoveClockRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}