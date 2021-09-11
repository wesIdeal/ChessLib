using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Tests.Validation.MoveValidation.CastlingRules.TestData;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.MoveValidation.CastlingRules;
using Moq;
using NUnit.Framework;


// ReSharper disable StringLiteralTypo

namespace ChessLib.Core.Tests.Validation.MoveValidation.CastlingRules
{
    internal class CastlingRuleTests
    {
        private static readonly KingDestinationValidator kingDestinationValidator = new KingDestinationValidator();

        [TestOf(typeof(NotInCheckBeforeMoveValidator))]
        [TestCaseSource(typeof(NotInCheckBeforeMoveValidatorTestData),
            nameof(NotInCheckBeforeMoveValidatorTestData.GetNotInCheckBeforeMoveTestCases))]
        public MoveError NotInCheckBeforeMoveValidatorTest(Board board, Move move, bool kingInCheck)
        {
            var bitboard = board.SetupPreMoveCheckMock(kingInCheck);
            var validator = new NotInCheckBeforeMoveValidator(bitboard.Object);
            var result = validator.Validate(board, null, move);
            bitboard.Verify();
            return result;
        }

        [TestOf(typeof(NoPieceBlocksCastlingMoveValidator))]
        [TestCaseSource(typeof(NoPieceBlocksCastlingMoveValidatorTestData),
            nameof(NoPieceBlocksCastlingMoveValidatorTestData.GetTestCases))]
        public MoveError CastlingWithPiecesBlockingTests(Board board, Move move)
        {
            TestContext.WriteLine(board.Fen);
            var validator = new NoPieceBlocksCastlingMoveValidator();
            return validator.Validate(board, null, move);
        }

        [TestOf(typeof(AttackNotBlockingMoveValidator))]
        [TestCaseSource(typeof(AttackNotBlockingMoveValidatorTestData),
            nameof(AttackNotBlockingMoveValidatorTestData.GetCastlingMovesForColor), new object[] { Color.Black })]
        [TestCaseSource(typeof(AttackNotBlockingMoveValidatorTestData),
            nameof(AttackNotBlockingMoveValidatorTestData.GetCastlingMovesForColor), new object[] { Color.White })]
        public MoveError CastlingPathAttackedTests(Mock<IBitboard> bitBoardMock, Board board, Move move)
        {
            var validator = new AttackNotBlockingMoveValidator(bitBoardMock.Object);

            var valid = validator.Validate(board, null, move);
            foreach (var setup in bitBoardMock.Setups)
            {
                setup.Verify();
            }

            bitBoardMock.Verify();

            bitBoardMock.VerifyNoOtherCalls();
            return valid;
        }

        [TestOf(typeof(CastlingMoveIsAvailableValidator))]
        [TestCaseSource(typeof(CastlingMoveIsAvailableValidatorTestData),
            nameof(CastlingMoveIsAvailableValidatorTestData.GetCastlingMoveTestCases), new object[] { Color.Black })]
        [TestCaseSource(typeof(CastlingMoveIsAvailableValidatorTestData),
            nameof(CastlingMoveIsAvailableValidatorTestData.GetCastlingMoveTestCases), new object[] { Color.White })]
        public MoveError CastlingMoveIsAvailableValidatorTests(Board board, Move move)
        {
            var validator = new CastlingMoveIsAvailableValidator();
            var result = validator.Validate(board, null, move);
            return result;
        }

        [TestOf(typeof(KingDestinationValidator))]
        [TestCaseSource(typeof(KingDestinationValidatorTestData),
            nameof(KingDestinationValidatorTestData.GetValidDestinationTestCases), new object[] { Color.Black })]
        [TestCaseSource(typeof(KingDestinationValidatorTestData),
            nameof(KingDestinationValidatorTestData.GetValidDestinationTestCases), new object[] { Color.White })]
        public MoveError KingDestinationValidatorTests(Board board, Move move)
        {
            var validationResult = kingDestinationValidator.Validate(board, null, move);
            return validationResult;
        }
    }
}