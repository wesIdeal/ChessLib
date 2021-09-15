using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.FENValidation.Rules.FENRules
{
    [TestFixture(TestOf = typeof(EnPassantRule))]
    public static class EnPassantRuleTests
    {
        private const string name = "FEN Validation: En Passant: ";

        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e6 0 1", FENError.None, TestName = name + "Valid")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq a3 0 1", FENError.None, TestName = name + "Valid")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq h3 0 1", FENError.None, TestName = name + "Valid")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", FENError.None, TestName = name + "Valid")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq 4e 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - e5")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq 4e 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - e4")]

        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq 4e 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - 4e")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq 4 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - 4")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e9 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - e9")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq i3 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - i3")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq z8 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - z8")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -- 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - '--'")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq 12 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - 12")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq ee 0 1", FENError.InvalidEnPassantSquare, TestName = name + "Invalid - ee")]
        public static void ValidateEnPassant(string fen, FENError expectedError)
        {
            var validator = new EnPassantRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}