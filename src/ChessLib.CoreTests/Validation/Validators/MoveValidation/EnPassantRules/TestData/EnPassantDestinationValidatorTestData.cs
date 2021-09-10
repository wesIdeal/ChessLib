using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.EnPassantRules.TestData
{
    internal class EnPassantDestinationValidatorTestData
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();
        public static IEnumerable<TestCaseData> GetEPDestinationValidatorTestCases()
        {
            var e2 = "e2".ToBoardIndex();
            var whiteAttacksEPSquare = "d6";
            var blackAttacksEPSquare = "d3";
            var badEpWhite = "e6";
            var badEpBlack = "e3";
            var whiteEnPassantAttackBoard =
                fenTextToBoard.Translate($"rnbqkbnr/ppp1pppp/8/3p4/8/5N2/PPPPPPPP/RNBQKB1R w KQkq {whiteAttacksEPSquare} 0 2");
            var blackEnPassantAttackBoard =
                fenTextToBoard.Translate($"rnbqkbnr/ppp1pppp/8/3p4/3P4/5N2/PPP1PPPP/RNBQKB1R b KQkq {blackAttacksEPSquare} 0 2");
            yield return new TestCaseData(whiteEnPassantAttackBoard, MoveHelpers.GenerateMove(e2, whiteAttacksEPSquare.ToBoardIndex()))
                .SetName($"{name} - EP Index ({whiteAttacksEPSquare}) is set")
                .Returns(MoveError.NoneSet);
            
            yield return new TestCaseData(whiteEnPassantAttackBoard, MoveHelpers.GenerateMove(e2, badEpWhite.ToBoardIndex()))
                .SetName($"{name} - EP Index ({badEpWhite}) is not set")
                .Returns(MoveError.EpNotAvailable);

            yield return new TestCaseData(blackEnPassantAttackBoard, MoveHelpers.GenerateMove(e2, blackAttacksEPSquare.ToBoardIndex()))
                .SetName($"{name} - EP Index ({blackAttacksEPSquare}) is set")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData(blackEnPassantAttackBoard, MoveHelpers.GenerateMove(e2, badEpBlack.ToBoardIndex()))
                .SetName($"{name} - EP Index ({badEpBlack}) is not set")
                .Returns(MoveError.EpNotAvailable);
        }

        private const string name = "En Passant Destination is Valid";
    }
}
