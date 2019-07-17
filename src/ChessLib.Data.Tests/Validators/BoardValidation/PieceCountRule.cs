using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
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
            var board = new BoardInfo(fen);
            var rule = new Data.Validators.BoardValidation.Rules.PieceCountRule();
            var actual = rule.Validate(board);
            Assert.AreEqual(expectedException, actual);
        }

    }
}
