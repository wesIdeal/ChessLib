using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
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
                var board = new Board(fen);
                var validator = new BoardValidator(board);
                validator.Validate(true);
            }
            catch (BoardException exc)
            {
                actual = exc.ExceptionType;
            }
            Assert.AreEqual(expectedResult, actual);
        }
    }
}
