using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules.TestData;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.MoveValidation;
using ChessLib.Core.Validation.Validators.MoveValidation.MoveRules;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules
{
    [TestFixture(TestOf = typeof(MoveValidator))]
    public class MoveValidatorTests
    {
        [TestCaseSource(typeof(MoveDestinationValidatorTestData),
            nameof(MoveDestinationValidatorTestData.GetMoveDestinationValidatorTestCases))]
        [TestOf(typeof(MoveDestinationValidator))]
        public MoveError TestMoveDestinationValidator(Board board, Move move, ulong pseudoLegalMovesReturnValue)
        {
            var bitboardMock = new Mock<IBitboard>();
            var sourcePieceAndColor = board.Occupancy.GetPieceOfColorAtIndex(move.SourceIndex);
            var destinationPieceAndColor = board.Occupancy.GetPieceOfColorAtIndex(move.DestinationIndex);
           
            /*
             * No calls expected if:
             *  a) The source square is not occupied by the active color
             *  b) The destination square is occupied by the active color
             */
            var numberOfExpectedCallsToMockedMethod = (sourcePieceAndColor?.Color != board.ActivePlayer ||
                                                       (destinationPieceAndColor?.Color != null &&
                                                        destinationPieceAndColor?.Color == board.ActivePlayer)) ? Times.Never() : Times.Once();
            var verifiableMockedParamsExpression =
                MoveDestinationValidatorTestParams.SetupPseudoLegalMovesMock(board, move, pseudoLegalMovesReturnValue,
                    bitboardMock);
            var moveValidator = new MoveDestinationValidator(bitboardMock.Object);
            var validationResult = moveValidator.Validate(board, null, move);

            
           
            bitboardMock.Verify(verifiableMockedParamsExpression, numberOfExpectedCallsToMockedMethod);
            return validationResult;
        }

        [TestCaseSource(typeof(NotInCheckAfterMoveValidatorTestData),
            nameof(NotInCheckAfterMoveValidatorTestData.GetKingNotInCheckAfterMoveValidatorTests))]
        [TestOf(typeof(NotInCheckAfterMoveValidator))]
        public MoveError TestKingNotInCheckAfterMove(Board board, Board postMoveBoard, Move move,
            bool moveLeavesKingInCheck)
        {
            var bitboardMock = new Mock<IBitboard>();

            var verifiableMockedParamsExpression =
                NotInCheckAfterMoveValidatorTestData.SetupKingInCheckMock(board, postMoveBoard, move,
                    moveLeavesKingInCheck,
                    bitboardMock);
            var moveValidator = new NotInCheckAfterMoveValidator(bitboardMock.Object);
            var validationResult = moveValidator.Validate(board, postMoveBoard.Occupancy, move);
            bitboardMock.Verify(verifiableMockedParamsExpression, Times.Once);
            return validationResult;
        }

        [TestCaseSource(typeof(ActiveColorValidatorTestData),
            nameof(ActiveColorValidatorTestData.GetActiveColorValidatorTestCases))]
        [TestOf(typeof(ActiveColorValidator))]
        public MoveError TestActiveColorValidator(Board board, Move move)
        {

            var moveValidator = new ActiveColorValidator();
            var validationResult = moveValidator.Validate(board, null, move);
            return validationResult;
        }
    }
}