using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Tests.Validation.MoveValidation.MoveRules.TestData;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.MoveValidation.MoveRules;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.MoveRules
{
    [TestFixture(Category = "Validation, Move Validation", Description = "Move validation tests")]
    public class MoveRuleTests
    {
        [TestCaseSource(typeof(ActiveColorValidatorTestData),
            nameof(ActiveColorValidatorTestData.GetActiveColorValidatorTestCases))]
        [TestOf(typeof(ActiveColorValidator))]
        public MoveError ActiveColorValidatorTests(Board board, Move move)
        {
            var moveValidator = new ActiveColorValidator();
            var validationResult = moveValidator.Validate(board, move);
            return validationResult;
        }

        [TestCaseSource(typeof(NotInCheckAfterMoveValidatorTestData),
            nameof(NotInCheckAfterMoveValidatorTestData.GetKingNotInCheckAfterMoveValidatorTests))]
        [TestOf(typeof(NotInCheckAfterMoveValidator))]
        public MoveError NotInCheckAfterMoveValidatorTests(Board board, Board postMoveBoard, Move move,
            bool moveLeavesKingInCheck)
        {
            var bitboardMock = new Mock<IBitboard>();

            var verifiableMockedParamsExpression = board.SetupPostMoveCheckMock(postMoveBoard,
                move,
                moveLeavesKingInCheck,
                bitboardMock);
            var moveValidator = new NotInCheckAfterMoveValidator(bitboardMock.Object);
            var validationResult = moveValidator.Validate(board, postMoveBoard.Occupancy);
            bitboardMock.Verify(verifiableMockedParamsExpression, Times.Once);
            return validationResult;
        }

        [TestCaseSource(typeof(MoveDestinationValidatorTestData),
            nameof(MoveDestinationValidatorTestData.GetMoveDestinationValidatorTestCases))]
        [TestOf(typeof(MoveDestinationValidator))]
        public MoveError MoveDestinationValidatorTests(Board board, Move move, ulong pseudoLegalMovesReturnValue)
        {
            var bitboardMock = new Mock<IBitboard>();
            var sourcePieceAndColor = board.Occupancy.GetPieceOfColorAtIndex(move.SourceIndex);
            var destinationPieceAndColor = board.Occupancy.GetPieceOfColorAtIndex(move.DestinationIndex);

            /*
             * No calls expected if:
             *  a) The source square is not occupied by the active color
             */
            var numberOfExpectedCallsToMockedMethod = Times.Once();
            if (sourcePieceAndColor != null && (sourcePieceAndColor.Value.Color != board.ActivePlayer))
            {
                numberOfExpectedCallsToMockedMethod = Times.Never();
            }
            /*
             *  or b) The destination square is occupied by the active color
             */
            if (destinationPieceAndColor != null && (destinationPieceAndColor.Value.Color == board.ActivePlayer))
            {
                numberOfExpectedCallsToMockedMethod = Times.Never();
            }

            var verifiableMockedParamsExpression = board.SetupPseudoLegalMovesMock(move,
                pseudoLegalMovesReturnValue,
                bitboardMock);
            var moveValidator = new MoveDestinationValidator(bitboardMock.Object);
            var validationResult = moveValidator.Validate(board, move);


            bitboardMock.Verify(verifiableMockedParamsExpression, numberOfExpectedCallsToMockedMethod);
            return validationResult;
        }
    }
}