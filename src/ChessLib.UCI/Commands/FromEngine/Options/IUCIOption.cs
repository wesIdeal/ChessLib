﻿namespace ChessLib.UCI.Commands.FromEngine.Options
{
    public interface IUCIOption
    {
        string Name { get; set; }
        string Value { get; set; }
    }
}