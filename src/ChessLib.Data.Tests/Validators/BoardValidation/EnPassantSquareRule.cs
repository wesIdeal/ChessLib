using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.BoardValidation
{
    [TestFixture]
    public sealed class EnPassantSquareRule
    {
        [TestCase(FENHelpers.FENInitial, BoardException.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/P7/8/1PPPPPPP/RNBQKBNR b KQkq a4 0 1", BoardException.BadEnPassant)]
        [TestCase("rnbqkbnr/1ppppppp/8/p7/P7/8/1PPPPPPP/RNBQKBNR w KQkq a5 0 2", BoardException.BadEnPassant)]
        [TestCase("rnbqkbnr/pppppppp/8/8/P7/8/1PPPPPPP/RNBQKBNR b KQkq a2 0 1", BoardException.BadEnPassant)]
        [TestCase("rnbqkbnr/1ppppppp/8/p7/P7/8/1PPPPPPP/RNBQKBNR w KQkq a7 0 2", BoardException.BadEnPassant)]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq f3 0 1", BoardException.BadEnPassant)]
        [TestCase("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 2", BoardException.BadEnPassant)]
        [TestCase("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", BoardException.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", BoardException.None)]
        [TestCase("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e6 0 2", BoardException.BadEnPassant)]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e3 0 1", BoardException.BadEnPassant)]
        public static void TestEnPassant(string fen, BoardException expectedException)
        {
            var board = new BoardInfo(fen);
            var rule = new ChessLib.Validators.BoardValidators.Rules.EnPassantSquareRule();
            var actual = rule.Validate(board);
            Assert.AreEqual(expectedException, actual);
        }

    }
}
