using System;
using System.Diagnostics.Tracing;

namespace ChessLib.EngineInterface
{
    public class DebugEventArgs : EventArgs
    {
        public EventLevel EventLevel = EventLevel.Informational;
        protected const string TimeFormat = "mm:ss.fff";
        public string DebugText { get; set; }
        public readonly DateTime Received = DateTime.Now;
        public DebugEventArgs(string txt)
        {
            DebugText = txt;
        }

        public new string ToString()
        {
            return $"{Received.ToString(TimeFormat)}\t{DebugText.Replace("\r\n", "\r\n\t")}";
        }
    }
   
}
