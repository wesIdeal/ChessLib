using System.Threading;
using System.Collections.Concurrent;

namespace ChessLib.UCI
{
    public class UCICommandQueue : ConcurrentQueue<UCICommandInfo>
    {
        public static AutoResetEvent InterruptIssued = new AutoResetEvent(false);
        public new void Enqueue(UCICommandInfo item)
        {
            if (item.CommandSent.IsInterruptCommand())
            {
                EnqueueInterruptEvent(item);
                return;
            }
            base.Enqueue(item);
        }

        private void EnqueueInterruptEvent(UCICommandInfo item)
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
