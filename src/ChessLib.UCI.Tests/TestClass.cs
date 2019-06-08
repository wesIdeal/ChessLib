// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ChessLib.UCI.Tests
{
    [TestFixture]
    public class EngineRunnerTests
    {
        public bool isFinished = false;
        public const string sfDirectory = @".\stockfish_10_x64.exe";
        private Task _task;
        private Engine _eng;
        private UCIEngineInformation _engInfo;
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
        public async Task TestIsReadyCommand()
        {
            Task task;
            using (var eng = new Engine("SF", sfDirectory, null))
            {


                task = eng.StartAsync();
                eng.SendIsReady();
                eng.SendQuit();
                task.Wait();
                var i = 0;

                Assert.AreEqual("Stockfish 10 64", _engInfo.Name);
                Assert.AreEqual("T. Romstad, M. Costalba, J. Kiiski, G. Linscott", _engInfo.Author);
                Assert.AreEqual(19, _engInfo.Options.Length);
                Assert.AreEqual(true, _engInfo.UCIOk);
            }
        }




    }
}
