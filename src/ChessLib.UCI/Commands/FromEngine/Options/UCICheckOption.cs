﻿namespace ChessLib.UCI.Commands.FromEngine.Options
{
    public class UCICheckOption : IUCIOption
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Default { get; set; }
    }
}