using ChessLib.Data;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.BoardValidation
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
