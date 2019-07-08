using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
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

        [TestCase("rnbqkbnr/pppppppp/8/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardException.WhiteTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardException.BlackTooManyPawns)]
        [TestCase("rnbqkbnr/pppppppp/1p6/8/8/1P6/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
            BoardException.WhiteTooManyPawns | BoardException.BlackTooManyPawns)]
        [TestCase(FENHelpers.FENInitial, BoardException.None)]
        public static void TestPawnCounts(string fen, BoardException expectedResult)
        {
            var board = new BoardInfo(fen);
            var pawnCountRule = new Data.Validators.BoardValidation.Rules.PawnCountRule();
            var actual = pawnCountRule.Validate(board);
            Assert.AreEqual(expectedResult, actual);
        }
    }
}
