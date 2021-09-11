using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.Promotion.TestData
{
    internal class SourceIsPawnValidatorTestData
    {
        private static readonly FenTextToBoard fenTranslator = new FenTextToBoard();
        private const string name = "Promotion is from Pawn Validation";
        public static IEnumerable<TestCaseData> GetTestCases()
        {
            var whitePromotionOfPawn = "4k3/P7/8/8/8/8/8/4K3 w - - 0 1";
            var whitePromotionOfRook = "4k3/R7/8/8/8/8/8/4K3 w - - 0 1";
            var blackPromotionOfPawn = "4k3/8/8/8/8/8/p7/4K3 b - - 0 1";
            var blackPromotionOfQueen = "4k3/8/8/8/8/8/q7/4K3 b - - 0 1";
            var whitePromotion = MoveHelpers.GenerateMove("a7".ToBoardIndex(), "a8".ToBoardIndex(), MoveType.Promotion);
            var blackPromotion = MoveHelpers.GenerateMove("a2".ToBoardIndex(), "a1".ToBoardIndex(), MoveType.Promotion);
            yield return new TestCaseData(fenTranslator.Translate(whitePromotionOfPawn), whitePromotion)
                .SetName($"{name} - White Promoting Pawn (Valid)")
                .Returns(MoveError.NoneSet);
            yield return new TestCaseData(fenTranslator.Translate(whitePromotionOfRook), whitePromotion)
                .SetName($"{name} - White Promoting Rook (Invalid)")
                .Returns(MoveError.SourceIsNotPawn);
            yield return new TestCaseData(fenTranslator.Translate(blackPromotionOfPawn), blackPromotion)
                .SetName($"{name} - Black Promoting Pawn (Valid)")
                .Returns(MoveError.NoneSet);
            yield return new TestCaseData(fenTranslator.Translate(blackPromotionOfQueen), blackPromotion)
                .SetName($"{name} - Black Promoting Queen (Invalid)")
                .Returns(MoveError.SourceIsNotPawn);
        }
    }
}
