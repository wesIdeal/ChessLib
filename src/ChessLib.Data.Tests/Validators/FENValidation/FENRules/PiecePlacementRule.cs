﻿using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Validators.FENValidation.FENRules
{
    [TestFixture]
    public static class PiecePlacementRule
    {
        [TestCase("rnbqkbnr/ppspppzp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", FENError.PiecePlacementInvalidChars)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR/rnbqkbnr w KQkq - 0 1",
            FENError.PiecePlacementRankCount)]
        [TestCase("rnbqkbnr/rnbqkbnr w KQkq - 0 1", FENError.PiecePlacementRankCount)]
        [TestCase("rnbqkbnr/ppppppppp/8/8/8/8/PPPPPPPP/RNBQrKBNR w KQkq - 0 1",
            FENError.PiecePlacementPieceCountInRank)]
        [TestCase(BoardConstants.FenStartingPosition, FENError.None)]
        public static void ValidatePiecePlacement(string fen, FENError expectedError)
        {
            var validator = new Core.Validation.Validators.FENValidation.Rules.PiecePlacementRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}