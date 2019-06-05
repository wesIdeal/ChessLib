// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace ChessLib.UCI.Tests
{
    [TestFixture]
    public class EngineRunnerTests
    {
        public bool isFinished = false;
        public const string sfDirectory = @".\stockfish_10_x64.exe";
        [Test]
        public void TestMethod()
        {
            var er = new EngineRunner();
            var idx = er.AddEngine("StockFish10", sfDirectory, null, receiveOutput, Guid.NewGuid());
            er.Engines[idx].Start();
            er.Engines[idx].SendCommand(CommandToUCI.Position, "rnbqkbnr/ppp1pppp/8/3p4/2P5/8/PP1PPPPP/RNBQKBNR w KQkq - 0 2");
            er.Engines[idx].SendGo(3, TimeSpan.FromSeconds(3));
            while (!isFinished)
            {

            }

            er.Engines[idx].Stop();
        }

        

        private void receiveOutput(Guid engineId, string engineName, string strOutput)
        {
            Console.WriteLine($"{DateTime.Now.TimeOfDay}\t{strOutput}");
            if (strOutput.StartsWith("bestmove"))
            {
                isFinished = true;
            }
        }
    }
}
