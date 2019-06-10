using System.Threading;
using System.Collections.Concurrent;

namespace ChessLib.UCI.Commands
{
    public class CommandQueue : ConcurrentQueue<CommandInfo>
    {
        public static AutoResetEvent InterruptIssued = new AutoResetEvent(false);
        public static AutoResetEvent CommandIssued = new AutoResetEvent(false);
        public static readonly AutoResetEvent[] CommandIssuedEvents = new[] { CommandQueue.CommandIssued, CommandQueue.InterruptIssued };
        public new void Enqueue(CommandInfo item)
        {
            if (item.CommandSent.IsInterruptCommand())
            {
                EnqueueInterruptEvent(item);
                return;
            }
            base.Enqueue(item);
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
    }
}
