using System.Linq;
using System.Threading;
using ChessLib.EngineInterface.UCI.Commands;
using ChessLib.EngineInterface.UCI.Commands.ToEngine;
using NUnit.Framework;

namespace ChessLib.EngineInterface.Tests
{
    [TestFixture]
    public class CommandQueueTests
    {
        [SetUp]
        public void Setup()
        {
            q.Clear();
        }

        private readonly CommandQueue q = new CommandQueue();

        ~CommandQueueTests()
        {
            q.Dispose();
        }

        [Test]
        public void TestEnqueueNonInterruptCommand()
        {
            var expectedSize = 5;
            for (var i = 0; i < expectedSize; i++)
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
            for (var i = 0; i < expectedSize; i++)
            {
                q.Enqueue(new CommandInfo(AppToUCICommand.IsReady));
            }

            Assert.AreEqual(expectedSize, q.Count());
            q.Enqueue(new InterruptCommand(interruptCommand));
            Assert.AreEqual(1, q.Count());
        }

        [TestCase(AppToUCICommand.Quit, true)]
        [TestCase(AppToUCICommand.Stop, true)]
        public void TestEnqueueInterruptCommand_SendsAppropriateEvent(AppToUCICommand interruptCommand,
            bool shouldReceiveInterruptEvent)
        {
            var expectedSize = 5;
            var timeout = 100;
            var expectedWaitHandle = shouldReceiveInterruptEvent ? 0 : WaitHandle.WaitTimeout;
            for (var i = 0; i < expectedSize; i++)
            {
                q.Enqueue(new CommandInfo(AppToUCICommand.IsReady));
            }

            Assert.AreEqual(expectedSize, q.Count());
            q.Enqueue(new InterruptCommand(interruptCommand));
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