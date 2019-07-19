using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data;
using ChessLib.EngineInterface.UCI;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;

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
        public void TestBestMove(string engineResponse, int bm, int? pm, string fen)
        {
            var translator = new MoveTranslatorService(fen);
            var bestMove = new MoveExt((ushort)bm);
            var ponderMove = pm.HasValue ? new MoveExt((ushort)pm) : null;
            var iResp = _infoObj.GetInfoResponse(engineResponse);
            Assert.IsTrue(iResp is BestMoveResponse);
            var obj = iResp as BestMoveResponse;
            var ponderMoveFromResponse = string.IsNullOrWhiteSpace(obj.PonderMoveLong)
                ? null
                : translator.FromLongAlgebraicNotation(obj.PonderMoveLong);

            Assert.AreEqual(bestMove, translator.FromLongAlgebraicNotation(obj.BestMoveLong));
            Assert.AreEqual(ponderMove, ponderMoveFromResponse);

        }
        [TestCase("bestmove e7e8q", "e8=Q# 1-0", null, "6k1/3RP3/8/8/8/8/8/4K3 w - - 0 60")]
        [TestCase("bestmove e2e4 ponder e7e6", "e4", "e6", FENHelpers.FENInitial)]
        [TestCase("bestmove e7e5 ponder g2g3", "e5", "g3", "rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq c3 0 1")]
        //[TestCase("bestmove d5f7 ponder f8f7", 2293, 3957, "r1b2rk1/pp1p1pp1/1b1p2B1/n1qQ2p1/8/5N2/P3RPPP/4R1K1 w - - 0 1")]
        public void TestBestMoveSAN(string engineResponse, string bmSAN, string pmSAN, string fen)
        {
            var trans = new MoveTranslatorService(fen);
            var iResp = _infoObj.GetInfoResponse(engineResponse);
            Assert.IsTrue(iResp is BestMoveResponse);
            var obj = iResp as BestMoveResponse;
            var tBm = trans.FromLongAlgebraicNotation(obj.BestMoveLong);
            var ponderMoveFromResponse = string.IsNullOrWhiteSpace(obj.PonderMoveLong)
                ? null
                : trans.FromLongAlgebraicNotation(obj.PonderMoveLong);
            Assert.AreEqual(bmSAN, tBm.SAN);
            Assert.AreEqual(pmSAN, ponderMoveFromResponse?.SAN);
            //Assert.AreEqual(bmSAN + " " + pmSAN, obj.ToString());
        }

        static readonly object[][] NodeCases = new[]
        {
            new object[]{"info depth 25 seldepth 8 multipv 1 score mate 4 nodes 14765893 nps 1582794 hashfull 349 tbhits 0 time 9329 pv d5f7 f8f7 e2e8 f7f8 e8f8 g8f8 e1e8"},
            new object[]{"info depth 24 seldepth 33 multipv 3 score cp -575 nodes 14765893 nps 1582794 hashfull 349 tbhits 0 time 9329 pv g6e4 c5d5 e4d5 b6d8 f3d4 d8f6 e1d1 f6d4 d1d4 a5c6 d4g4 b7b6 g4g5 c8a6 e2d2 a8e8 f2f3 e8e1 g1f2 e1a1 f2g3 f8e8 d5e4 c6e5 f3f4"},
            new object[]{"info depth 24 seldepth 31 multipv 4 score cp -780 nodes 14765893 nps 1582794 hashfull 349 tbhits 0 time 9329 pv d5c5 d6c5 g6e4 b6d8 h2h3 d7d6 e1d1 a5c4 e4d5 c4b6 d5e4 f7f5 e4d5 g8h7 d5e6 c8e6 e2e6 d6d5 g2g3 d8f6 d1c1 b6a4 g3g4 f5g4 h3g4 a4c3"},

        };
        static readonly object[][] ScoreStrings_Mate = new[]
        {   //fen, info, pv count, mate in _
             new object[]{"r1b2rk1/pp1p1pp1/1b1p2B1/n1qQ2p1/8/5N2/P3RPPP/4R1K1 w - - 0 1",
                 "info depth 8 seldepth 9 multipv 1 score mate 4 nodes 1412 nps 470666 tbhits 0 time 3 pv d5f7 f8f7 e2e8 f7f8 e8f8 g8f8 e1e8",
                 7, 4 }
        };
        static readonly object[][] ScoreStrings_Centipawn = new[]
        {
            new object[]{FENHelpers.FENInitial, "info depth 22 seldepth 30 multipv 1 score cp 71 nodes 6827891 nps 1481747 hashfull 999 tbhits 0 time 4608 pv e2e4 e7e5 g1f3 b8c6 f1b5 a7a6 b5a4 g8f6", 8, 71, Bound.None },
            new object[]{FENHelpers.FENInitial, "info depth 22 seldepth 30 multipv 1 score cp -100 nodes 6827891 nps 1481747 hashfull 999 tbhits 0 time 4608 pv e2e4 e7e5 g1f3 b8c6 f1b5 a7a6", 6, -100, Bound.None },
            new object[]{FENHelpers.FENInitial, "info depth 23 seldepth 33 multipv 1 score cp 72 lowerbound nodes 7776343 nps 1567179 hashfull 990 tbhits 0 time 4962 pv e2e4", 1, 72,  Bound.Lower },
            new object[]{ FENHelpers.FENInitial, "info depth 23 seldepth 33 multipv 1 score cp 72 upperbound nodes 7776343 nps 1567179 hashfull 990 tbhits 0 time 4962 pv e2e4", 1, 72,  Bound.Upper }
        };
        private InfoResponseFactory _infoObj;

        [Test]
        [TestCaseSource("ScoreStrings_Mate")]
        public void TestInfoPV_Scores_Mate(string fen, string info, int pvCount, int mateIn)
        {
            var resp = _infoObj.GetInfoResponse(info);
            Assert.IsTrue(resp is PrincipalVariationResponse);
            var obj = resp as PrincipalVariationResponse;
            Assert.AreEqual(pvCount, obj.Variation.Count());
            Assert.AreEqual(mateIn, obj.Score.MateInXMoves);
            Console.Write($"({obj.ToString()})\t");
        }

        [TestCase("info depth 25 currmove e2e3 currmovenumber 14", 25, 788, 14)]
        [TestCase("info depth 30 currmove a2a4 currmovenumber 15", 30, 536, 15)]
        [TestCase("info depth 1 currmove g2g3 currmovenumber 16", 1, 918, 16)]
        [TestCase("info depth 4 currmove h2h4 currmovenumber 17", 4, 991, 17)]
        public void TestInfoCalc(string info, int depth, int move, int moveNumber)
        {
            var resp = _infoObj.GetInfoResponse(info);
            Assert.IsTrue(resp is InfoCalculationResponse);
            var obj = resp as InfoCalculationResponse;
            var mv = new MoveExt((ushort)move);
            Assert.AreEqual(depth, obj.Depth);
            Assert.AreEqual(mv, obj.CurrentMove);
            Assert.AreEqual(moveNumber, obj.CurrentMoveNumber);
            Console.Write($"({obj.ToString()})\t");
        }

        [Test]
        [TestCaseSource("ScoreStrings_Centipawn")]
        public void TestInfoPV_Scores_Centipawn(string fen, string info, int pvCount, int centipawn, Bound scoreBound)
        {
            var resp = _infoObj.GetInfoResponse(info);
            Assert.IsTrue(resp is PrincipalVariationResponse);
            var obj = resp as PrincipalVariationResponse;
            Assert.AreEqual(pvCount, obj.Variation.Count());
            Assert.AreEqual(centipawn, obj.Score.CentipawnScore);
            Assert.AreEqual(scoreBound, obj.Score.Bound);
            Console.Write($"({obj.ToString()})\t");
        }

        [Test]
        [TestCaseSource("NodeCases")]
        public void TestInfoPV_Nodes(string info)
        {
            var resp = _infoObj.GetInfoResponse(info);
            Assert.IsTrue(resp is PrincipalVariationResponse);
            var obj = resp as PrincipalVariationResponse;
            Assert.AreEqual(14765893, obj.Nodes);
            Console.Write($"({obj.ToString()})\t");
        }
    }
}
