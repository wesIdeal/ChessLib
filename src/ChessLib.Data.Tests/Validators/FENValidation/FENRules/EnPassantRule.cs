using ChessLib.Data.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Validators.FENValidation.FENRules
{
    [TestFixture]
    public static class EnPassantRule
    {
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e6 0 1", FENError.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq a3 0 1", FENError.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq h3 0 1", FENError.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", FENError.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq 4e 0 1", FENError.InvalidEnPassantSquare)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq 4 0 1", FENError.InvalidEnPassantSquare)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e9 0 1", FENError.InvalidEnPassantSquare)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq i3 0 1", FENError.InvalidEnPassantSquare)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq z8 0 1", FENError.InvalidEnPassantSquare)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -- 0 1", FENError.InvalidEnPassantSquare)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq 12 0 1", FENError.InvalidEnPassantSquare)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq ee 0 1", FENError.InvalidEnPassantSquare)]
        public static void ValidateEnPassant(string fen, FENError expectedError)
        {
            var validator = new Data.Validators.FENValidation.Rules.EnPassantRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}