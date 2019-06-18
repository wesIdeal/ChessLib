using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.UCI.Commands.FromEngine;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.UCI.Tests.Commands.FromEngine.Options
{
    [TestFixture]
    public class InfoResponseFactoryTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _infoObj = new InfoResponseFactory();
        }
        [TestCase("bestmove e2e4 ponder e7e6", 796, 3372, FENHelpers.FENInitial)]
        [TestCase("bestmove d5f7 ponder f8f7", 2293, 3957, "r1b2rk1/pp1p1pp1/1b1p2B1/n1qQ2p1/8/5N2/P3RPPP/4R1K1 w - - 0 1")]
        [TestCase("bestmove e7e8q", 32060, null, "6k1/3RP3/8/8/8/8/8/4K3 w - - 0 60")]
        public void TestBestMove(string engineResponse, int bm, int? pm, string fen )
        {
            var bestMove = new MoveExt((ushort)bm);
            var ponderMove = pm.HasValue ? new MoveExt((ushort)pm) : null;
            var iResp = _infoObj.GetInfoResponse(fen, engineResponse);
            Assert.IsTrue(iResp is BestMoveResponse);
            var obj = iResp as BestMoveResponse;
            Assert.AreEqual(bestMove, obj.BestMove);
            Assert.AreEqual(ponderMove, obj.PonderMove);
           
        }
         [TestCase("bestmove e7e8q", "60. e8=Q# 1-0", "", "6k1/3RP3/8/8/8/8/8/4K3 w - - 0 60")]
        [TestCase("bestmove e2e4 ponder e7e6", "1. e4", "e6", FENHelpers.FENInitial)]
        [TestCase("bestmove e7e5 ponder g2g3", "1...e5", "2. g3", "rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq c3 0 1")]
        //[TestCase("bestmove d5f7 ponder f8f7", 2293, 3957, "r1b2rk1/pp1p1pp1/1b1p2B1/n1qQ2p1/8/5N2/P3RPPP/4R1K1 w - - 0 1")]
        public void TestBestMoveSAN(string engineResponse, string bmSAN, string pmSAN, string fen)
        {
            var iResp = _infoObj.GetInfoResponse(fen, engineResponse);
            Assert.IsTrue(iResp is BestMoveResponse);
            var obj = iResp as BestMoveResponse;
            Assert.AreEqual(bmSAN, obj.BestMoveSan);
            Assert.AreEqual(pmSAN, obj.PonderMoveSan);
            Assert.AreEqual(bmSAN + " " + pmSAN, obj.ToString());
        }

        static object[][] InfoStrings = new[]
        {
            new object[]{"info depth 25 seldepth 8 multipv 1 score mate 4 nodes 14765893 nps 1582794 hashfull 349 tbhits 0 time 9329 pv d5f7 f8f7 e2e8 f7f8 e8f8 g8f8 e1e8"},
            new object[]{"info depth 24 seldepth 33 multipv 3 score cp -575 nodes 14765893 nps 1582794 hashfull 349 tbhits 0 time 9329 pv g6e4 c5d5 e4d5 b6d8 f3d4 d8f6 e1d1 f6d4 d1d4 a5c6 d4g4 b7b6 g4g5 c8a6 e2d2 a8e8 f2f3 e8e1 g1f2 e1a1 f2g3 f8e8 d5e4 c6e5 f3f4"},
            new object[]{"info depth 24 seldepth 31 multipv 4 score cp -780 nodes 14765893 nps 1582794 hashfull 349 tbhits 0 time 9329 pv d5c5 d6c5 g6e4 b6d8 h2h3 d7d6 e1d1 a5c4 e4d5 c4b6 d5e4 f7f5 e4d5 g8h7 d5e6 c8e6 e2e6 d6d5 g2g3 d8f6 d1c1 b6a4 g3g4 f5g4 h3g4 a4c3"},
        };
        private InfoResponseFactory _infoObj;

        [Test]
        [TestCaseSource("InfoStrings")]
        public void TestInfoStringParsing(string info)
        {
            string fen = "r1b2rk1/pp1p1pp1/1b1p2B1/n1qQ2p1/8/5N2/P3RPPP/4R1K1 w - - 0 1";
           
            var resp = _infoObj.GetInfoResponse(fen, info);
            Assert.IsTrue(resp is PrincipalVariationResponse);
            var obj = resp as PrincipalVariationResponse;
            Assert.AreEqual(14765893, obj.Nodes);
            Console.Write($"({obj.ToString()})\t" + obj.SAN);
        }
    }
}
