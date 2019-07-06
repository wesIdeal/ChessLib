// NUnit 3 t0ests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.EngineInterface;
using ChessLib.EngineInterface.UCI.Commands;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;
using ChessLib.EngineInterface.UCI.Commands.ToEngine;
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
        public bool IsStarted;
        public const string StockfishPath = @".\stockfish_10_x64.exe";
        public Mock<EngineProcess> ProcessMock;
        public EngineProcess Process => ProcessMock.Object;

        public string LastCommand { get; private set; }

        public UCIEngine Eng;
        public Task EngineTask;

        UCIEngineStartupArgs _startup;
        //[SetUp]
        public void Setup()
        {

            _startup = new UCIEngineStartupArgs(Guid.NewGuid(), "mocked engine", "runMockEngine.exe");
            ProcessMock = new Mock<EngineProcess>(new UCIEngineMessageSubscriber(null));
            ProcessMock.Setup(x => x.Start()).Callback(() =>
            {
                IsStarted = true;

            }).Returns(true);

            ProcessMock.Setup(x => x.BeginErrorReadLine()).Callback(SetupErrorReadLine);
            ProcessMock.Setup(x => x.BeginOutputReadLine()).Callback(SetupOutputReadLine);
            ProcessMock.Setup(x => x.SetPriority(It.IsAny<ProcessPriorityClass>())).Callback<ProcessPriorityClass>(SetupSetPriority);
            ProcessMock.Setup(x => x.WaitForExit(It.IsAny<int>())).Returns(true);
            ProcessMock.SetupGet(x => x.ProcessId).Returns(420);
            ProcessMock.Setup(s => s.SendCommandToEngine(It.IsAny<string>())).Callback<string>(txt =>
            {
                if (txt == "uci")
                {
                    ProcessMock.Object.MessageSubscriber.ProcessEngineResponse(EngineConstants.UCIResponse);
                }
                if (txt == "isready")
                {
                    ProcessMock.Object.MessageSubscriber.ProcessEngineResponse(EngineConstants.ReadyOk);
                }
                if (txt == "quit")
                {
                    Process.Close();
                }
                LastCommand = txt;
            });
            Eng = new UCIEngine(_startup, null, Process);
            (ProcessMock.Object.MessageSubscriber as UCIEngineMessageSubscriber).EngineResponseCallback =
                Eng.ResponseReceived;
            Eng.DebugEventExecuted += (s, arg) =>
            {
                Console.WriteLine(arg.DebugText);
            };
            EngineTask = Eng.StartAsync();
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
            Eng.SendQuit();
            EngineTask.Wait(10 * 1000);
            Eng.Dispose();
        }

        [Test]
        public void WriteToEngineShouldSendCommandToWriter()
        {
            //Arrange
            Eng.WriteToEngine(new CommandInfo(AppToUCICommand.UCI));
            Assert.AreEqual("uci", LastCommand);
        }

        [Test]
        public void TestRunOfRealEngine()
        {
            var startupArgs = new UCIEngineStartupArgs(Guid.NewGuid(), "StockFish", "stockfish_10_x64.exe");
            using (var engine = new UCIEngine(startupArgs))
            {
                //engine.DebugEventExecuted += (s, o) => { Console.WriteLine(o.ToString()); };
                EngineTask = engine.StartAsync();
                engine.SetOption("Debug Log File", "c:\\temp\\sf.log.txt");
                engine.EngineCalculationReceived += (s, o) =>
                {
                    if (o.ResponseObject == null)
                    {
                        Debug.WriteLine("****Calc Result Was Null****");
                    }
                    else if (o.ResponseObject.ResponseType == CalculationResponseTypes.BestMove)
                    {
                        var bm = o.ResponseObject as BestMoveResponse;
                        Console.WriteLine($"Bestmove found: {bm.BestMove}. Pondering: {bm.PonderMove}");
                        engine.SendQuit();
                       
                    }
                    else if (o.ResponseObject.ResponseType == CalculationResponseTypes.PrincipalVariation)
                    {
                        var pv = o.ResponseObject as PrincipalVariationResponse;
                        Console.WriteLine($"Principal variation {pv.PVOrdinal} found, starting with {pv.Variation[0].SAN}.");
                    }
                };
                engine.SetOption("MultiPV", "3");
                engine.SendPosition("rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq - 0 1");
                engine.SendGo(TimeSpan.FromSeconds(20));
               EngineTask.Wait();
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

            Eng.SendPosition();
            finished.WaitOne();
        }




    }



}
