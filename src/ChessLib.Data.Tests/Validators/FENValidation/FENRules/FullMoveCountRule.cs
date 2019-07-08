using ChessLib.Data.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Validators.FENValidation.FENRules
{
    [TestFixture]
    public static class FullMoveCountRule
    {
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 -1", FENError.FullMoveCounter)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0  ", FENError.FullMoveCounter)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0  -", FENError.FullMoveCounter)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", FENError.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 12", FENError.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 0", FENError.None)]
        public static void ValidatFullMoveCounter(string fen, FENError expectedError)
        {
            var validator = new Data.Validators.FENValidation.Rules.FullMoveCountRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}