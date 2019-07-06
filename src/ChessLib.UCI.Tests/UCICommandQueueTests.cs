using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.EngineInterface.UCI.Commands;
using ChessLib.EngineInterface.UCI.Commands.ToEngine;

namespace ChessLib.UCI.Tests
{
    [TestFixture]
    public class CommandQueueTests
    {
        private CommandQueue q = new CommandQueue();
        ~CommandQueueTests()
        {
            q.Dispose();
        }
        [SetUp]
        public void Setup()
        {
            q.Clear();
        }
        [Test]
        public void TestEnqueueNonInterruptCommand()
        {
            var expectedSize = 5;
            for (int i = 0; i < expectedSize; i++)
            {
                q.Enqueue(new CommandInfo(AppToUCICommand.IsReady));
            }
            Assert.AreEqual(expectedSize, q.Count());
        }

        [TestCase(AppToUCICommand.Quit)]
        [TestCase(AppToUCICommand.Stop)]
        public void TestEnqueueInterruptCommand_ClearsExistingCommands(AppToUCICommand interruptCommand)
        {
            var expectedSize = 5;
            for (int i = 0; i < expectedSize; i++)
            {
                q.Enqueue(new CommandInfo(AppToUCICommand.IsReady));
            }
            Assert.AreEqual(expectedSize, q.Count());
            q.Enqueue(new CommandInfo(interruptCommand));
            Assert.AreEqual(1, q.Count());
        }

        [TestCase(AppToUCICommand.Quit, true)]
        [TestCase(AppToUCICommand.Stop, true)]
        [TestCase(AppToUCICommand.UCI, false)]
        public void TestEnqueueInterruptCommand_SendsAppropriateEvent(AppToUCICommand interruptCommand, bool shouldReceiveInterruptEvent)
        {
            var expectedSize = 5;
            var timeout = 100;
            var expectedWaitHandle = shouldReceiveInterruptEvent ? 0 : WaitHandle.WaitTimeout;
            for (int i = 0; i < expectedSize; i++)
            {
                q.Enqueue(new CommandInfo(AppToUCICommand.IsReady));
            }
            Assert.AreEqual(expectedSize, q.Count());
            q.Enqueue(new CommandInfo(interruptCommand));
            var waitHandle = WaitHandle.WaitAny(new[] { q.InterruptIssued }, timeout);
            Assert.AreEqual(expectedWaitHandle, waitHandle);
        }

        [Test]
        public void TestClear()
        {
            q.Enqueue(new CommandInfo(AppToUCICommand.IsReady));
            q.Enqueue(new CommandInfo(AppToUCICommand.IsReady));
            q.Enqueue(new CommandInfo(AppToUCICommand.IsReady));
            q.Clear();
            Assert.IsEmpty(q);
        }
    }
}
