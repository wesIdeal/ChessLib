using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace ChessLib.EngineInterface.UCI.Commands
{
    public class CommandQueue : ConcurrentQueue<CommandInfo>, IDisposable
    {
        public AutoResetEvent InterruptIssued = new AutoResetEvent(false);
        public AutoResetEvent CommandIssued = new AutoResetEvent(false);
        public readonly WaitHandle[] CommandIssuedEvents;

        public CommandQueue()
        {
            CommandIssuedEvents = new WaitHandle[] { CommandIssued, InterruptIssued };
        }

        public new void Enqueue(CommandInfo item)
        {
            if (item.CommandSent.IsInterruptCommand())
            {
                EnqueueInterruptEvent(item);
            }
            else
            {
                base.Enqueue(item);
                CommandIssued.Set();
            }

        }


        private void EnqueueInterruptEvent(CommandInfo item)
        {
            Clear();
            base.Enqueue(item);
            InterruptIssued.Set();
        }
        public void Clear()
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
