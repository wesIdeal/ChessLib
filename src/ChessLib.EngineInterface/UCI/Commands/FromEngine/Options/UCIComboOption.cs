﻿namespace ChessLib.EngineInterface.UCI.Commands.FromEngine.Options
{
    public class UCIComboOption : UCIOption<string>
    {
        public string Default { get; set; }
        public string[] Options { get; set; }
    }
}
