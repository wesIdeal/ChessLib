using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Validators.FENValidation.FENRules
{
    [TestFixture]
    public class FENStructureRule
    {
        [TestCase("", FENError.InvalidFENString)]
        [TestCase(null, FENError.InvalidFENString)]
        [TestCase("fen s struct", FENError.InvalidFENString)]
        [TestCase(FENHelpers.FENInitial, FENError.None)]
        public static void ValidateStructure(string fen, FENError expectedError)
        {
            var validator = new Data.Validators.FENValidation.Rules.FENStructureRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}