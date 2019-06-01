using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.BoardValidation
{
    [TestFixture]
    public sealed class PieceCountRule
    {
        [TestCase(FENHelpers.FENInitial, BoardException.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/7N/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardException.WhiteTooManyPieces)]
        [TestCase("rnbqkbnr/pppppppp/7n/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardException.BlackTooManyPieces)]
        [TestCase("rnbqkbnr/pppppppp/7n/8/8/7N/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardException.BlackTooManyPieces | BoardException.WhiteTooManyPieces)]
        public static void TestPieceCounts(string fen, BoardException expectedException)
        {
            var board = new BoardInfo(fen);
            var rule = new ChessLib.Validators.BoardValidators.Rules.PieceCountRule();
            var actual = rule.Validate(board);
            Assert.AreEqual(expectedException, actual);
        }

    }
}
