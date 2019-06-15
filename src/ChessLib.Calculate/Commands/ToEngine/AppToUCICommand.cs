using System.ComponentModel;

namespace ChessLib.UCI.Commands.ToEngine
{
    public enum AppToUCICommand
    {
        [Command(command: "uci", expectedArgCount: 0, expectedResponse: EngineToAppCommand.Ok | EngineToAppCommand.UCIOk, exactMatch: false)]
        UCI,
        [Command("isready", expectedArgCount: 0, expectedResponse: EngineToAppCommand.Ready, exactMatch: true)]
        IsReady,
        [Command("quit")]
        Quit,
        [Command("stop", 0, expectedResponse: EngineToAppCommand.BestMove)]
        Stop,
        [Description("ponderhit")]
        PonderHit,
        [Command("position")]
        Position,
        [Command("go", 0, EngineToAppCommand.BestMove | EngineToAppCommand.Info)]
        Go,
        [Command("setoption", 2)]
        SetOption,
        [Command("name")]
        Option_Name,
        [Command("value")]
        Option_Value,
        [Command("ucinewgame")]
        NewGame,


    }

}
