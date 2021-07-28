#region

using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;
using NUnit.Framework;

#endregion

namespace ChessLib.Core.Tests.Validation.Validators.BoardValidation.Rules
{
    [TestFixture]
    public sealed class EnPassantSquareRule
    {
        [TestCase(BoardConstants.FenStartingPosition, BoardExceptionType.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/P7/8/1PPPPPPP/RNBQKBNR b KQkq a4 0 1", BoardExceptionType.BadEnPassant)]
        [TestCase("rnbqkbnr/1ppppppp/8/p7/P7/8/1PPPPPPP/RNBQKBNR w KQkq a5 0 2", BoardExceptionType.BadEnPassant)]
        [TestCase("rnbqkbnr/pppppppp/8/8/P7/8/1PPPPPPP/RNBQKBNR b KQkq a2 0 1", BoardExceptionType.BadEnPassant)]
        [TestCase("rnbqkbnr/1ppppppp/8/p7/P7/8/1PPPPPPP/RNBQKBNR w KQkq a7 0 2", BoardExceptionType.BadEnPassant)]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq f3 0 1", BoardExceptionType.BadEnPassant)]
        [TestCase("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 2", BoardExceptionType.BadEnPassant)]
        [TestCase("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", BoardExceptionType.None)]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", BoardExceptionType.None)]
        [TestCase("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e6 0 2", BoardExceptionType.BadEnPassant)]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e3 0 1", BoardExceptionType.BadEnPassant)]
        public static void TestEnPassant(string fen, BoardExceptionType expectedException)
        {
            var enPassantValidator =
                new EnPassantPositionRule();
            var board = new Board(fen, true);
            var actualExceptionType = enPassantValidator.Validate(board);
            Assert.AreEqual(expectedException, actualExceptionType);
        }

        [SetUp]
        public void Setup()
        {
            FenReader.BypassValidation = true;
        }
        private static readonly FenTextToBoard FenReader = new FenTextToBoard();
    }
}