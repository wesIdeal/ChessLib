using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ChessLib.UCI
{

    public enum AppToUCICommand
    {
        [UCICommand(command: "uci", expectedArgCount: 0, expectedResponse: UCIToAppCommand.Ok, exactMatch: false)]
        UCI,
        [UCICommand("isready", expectedArgCount: 0, expectedResponse: UCIToAppCommand.Ready, exactMatch: true)]
        IsReady,
        [UCICommand("quit")]
        Quit,
        [UCICommand("stop")]
        Stop,
        [Description("ponderhit")]
        PonderHit,
        [UCICommand("position")]
        Position,
        [UCICommand("go", 0, UCIToAppCommand.BestMove)]
        Go
    }

    public enum UCIToAppCommand
    {
        [Description("readyok")]
        Ready,
        [Description("uciok")]
        Ok,
        [Description("bestmove")]
        BestMove,
        [Description("ponder")]
        Ponder,
    }

}
