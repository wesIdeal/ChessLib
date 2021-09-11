using ChessLib.Core.Tests.Validation.MoveValidation.Promotion.TestData;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.MoveValidation.PromotionRules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.Promotion
{
    [TestFixture]
    public class PromotionRuleTests
    {
        private static readonly SourceIsPawnValidator validator = new SourceIsPawnValidator();

        [TestOf(typeof(SourceIsPawnValidator))]
        [TestCaseSource(typeof(SourceIsPawnValidatorTestData), nameof(SourceIsPawnValidatorTestData.GetTestCases))]
        public MoveError SourceIsPawnTests(Board board, Move move)
        {
            var validationResult = validator.Validate(board, move);
            return validationResult;
        }
    }
}