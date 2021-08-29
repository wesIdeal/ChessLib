using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.MoveValidation;
using ChessLib.Core.Validation.Validators.MoveValidation.MoveRules;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules
{
    [TestFixture(TestOf = typeof(MoveValidator))]
    public class MoveValidatorTests
    {
        [TestCaseSource(typeof(MoveValidatorTestData),
            nameof(MoveValidatorTestData.GetMoveDestinationValidatorTestCases))]
        [TestOf(typeof(MoveDestinationValidator))]
        public MoveError TestMoveDestinationValidator(Board board, Move move, ulong pseudoLegalMovesReturnValue)
        {
            var bitboardMock = new Mock<IBitboard>();
            var sourcePieceAndColor = board.Occupancy.GetPieceOfColorAtIndex(move.SourceIndex);

            var verifiableMockedParamsExpression =
                MoveDestinationValidatorTestParams.SetupPseudoLegalMovesMock(board, move, pseudoLegalMovesReturnValue,
                    bitboardMock);
            var moveValidator = new MoveDestinationValidator(bitboardMock.Object);
            var validationResult = moveValidator.Validate(board, null, move);

            //we don't expect any calls if the source square is not occupied
            var numberOfCallsToMockedMethodExpected = sourcePieceAndColor.HasValue ? Times.Once() : Times.Never();
            bitboardMock.Verify(verifiableMockedParamsExpression, numberOfCallsToMockedMethodExpected);
            return validationResult;
        }
    }


}