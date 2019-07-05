using System.Threading;
using System.Collections.Concurrent;
using System;

namespace ChessLib.UCI.Commands
{
    public class CommandQueue : ConcurrentQueue<CommandInfo>, IDisposable
    {
        public AutoResetEvent InterruptIssued = new AutoResetEvent(false);
        public AutoResetEvent CommandIssued = new AutoResetEvent(false);
        private bool _isDisposed = false;
        public readonly WaitHandle[] CommandIssuedEvents;

        public CommandQueue()
        {
            CommandIssuedEvents = new[] { CommandIssued, InterruptIssued };
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
            while (TryDequeue(out _)) ;
        }

        public bool IsDisposed => _isDisposed;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                CommandIssued.Dispose();
                InterruptIssued.Dispose();
                _isDisposed = true;
            }
        }
    }
}
