﻿using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.BoardValidation;
using ChessLib.Data.Helpers;
using NUnit.Framework;

namespace ChessLib.Data.Validators.BoardValidation.Tests
{
    [TestFixture]
    public sealed class PawnCountRule 
    {
        [OneTimeSetUp]
        public static void Setup()
        {

        }

        protected static readonly FenReader FenReader = new FenReader();
        [TestCase("rnbqkbnr/pppppppp/8/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.WhiteTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.BlackTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
            BoardExceptionType.WhiteTooManyPawns | BoardExceptionType.BlackTooManyPawns)]
        [TestCase(FenReader.FENInitial, BoardExceptionType.None)]
        public static void TestPawnCounts(string fen, BoardExceptionType expectedResult)
        {
            BoardExceptionType actual = BoardExceptionType.None;
            try
            {
                var board = FenReader.GetBoard(fen);
                var validator = new BoardValidator();
                validator.Validate(board);
            }
            catch (BoardException exc)
            {
                actual = exc.ExceptionType;
            }
            Assert.AreEqual(expectedResult, actual);
        }
    }
}
