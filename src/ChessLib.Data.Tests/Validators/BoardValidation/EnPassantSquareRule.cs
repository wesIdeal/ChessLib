#region

using ChessLib.Core;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Helpers;
using ChessLib.Data.Helpers;
using Moq;
using NUnit.Framework;

#endregion

namespace ChessLib.Data.Tests.Validators.BoardValidation
{
    [TestFixture]
    public sealed class EnPassantSquareRule
    {
        [TestCase(FENHelpers.FENInitial, BoardExceptionType.None)]
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
                new Core.Validation.Validators.BoardValidation.Rules.EnPassantSquareRule();
            var board = fen.BoardFromFen(out var activePlayer, out _, out var enPassantIndex, out _, out _, false);
            var actualExceptionType = enPassantValidator.ValidateEnPassantSquare(board, enPassantIndex, activePlayer);
            Assert.AreEqual(expectedException, actualExceptionType);
        }

        [Test]
        public static void ValidateShouldCallValidateEnPassantSquare()
        {
            var board = new Board();
            var epMock = new Mock<Core.Validation.Validators.BoardValidation.Rules.EnPassantSquareRule>();
            epMock.Setup(
                    x => x.ValidateEnPassantSquare(It.IsAny<ulong[][]>(), It.IsAny<ushort?>(), It.IsAny<Color>()))
                .Returns(BoardExceptionType.None).Verifiable();
            epMock.Object.Validate(board);
            epMock.Verify(x => x.ValidateEnPassantSquare(board.Occupancy, null, Color.White));
        }
    }
}