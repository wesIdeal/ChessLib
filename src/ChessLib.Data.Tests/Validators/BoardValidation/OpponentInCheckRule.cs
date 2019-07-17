using ChessLib.Data;
using ChessLib.Data.Types.Enums;
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

            var board = new BoardInfo(fen);
            var rule = new Data.Validators.BoardValidation.Rules.OpponentInCheckRule();
            var actual = rule.Validate(board);
            Assert.AreEqual(expectedException, actual);
        }
    }
}
