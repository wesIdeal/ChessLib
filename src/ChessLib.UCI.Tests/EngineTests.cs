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
using Microsoft.Win32.SafeHandles;
using Moq;
using NUnit.Framework;

namespace ChessLib.UCI.Tests
{
    [TestFixture]
    public class EngineTests
    {

        public string LastReceived;
        public bool isStarted = false;
        public const string sfDirectory = @".\stockfish_10_x64.exe";
        public Mock<UCIEngineProcess> _processMock = new Mock<UCIEngineProcess>();
        public UCIEngineProcess _process => _processMock.Object;

        public string LastCommand { get; private set; }

        public Engine _eng;
        public Task _engineTask;
        private StringWriter _stringWriter;


        [SetUp]
        public void Setup()
        {
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

            _eng = new Engine(Guid.NewGuid(), "moqEngine", "runMoqEngine.exe", _process);

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
            _eng.WriteToEngine(new ChessLib.UCI.Commands.CommandInfo(UCI.Commands.ToEngine.AppToUCICommand.UCI));
            Assert.AreEqual("uci", LastCommand);
        }





    }



}
