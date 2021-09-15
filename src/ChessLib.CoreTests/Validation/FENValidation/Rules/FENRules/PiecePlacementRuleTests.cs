using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.FENValidation.Rules.FENRules
{
    [TestFixture(TestOf = typeof(PiecePlacementRule))]
    public static class PiecePlacementRuleTests
    {
        private const string name = "FEN Validation: Piece Placement: ";

        [TestCase("rnbqkbnr/ppspppzp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", FENError.PiecePlacementInvalidChars,
            TestName = name + "Invalid Chars")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR/rnbqkbnr w KQkq - 0 1",
            FENError.PiecePlacementRankCount, TestName = name + "Too Many Ranks")]
        [TestCase("rnbqkbnr/rnbqkbnr w KQkq - 0 1", FENError.PiecePlacementRankCount,
            TestName = name + "Too Few Ranks")]
        [TestCase("rnbqkbnr/ppppppppp/8/8/8/8/PPPPPPPP/RNBQrKBNR w KQkq - 0 1",
            FENError.PiecePlacementPieceCountInRank, TestName = name + "Too Many Pieces on Rank")]
        [TestCase(BoardConstants.FenStartingPosition, FENError.None, TestName = name + "Valid")]
        public static void ValidatePiecePlacement(string fen, FENError expectedError)
        {
            var validator = new PiecePlacementRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}