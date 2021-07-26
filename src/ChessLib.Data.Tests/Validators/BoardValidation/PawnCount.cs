using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.BoardValidation;
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

        private static readonly FenTextToBoard FenReader = new FenTextToBoard();

        [TestCase("rnbqkbnr/pppppppp/8/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.WhiteTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardExceptionType.BlackTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
            BoardExceptionType.WhiteTooManyPawns | BoardExceptionType.BlackTooManyPawns)]
        [TestCase(BoardConstants.FenStartingPosition, BoardExceptionType.None)]
        public static void TestPawnCounts(string fen, BoardExceptionType expectedResult)
        {
            var actual = BoardExceptionType.None;
            try
            {
                var board = FenReader.Translate(fen);
                var validator = new BoardValidator();
                validator.Validate(board);
            }
            catch (BoardException exc)
            {
                actual = exc.ExceptionType;
            }

            Assert.AreEqual(expectedResult, actual);
        }
    }
}