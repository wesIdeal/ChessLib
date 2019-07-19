using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.BoardValidation
{
    [TestFixture]
    public sealed class PawnCountRule 
    {
        [OneTimeSetUp]
        public static void Setup()
        {

        }

        [TestCase("rnbqkbnr/pppppppp/8/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.WhiteTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.BlackTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
            BoardExceptionType.WhiteTooManyPawns | BoardExceptionType.BlackTooManyPawns)]
        [TestCase(FENHelpers.FENInitial, BoardExceptionType.None)]
        public static void TestPawnCounts(string fen, BoardExceptionType expectedResult)
        {
            BoardExceptionType actual = BoardExceptionType.None;
            try
            {
                var board = new BoardInfo(fen);
                board.ValidateBoard();
            }
            catch (BoardException exc)
            {
                actual = exc.ExceptionType;
            }
            Assert.AreEqual(expectedResult, actual);
        }
    }
}
