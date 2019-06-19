// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.Data.Helpers;
using ChessLib.UCI.Commands.FromEngine;
using Microsoft.Win32.SafeHandles;
using Moq;
using NUnit.Framework;

namespace ChessLib.UCI.Tests
{
    [TestFixture]
    public class EngineTests
    {
        public Guid Id = Guid.NewGuid();
        public string LastReceived;
        public bool isStarted = false;
        public const string sfDirectory = @".\stockfish_10_x64.exe";
        public Mock<UCIEngineProcess> _processMock;
        public UCIEngineProcess _process => _processMock.Object;

        public string LastCommand { get; private set; }

        public Engine _eng;
        public Task _engineTask;
        private StringWriter _stringWriter;


        [SetUp]
        public void Setup()
        {
            _processMock = new Mock<UCIEngineProcess>(Id);
            _processMock.Setup<bool>(x => x.Start()).Callback(() =>
            {
                isStarted = true;

            }).Returns(true);

            _processMock.Setup(x => x.BeginErrorReadLine()).Callback(SetupErrorReadLine);
            _processMock.Setup(x => x.BeginOutputReadLine()).Callback(SetupOutputReadLine);
            _processMock.Setup(x => x.SetPriority(It.IsAny<ProcessPriorityClass>())).Callback<ProcessPriorityClass>(SetupSetPriority);
            _processMock.Setup(x => x.WaitForExit(It.IsAny<int>())).Returns(true);
            _processMock.Setup(s => s.SendCommmand(It.IsAny<string>())).Callback<string>((txt) =>
            {
                if (txt == "quit")
                {
                    _process.Close();
                }
                LastCommand = txt;
            });

            _eng = new Engine(Guid.NewGuid(), "moqEngine", "runMoqEngine.exe", _processMock.Object);

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

        [TearDown]
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
            _eng.WriteToEngine(new UCI.Commands.CommandInfo(UCI.Commands.ToEngine.AppToUCICommand.UCI));
            Assert.AreEqual("uci", LastCommand);
        }

        [Test]
        public void SendPosition_FEN()
        {
            var fen = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2";
            AutoResetEvent finished = new AutoResetEvent(false);
            EventHandler<DebugEventArgs> handler = (s, args) =>
            {
                if (args.DebugText.StartsWith("position"))
                {
                    try
                    {
                        Assert.AreEqual($"position fen {fen}", LastCommand);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        finished.Set();
                    }
                }
            };
            _eng.MessageSentFromQueue += handler;

            _processMock.Setup(x => x.SetPosition(It.IsAny<string>())).Verifiable();
            _eng.SendPosition(fen);
            _processMock.Verify(x => x.SetPosition(It.Is<string>(f => f == fen)), Times.Once);
            finished.WaitOne();
        }

        [Test]
        public void SendPosition_Moves()
        {
            AutoResetEvent finished = new AutoResetEvent(false);
            EventHandler<DebugEventArgs> handler = (s, args) =>
            {
                if (args.DebugText.StartsWith("position"))
                {
                    try
                    {
                        Assert.AreEqual($"position startpos moves e2e4 d7d5 e4d5", LastCommand);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        finished.Set();
                    }
                }
            };
            _eng.MessageSentFromQueue += handler;

            _processMock.Setup(x => x.SetPosition(It.IsAny<string>())).Verifiable();
            _eng.SendPosition(new[] { MoveHelpers.GenerateMove(12, 28), MoveHelpers.GenerateMove(51, 35), MoveHelpers.GenerateMove(28, 35) });
            var fenResult = "rnbqkbnr/ppp1pppp/8/3P4/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2";
            _processMock.Verify(x => x.SetPosition(It.Is<string>(f => f == fenResult)), Times.Once);
            finished.WaitOne();
        }
        [Test]
        public void SendPosition_FEN_PlusMoves()
        {
            var fen = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2";
            AutoResetEvent finished = new AutoResetEvent(false);
            EventHandler<DebugEventArgs> handler = (s, args) =>
            {
                if (args.DebugText.StartsWith("position"))
                {
                    try
                    {
                        Assert.AreEqual($"position fen {fen} moves e4d5", LastCommand);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        finished.Set();
                    }
                }
            };
            _eng.MessageSentFromQueue += handler;

            _processMock.Setup(x => x.SetPosition(It.IsAny<string>())).Verifiable();
            _eng.SendPosition(fen, new[] { MoveHelpers.GenerateMove(28, 35) });
            var fenResult = "rnbqkbnr/ppp1pppp/8/3P4/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2";
            _processMock.Verify(x => x.SetPosition(It.Is<string>(f => f == fenResult)), Times.Once);
            finished.WaitOne();
        }

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
                        Assert.AreEqual($"ucinewgame", LastCommand);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        finished.Set();
                    }
                }
            };

            _eng.MessageSentFromQueue += handler;
            _eng.SendNewGame();
            finished.WaitOne();
        }



    }



}
