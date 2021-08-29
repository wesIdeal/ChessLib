using ChessLib.Core.Translate;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Translate
{
    [TestFixture]
    public class MoveToLanTests
    {
        private readonly LanToMove lanToMove = new LanToMove();
        private readonly MoveToLan moveToLan = new MoveToLan();

        [TestCase("2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23", "d5f7", "Bxf7+")]
        public void TestCheckDisplay(string fen, string lan, string expectedSan)
        {
            var move = lanToMove.Translate(lan);
            var actual = moveToLan.Translate(move);
            Assert.AreEqual(lan, actual);
        }
    }
}