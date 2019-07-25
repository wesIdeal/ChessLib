using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.BoardValidation
{
    [TestFixture]
    public sealed class PieceCountRule
    {
        [TestCase(FENHelpers.FENInitial, BoardExceptionType.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/7N/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.WhiteTooManyPieces)]
        [TestCase("rnbqkbnr/pppppppp/7n/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.BlackTooManyPieces)]
        [TestCase("rnbqkbnr/pppppppp/7n/8/8/7N/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.BlackTooManyPieces | BoardExceptionType.WhiteTooManyPieces)]
        public static void TestPieceCounts(string fen, BoardExceptionType expectedException)
        {
            BoardExceptionType actual = BoardExceptionType.None;
            try
            {
                var board = new BoardInfo(fen);
                board.ValidateBoard();

            }
            catch (BoardException be)
            {

                actual = be.ExceptionType;
            }
            Assert.AreEqual(expectedException, actual);
        }

    }
}
