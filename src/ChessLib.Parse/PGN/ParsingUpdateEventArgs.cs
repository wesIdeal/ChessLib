using System;

namespace ChessLib.Parse.PGN
{
    public class ParsingUpdateEventArgs : EventArgs
    {
        public int NumberComplete { get; set; }
        public int Maximum { get; set; }
        public string Label { get; set; }
        public bool IsIndeterminate { get; set; }
    }
}