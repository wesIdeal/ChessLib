// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            _eng = new Engine("StockFish", sfDirectory);

            _eng.DebugEventExecuted += (sender, dbg) =>
            {
                Console.WriteLine(dbg.ToString());
                Debug.WriteLine(dbg.ToString());
            };
            _eng.EngineCommunication += (sender, dbg) =>
             {
                 Debug.WriteLine(dbg.ToString());
             };
        }

        [Test]
        public async Task TestUCICommand()
        {
            _eng.ResponseReceived += (sender, obj) =>
            {
                if (!(obj.ResponseObject is UCIEngineInformation)) { return; }
                var responseObj = (UCIEngineInformation)obj.ResponseObject;
                Assert.IsNotNull(obj.ResponseObject);
                Assert.IsTrue(responseObj.UCIOk);
            };
            _eng.StateChanged += (sender, obj) =>
            {
                if (obj.StateChangeField != StateChangeField.UCIOk) return;
                Assert.IsNotNull(obj.StateChangeField);
                Assert.IsTrue(_eng.UCIOk);
                _eng.SendQuit();
            };
            _eng.SendUCI();
            _eng.SendIsReady();
            _task.Wait();
        }


        [Test]
        public void TestIsReadyCommand()
        {
            var executedHandler = false;
            var waitForCommand = new AutoResetEvent(false);
            _eng.StateChanged += (sender, obj) =>
            {
                if (obj.StateChangeField != StateChangeField.IsReady) return;
                executedHandler = true;
                waitForCommand.Set();
            };
            var t = _eng.StartAsync();
            _eng.SendIsReady();
            WaitHandle.WaitAny(new[] { waitForCommand }, 10 * 1000);
            Assert.IsTrue(executedHandler);
            t.Wait();
        }

        [Test]
        public void TestIsAnalyzingIsFalseCommand()
        {

            //_eng.ResponseReceived += (sender, obj) =>
            //{
            //    if (obj.IssuedCommand != Commands.ToEngine.AppToUCICommand.Go) { return; }
            //    var ro = obj.ResponseObject as InfoResponse;
            //    if (ro == null)
            //    {
            //        var bestMove = obj.ResponseObject as BestMoveResponse;
            //        Assert.IsNotNull(bestMove);
            //    }
            //    Assert.IsTrue(_eng.IsAnalyizing);
            //    _eng.SendStop();
            //};
            //_eng.EngineCommunication += (sender, obj) =>
            //{
            //    if (obj.CommandText == "stop")
            //    {
            //        Thread.Sleep(2000);
            //        Assert.IsFalse(_eng.IsAnalyizing);
            //    }
            //};
            //_eng.SendUCI();
            //_eng.SendIsReady();
            //_eng.SendGo(TimeSpan.FromSeconds(2));
            //_task.Wait();
        }




    }
}
