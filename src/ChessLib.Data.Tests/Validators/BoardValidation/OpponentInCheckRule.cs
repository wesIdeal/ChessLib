using ChessLib.Core;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.BoardValidation;
using NUnit.Framework;

namespace ChessLib.Data.Validators.BoardValidation.Tests
{
    [TestFixture]
    public sealed class OpponentInCheckRule
    {
        [TestCase("8/8/8/8/8/8/6kQ/4K3 b - - 0 1", BoardExceptionType.None)]
        [TestCase("8/8/8/8/8/8/6kQ/4K3 w - - 0 1", BoardExceptionType.OppositeCheck)]
        public static void ValidateCheck(string fen, BoardExceptionType expectedException)
        {
            BoardExceptionType actual = BoardExceptionType.None;
            try
            {
                var board = new Board(fen);
                var boardValidator = new BoardValidator();
                boardValidator.Validate(board);
            }
            catch (BoardException be)
            {

                actual = be.ExceptionType;
            }
            Assert.AreEqual(expectedException, actual);
        }
    }
}
