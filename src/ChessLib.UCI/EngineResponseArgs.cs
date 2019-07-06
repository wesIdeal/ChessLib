using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.ToEngine;
using ChessLib.UCI.Commands.FromEngine;
using System;
using System.Diagnostics.Tracing;

namespace ChessLib.UCI
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
    public class EngineCommunicationArgs : EventArgs
    {
        public enum TextSource { Engine, UI }
        public EngineCommunicationArgs(TextSource source, string commandText)
        {
            CommandSource = source;
            CommandText = commandText;
        }
        public TextSource CommandSource { get; set; }
        public string CommandText { get; set; }

        /// <summary>
        /// Can be used to quickly write debug text
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            var sourceStr = CommandSource == TextSource.Engine ? "Engine" : "UI";
            var sourceDelimiter = "> ";
            return $"{sourceStr}{sourceDelimiter}{CommandText}";
        }
    }

}
