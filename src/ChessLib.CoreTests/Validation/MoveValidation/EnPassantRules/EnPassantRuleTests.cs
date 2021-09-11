using ChessLib.Core.Tests.Validation.MoveValidation.EnPassantRules.TestData;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.MoveValidation.EnPassantRules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.EnPassantRules
{
    [TestFixture]
    public class EnPassantRuleTests
    {
        private static readonly EnPassantDestinationValidator validator = new EnPassantDestinationValidator();

        [TestOf(typeof(EnPassantDestinationValidator))]
        [TestCaseSource(typeof(EnPassantDestinationValidatorTestData),
            nameof(EnPassantDestinationValidatorTestData.GetEPDestinationValidatorTestCases))]
        public MoveError TestMethod(Board board, Move move)
        {
            var validationResult = validator.Validate(board, null, move);
            return validationResult;
        }
    }
}