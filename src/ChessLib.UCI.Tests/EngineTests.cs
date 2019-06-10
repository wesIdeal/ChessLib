// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.UCI.Commands.FromEngine;
using NUnit.Framework;

namespace ChessLib.UCI.Tests
{
    [TestFixture]
    public class EngineTests
    {
        public bool isFinished = false;
        public const string sfDirectory = @".\stockfish_10_x64.exe";
        private Task _task;
        private Engine _eng;
        private UCIEngineInformation _engInfo;

        [SetUp]
        public void Setup()
        {
            _eng = new Engine("StockFish", sfDirectory, null);
            _task = _eng.StartAsync();
            _eng.DebugEventExecuted += (sender, dbg) =>
            {
                Console.WriteLine(dbg.ToString());
            };
        }

        [Test]
        public async Task TestUCICommand()
        {
            using (var er = new EngineRunner())
            {
                var idx = er.AddEngine("StockFish10", sfDirectory, null);
                _eng = er.Engines[idx];
                await _eng.StartAsync();
                var i = 0;

                Assert.AreEqual("Stockfish 10 64", _engInfo.Name);
                Assert.AreEqual("T. Romstad, M. Costalba, J. Kiiski, G. Linscott", _engInfo.Author);
                Assert.AreEqual(19, _engInfo.Options.Length);
                Assert.AreEqual(true, _engInfo.UCIOk);
            }
        }

        [Test]
        public async Task TestQuitCommand()
        {
            using (var er = new EngineRunner())
            {
                var idx = er.AddEngine("StockFish10", sfDirectory, null);
                _eng = er.Engines[idx];
                await _eng.StartAsync();

            }
        }
        [Test]
        public void TestIsReadyCommand()
        {
            _eng.ResponseReceived += (sender, obj) =>
            {
                if (obj.IssuedCommand != Commands.ToEngine.AppToUCICommand.IsReady) return;
                var readyResponse = obj.ResponseObject as ReadyOk;
                Assert.IsNotNull(readyResponse);
                Assert.AreEqual("readyok", readyResponse.EngineResponse);
            };

            _eng.SendIsReady();
            _eng.SendQuit();
            _task.Wait();
        }




    }
}
