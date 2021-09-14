using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.BoardValidation;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.BoardValidation.Rules
{
    [TestFixture]
    public sealed class OpponentInCheckRule
    {
        private static readonly FenTextToBoard FenReader = new FenTextToBoard();

        [TestCase("8/8/8/8/8/8/6kQ/4K3 b - - 0 1", BoardExceptionType.None)]
        [TestCase("8/8/8/8/8/8/6kQ/4K3 w - - 0 1", BoardExceptionType.OppositeCheck)]
        public static void ValidateCheck(string fen, BoardExceptionType expectedException)
        {
            var actual = BoardExceptionType.None;
            try
            {
                var board = FenReader.Translate(fen);
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