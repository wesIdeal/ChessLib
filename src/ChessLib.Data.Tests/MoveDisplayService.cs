using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Core.Tests
{
    [TestFixture]
    public class PGNFormatterTests
    {
        private PgnFormatter<Move> _moveFormatter = new PgnFormatter<Move>(new PGNFormatterOptions());

        [TestCase("2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23", "d5f7", "23. Bxf7+")]
        public void TestCheckDisplay(string fen, string lan, string expected)
        {
            var game = new Game(fen);
            var lanToMove = new LanToMove();
            var move = lanToMove.Translate(lan);
            game.ApplyMove(move, MoveApplicationStrategy.ContinueMainLine);
            var pgnOutput = _moveFormatter.BuildPgn(game);
            Assert.AreEqual(expected, pgnOutput);
        }
    }
}