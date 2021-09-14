using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.BoardValidation;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.BoardValidation.Rules
{
    [TestFixture]
    public sealed class PieceCountRule
    {
        private static readonly FenTextToBoard FenReader = new FenTextToBoard();

        [TestCase(BoardConstants.FenStartingPosition, BoardExceptionType.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/7N/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.WhiteTooManyPieces)]
        [TestCase("rnbqkbnr/pppppppp/7n/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.BlackTooManyPieces)]
        [TestCase("rnbqkbnr/pppppppp/7n/8/8/7N/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
            BoardExceptionType.BlackTooManyPieces | BoardExceptionType.WhiteTooManyPieces)]
        public static void TestPieceCounts(string fen, BoardExceptionType expectedException)
        {
            var actual = BoardExceptionType.None;
            try
            {
                var board = FenReader.Translate(fen);
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