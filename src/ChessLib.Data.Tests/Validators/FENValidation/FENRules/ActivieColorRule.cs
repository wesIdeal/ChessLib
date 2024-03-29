﻿using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using NUnit.Framework;

namespace ChessLib.Data.Validators.FENValidation.FENRules.Tests
{
    [TestFixture]
    public static class ActivieColorRule
    {
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR z KQkq - 0 1", FENError.InvalidActiveColor)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR   KQkq - 0 1", FENError.InvalidActiveColor)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", FENError.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", FENError.None)]
        public static void ValidateActiveColor(string fen, FENError expectedError)
        {
            var validator = new ActiveColorRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}