﻿using System;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.EngineInterface.UCI;
using ChessLib.EngineInterface.UCI.Commands.FromEngine.Options;
using NUnit.Framework;

namespace ChessLib.EngineInterface.Tests
{
    [TestFixture]
    public class UCIResponseFactoryTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _factory = new UCIResponseFactory();
        }

        private UCIResponseFactory _factory;

        [Test]
        public void TestReadyOk()
        {
            var resp = _factory.MakeResponseArgs(BoardConstants.FenStartingPosition, "readyok");
            Assert.IsTrue(resp is ReadyOkResponseArgs);
            Assert.AreEqual("readyok", resp.ResponseText);
        }

        [Test]
        public void TestUCI()
        {
            UCIResponse obj = null;
            var str = "id name Stockfish 10 64\r\n" + "\r\n" +
                      "id author T.Romstad, M.Costalba, J.Kiiski, G.Linscott\r\n" +
                      "option name Debug Log File type string default\r\n" +
                      "uciok";
            foreach (var command in str.Split("\r\n"))
            {
                var resp = _factory.MakeResponseArgs(BoardConstants.FenStartingPosition, command);
                if (command == "uciok")
                {
                    Assert.IsTrue(resp.ResponseObject is UCIResponse);
                    obj = resp.ResponseObject as UCIResponse;
                }
                else
                {
                    Assert.IsNull(resp);
                }
            }
            Assert.IsNotNull(obj);
            Assert.AreEqual(1, obj.Options.Count());
            Assert.IsTrue(obj.Options[0] is UCIStringOption);
            Assert.IsTrue(obj.Name == "Stockfish 10 64");
            Assert.IsTrue(obj.Author == "T.Romstad, M.Costalba, J.Kiiski, G.Linscott");
            Assert.IsTrue(obj.UCIOk);
        }

        [TestCase("info depth 25 currmove g2g3 currmovenumber 16", typeof(InfoCalculationResponse))]
        [TestCase(
            "info depth 25 seldepth 35 multipv 1 score cp 51 upperbound nodes 14803677 nps 1531520 hashfull 996 tbhits 0 time 9666 pv e2e4 e7e6",
            typeof(PrincipalVariationResponse))]
        [TestCase("bestmove e2e4 ponder e7e6", typeof(BestMoveResponse))]
        public void TestInfo(string engineResponse, Type t)
        {
            var resp = _factory.MakeResponseArgs(BoardConstants.FenStartingPosition, engineResponse);
            Assert.IsTrue(resp.ResponseObject.GetType() == t);
        }
    }
}