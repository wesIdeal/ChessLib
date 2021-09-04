﻿using System;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules.TestData;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using ChessLib.Core.Types.Enums;


// ReSharper disable StringLiteralTypo

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules
{
    internal class CastlingRuleTests
    {
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
    }
}