using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.Services;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.BoardValidation;
using ChessLib.Data.Helpers;
using NUnit.Framework;

namespace ChessLib.Data.Validators.BoardValidation.Tests
{
    [TestFixture]
    public sealed class PieceCountRule
    {
        private static readonly FenReader FenReader = new FenReader();
        [TestCase(FenReader.FENInitial, BoardExceptionType.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/7N/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.WhiteTooManyPieces)]
        [TestCase("rnbqkbnr/pppppppp/7n/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.BlackTooManyPieces)]
        [TestCase("rnbqkbnr/pppppppp/7n/8/8/7N/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.BlackTooManyPieces | BoardExceptionType.WhiteTooManyPieces)]
        public static void TestPieceCounts(string fen, BoardExceptionType expectedException)
        {
            BoardExceptionType actual = BoardExceptionType.None;
            try
            {
                var board = FenReader.GetBoard(fen);
                var validator = new BoardValidator();
                validator.Validate(board);
            }
            catch (BoardException be)
            {

                actual = be.ExceptionType;
            }
            Assert.AreEqual(expectedException, actual);
        }

    }
}
