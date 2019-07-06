// NUnit 3 t0ests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.ToEngine;
using Moq;
using NUnit.Framework;

namespace ChessLib.UCI.Tests
{
    internal class EngineConstants
    {
        public const string UCIResponse =
            "id name Stockfish 10 64\r\nid author T. Romstad, M. Costalba, J. Kiiski, G. Linscott\r\n\r\noption name Debug Log File type string default\r\noption name Contempt type spin default 24 min -100 max 100\r\noption name Analysis Contempt type combo default Both var Off var White var Black var Both\r\noption name Threads type spin default 1 min 1 max 512\r\noption name Hash type spin default 16 min 1 max 131072\r\noption name Clear Hash type button\r\noption name Ponder type check default false\r\noption name MultiPV type spin default 1 min 1 max 500\r\noption name Skill Level type spin default 20 min 0 max 20\r\noption name Move Overhead type spin default 30 min 0 max 5000\r\noption name Minimum Thinking Time type spin default 20 min 0 max 5000\r\noption name Slow Mover type spin default 84 min 10 max 1000\r\noption name nodestime type spin default 0 min 0 max 10000\r\noption name UCI_Chess960 type check default false\r\noption name UCI_AnalyseMode type check default false\r\noption name SyzygyPath type string default <empty>\r\noption name SyzygyProbeDepth type spin default 1 min 1 max 100\r\noption name Syzygy50MoveRule type check default true\r\noption name SyzygyProbeLimit type spin default 7 min 0 max 7\r\nuciok";

        public const string ReadyOk = "readyok";
    }
    [TestFixture]
    public class EngineTests
    {
        public Guid Id = Guid.NewGuid();
        public string LastReceived;
        public bool isStarted;
        public const string sfDirectory = @".\stockfish_10_x64.exe";
        public Mock<EngineProcess> _processMock;
        public EngineProcess _process => _processMock.Object;

        public string LastCommand { get; private set; }

        public UCIEngine _eng;
        public Task _engineTask;
        private StringWriter _stringWriter;

        UCIEngineStartupArgs startup;
        //[SetUp]
        public void Setup()
        {

            startup = new UCIEngineStartupArgs(Guid.NewGuid(), "mocked engine", "runMockEngine.exe");
            _processMock = new Mock<EngineProcess>(new UCIEngineMessageSubscriber(null));
            _processMock.Setup(x => x.Start()).Callback(() =>
            {
                isStarted = true;

            }).Returns(true);

            _processMock.Setup(x => x.BeginErrorReadLine()).Callback(SetupErrorReadLine);
            _processMock.Setup(x => x.BeginOutputReadLine()).Callback(SetupOutputReadLine);
            _processMock.Setup(x => x.SetPriority(It.IsAny<ProcessPriorityClass>())).Callback<ProcessPriorityClass>(SetupSetPriority);
            _processMock.Setup(x => x.WaitForExit(It.IsAny<int>())).Returns(true);
            _processMock.SetupGet(x => x.ProcessId).Returns(420);
            _processMock.Setup(s => s.SendCommandToEngine(It.IsAny<string>())).Callback<string>(txt =>
            {
                if (txt == "uci")
                {
                    _processMock.Object.MessageSubscriber.ProcessEngineResponse(EngineConstants.UCIResponse);
                }
                if (txt == "isready")
                {
                    _processMock.Object.MessageSubscriber.ProcessEngineResponse(EngineConstants.ReadyOk);
                }
                if (txt == "quit")
                {
                    _process.Close();
                }
                LastCommand = txt;
            });
            _eng = new UCIEngine(startup, null, _process);
            (_processMock.Object.MessageSubscriber as UCIEngineMessageSubscriber).EngineResponseCallback =
                _eng.ResponseReceived;
            _eng.DebugEventExecuted += (s, arg) =>
            {
                Console.WriteLine(arg.DebugText);
            };
            _engineTask = _eng.StartAsync();
        }

        private void SetupSetPriority(ProcessPriorityClass priority)
        {

        }



        private void SetupOutputReadLine()
        {

        }

        private void SetupErrorReadLine()
        {

        }

        //[TearDown]
        public void TearDown()
        {
            _eng.SendQuit();
            _engineTask.Wait(10 * 1000);
            _eng.Dispose();
        }

        [Test]
        public void WriteToEngineShouldSendCommandToWriter()
        {
            //Arrange
            _eng.WriteToEngine(new CommandInfo(AppToUCICommand.UCI));
            Assert.AreEqual("uci", LastCommand);
        }

        [Test]
        public void TestRunOfRealEngine()
        {
            var startupArgs = new UCIEngineStartupArgs(Guid.NewGuid(), "StockFish", "stockfish_10_x64.exe");
            using (var engine = new UCIEngine(startupArgs))
            {
                engine.DebugEventExecuted += (s, o) => { Console.WriteLine(o.ToString()); };
                _engineTask = engine.StartAsync();
                engine.SetOption("Debug Log File", ".\\sf.log.txt");
                engine.EngineResponseObjectReceived += (s, o) =>
                {
                    if (o == null) return;
                    var obj = o.ResponseObject;
                };
                engine.SendUCI();
                engine.SendIsReady();
                engine.SendQuit();
                var b = _engineTask.Wait(10 * 1000);
            }
        }

        //[Test]
        //public void SendPosition_FEN()
        //{
        //    var fen = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2";
        //    AutoResetEvent finished = new AutoResetEvent(false);
        //    EventHandler<DebugEventArgs> handler = (s, args) =>
        //    {
        //        if (args.DebugText.StartsWith("position"))
        //        {
        //            try
        //            {
        //                Assert.AreEqual($"position fen {fen}", LastCommand);
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }
        //            finally
        //            {
        //                finished.Set();
        //            }
        //        }
        //    };
        //    _eng.MessageSentFromQueue += handler;

        //    _processMock.Setup(x => x.SetPosition(It.IsAny<string>())).Verifiable();
        //    _eng.SendPosition(fen);
        //    _processMock.Verify(x => x.SetPosition(It.Is<string>(f => f == fen)), Times.Once);
        //    finished.WaitOne();
        //}

        //[Test]
        //public void SendPosition_Moves()
        //{
        //    AutoResetEvent finished = new AutoResetEvent(false);
        //    EventHandler<DebugEventArgs> handler = (s, args) =>
        //    {
        //        if (args.DebugText.StartsWith("position"))
        //        {
        //            try
        //            {
        //                Assert.AreEqual($"position startpos moves e2e4 d7d5 e4d5", LastCommand);
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }
        //            finally
        //            {
        //                finished.Set();
        //            }
        //        }
        //    };
        //    _eng.MessageSentFromQueue += handler;

        //    _processMock.Setup(x => x.SetPosition(It.IsAny<string>())).Verifiable();
        //    _eng.SendPosition(new[] { MoveHelpers.GenerateMove(12, 28), MoveHelpers.GenerateMove(51, 35), MoveHelpers.GenerateMove(28, 35) });
        //    var fenResult = "rnbqkbnr/ppp1pppp/8/3P4/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2";
        //    _processMock.Verify(x => x.SetPosition(It.Is<string>(f => f == fenResult)), Times.Once);
        //    finished.WaitOne();
        //}
        //[Test]
        //public void SendPosition_FEN_PlusMoves()
        //{
        //    var fen = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2";
        //    AutoResetEvent finished = new AutoResetEvent(false);
        //    EventHandler<DebugEventArgs> handler = (s, args) =>
        //    {
        //        if (args.DebugText.StartsWith("position"))
        //        {
        //            try
        //            {
        //                Assert.AreEqual($"position fen {fen} moves e4d5", LastCommand);
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }
        //            finally
        //            {
        //                finished.Set();
        //            }
        //        }
        //    };
        //    _eng.MessageSentFromQueue += handler;

        //    _processMock.Setup(x => x.SetPosition(It.IsAny<string>())).Verifiable();
        //    _eng.SendPosition(fen, new[] { MoveHelpers.GenerateMove(28, 35) });
        //    var fenResult = "rnbqkbnr/ppp1pppp/8/3P4/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2";
        //    _processMock.Verify(x => x.SetPosition(It.Is<string>(f => f == fenResult)), Times.Once);
        //    finished.WaitOne();
        //}

        [Test]
        public void SendPosition_NewGame()
        {
            AutoResetEvent finished = new AutoResetEvent(false);
            EventHandler<DebugEventArgs> handler = (s, args) =>
            {
                if (args.DebugText.StartsWith("ucinewgame"))
                {
                    try
                    {
                        Assert.AreEqual("ucinewgame", LastCommand);
                    }
                    finally
                    {
                        finished.Set();
                    }
                }
            };

            _eng.SendPosition();
            finished.WaitOne();
        }




    }



}
