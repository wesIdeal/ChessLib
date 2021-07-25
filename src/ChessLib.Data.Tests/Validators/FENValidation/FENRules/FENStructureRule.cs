using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Data.Helpers;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Validators.FENValidation.FENRules
{
    [TestFixture]
    public class FENStructureRule
    {
        [TestCase("", FENError.InvalidFENString)]
        [TestCase(null, FENError.InvalidFENString)]
        [TestCase("fen s struct", FENError.InvalidFENString)]
        [TestCase(FenReader.FENInitial, FENError.None)]
        public static void ValidateStructure(string fen, FENError expectedError)
        {
            var validator = new Core.Validation.Validators.FENValidation.Rules.FENStructureRule();
            var actual = validator.Validate(fen);
            Assert.AreEqual(expectedError, actual);
        }
    }
}