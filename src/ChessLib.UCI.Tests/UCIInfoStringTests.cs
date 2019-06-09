using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.UCI.Tests
{

    [TestFixture]
    public class UCIInfoStringTests
    {
        static object[][] InfoStrings = new[]
        {
            new object[]{"info depth 25 seldepth 8 multipv 1 score mate 4 nodes 14765893 nps 1582794 hashfull 349 tbhits 0 time 9329 pv d5f7 f8f7 e2e8 f7f8 e8f8 g8f8 e1e8"},
            new object[]{"info depth 24 seldepth 33 multipv 3 score cp -575 nodes 14765893 nps 1582794 hashfull 349 tbhits 0 time 9329 pv g6e4 c5d5 e4d5 b6d8 f3d4 d8f6 e1d1 f6d4 d1d4 a5c6 d4g4 b7b6 g4g5 c8a6 e2d2 a8e8 f2f3 e8e1 g1f2 e1a1 f2g3 f8e8 d5e4 c6e5 f3f4"},
            new object[]{"info depth 24 seldepth 31 multipv 4 score cp -780 nodes 14765893 nps 1582794 hashfull 349 tbhits 0 time 9329 pv d5c5 d6c5 g6e4 b6d8 h2h3 d7d6 e1d1 a5c4 e4d5 c4b6 d5e4 f7f5 e4d5 g8h7 d5e6 c8e6 e2e6 d6d5 g2g3 d8f6 d1c1 b6a4 g3g4 f5g4 h3g4 a4c3"},
        };
        readonly string fen = "r1b2rk1/pp1p1pp1/1b1p2B1/n1qQ2p1/8/5N2/P3RPPP/4R1K1 w - - 0 1";
        public UCIInfoStringTests()
        {

        }
        [Test]
        [TestCaseSource("InfoStrings")]
        public void TestInfoStringParsing(string info)
        {
            var infoObj = new UCIInfoString(fen, info);
            Assert.AreEqual(14765893, infoObj.Nodes);
        }

    }
}
