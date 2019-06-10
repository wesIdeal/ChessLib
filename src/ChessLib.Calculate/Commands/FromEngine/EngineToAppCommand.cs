using System;
using System.Linq;
using System.Runtime.Serialization;

namespace ChessLib.UCI.Commands
{

    [Flags]
    public enum EngineToAppCommand
    {

        None = 0,
        [Command("readyok")]
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
        UCIOk,
        [Command("id")]
        Id,
        [Command("option")]
        Option
    }

}
