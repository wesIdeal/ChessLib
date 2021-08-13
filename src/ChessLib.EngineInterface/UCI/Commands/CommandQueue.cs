using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ChessLib.EngineInterface.UCI.Commands
{

    public class CommandQueue : ConcurrentQueue<CommandInfo>, IDisposable
    {
        public AutoResetEvent InterruptIssued = new AutoResetEvent(false);
        public AutoResetEvent CommandIssued = new AutoResetEvent(false);
        private readonly WaitHandle[] _standardWaitHandles;

        public WaitHandle[] CommandIssuedEvents
        {
            get { return _standardWaitHandles; }
        }



        public CommandQueue()
        {
            _standardWaitHandles = new WaitHandle[] { CommandIssued, InterruptIssued };
        }

        public new void Enqueue(CommandInfo item)
        {
            if (item is InterruptCommand)
            {
                EnqueueInterruptEvent(item);
            }
            else
            {
                base.Enqueue(item);
                CommandIssued.Set();
            }

        }

        public new bool TryDequeue(out CommandInfo commandToIssue)
        {
            var success = base.TryDequeue(out commandToIssue);
            if (success)
            {
                CommandIssued.Set();
            }
            return success;
        }

        private void EnqueueInterruptEvent(CommandInfo item)
        {
            Clear();
            base.Enqueue(item);
            InterruptIssued.Set();
        }
        public new void Clear()
        {
            while (TryDequeue(out var command))
            {
                Debug.WriteLine($"Dequeued {command}");
            }
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                CommandIssued.Dispose();
                InterruptIssued.Dispose();
                IsDisposed = true;
            }
        }
    }
}
