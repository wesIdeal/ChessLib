using System;

namespace ChessLib.EngineInterface.Commands.FromEngine
{

    [Flags]
    public enum EngineToAppCommand
    {
        [Command("NONE")]
        None = 0,
        [Command(command: "readyok", 0)]
        Ready = 1 << 1,
        [Command("uciok")]
        Ok = 1 << 2,
        [Command("bestmove")]
        BestMove = 1 << 3,
        [Command("ponder")]
        Ponder = 1 << 4,
        [Command("info")]
        Info = 1 << 5,
        [Command("uciok")]
        UCIOk = 1 << 6,
        [Command("id")]
        Id = 1 << 7,
        [Command("option")]
        Option = 1 << 8
    }

}
