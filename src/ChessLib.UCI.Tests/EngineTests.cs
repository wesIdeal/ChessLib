// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        private Engine _eng;

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
        public void TestUCICommand()
        {
            using (var waitHandle = new AutoResetEvent(false))

            {

                _eng.StateChanged += (sender, obj) =>
                {
                    if (obj.StateChangeField != StateChangeField.UCIOk) return;
                    Assert.IsNotNull(obj.StateChangeField);
                    Assert.IsTrue(_eng.UCIOk);
                    waitHandle.Set();

                };

                var t = _eng.StartAsync();
                WaitHandle.WaitAll(new[] { waitHandle });
                _eng.SendQuit();
                t.Wait();
            }
        }


        [Test]
        public void TestIsReadyCommand()
        {
            var executedHandler = false;
            using (var waitForCommand = new AutoResetEvent(false))
            {
                _eng.StateChanged += (sender, obj) =>
                {
                    if (obj.StateChangeField != StateChangeField.IsReady) return;
                    executedHandler = true;
                    waitForCommand.Set();

                };
                Assert.IsFalse(_eng.IsReady);
                var t = _eng.StartAsync();
                WaitHandle.WaitAny(new[] { waitForCommand }, 10 * 1000);
                Assert.IsTrue(_eng.IsReady);
                Assert.IsTrue(executedHandler);
                _eng.SendQuit();
                t.Wait();
                Assert.IsTrue(t.Status == TaskStatus.RanToCompletion);
            }
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
