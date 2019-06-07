using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ChessLib.UCI
{
    public delegate void ReceiveOutput(Guid engineId, string engineName, string strOutput);

    public struct UCICommandInfo
    {
        public readonly string Command;
        public readonly int ArgumentCount;
        public readonly string ExpectedResponse;
        public bool ExactMatch { get; }
        public string[] CommandArguments { get; private set; }
        public void SetCommandArguments(string[] arguments)
        {
            if ((arguments == null && ArgumentCount != 0) || arguments.Length < ArgumentCount)
            {
                throw new UCICommandException($"The {Command} command requires {ArgumentCount} arguments.");
            }
            CommandArguments = arguments ?? new string[] { };
        }
        public bool AwaitResponse => !string.IsNullOrEmpty(ExpectedResponse);

        public UCICommandInfo(UCICommandToEngine command)
        {
            ArgumentCount = UCICommandAttribute.GetExpectedArgCount(command);
            Command = UCICommandAttribute.GetCommandString(command);
            ExpectedResponse = UCICommandAttribute.GetExpectedResponse(command);
            ExactMatch = UCICommandAttribute.GetExactMatch(command);
            CommandArguments = new string[] { };
        }

        internal string GetFullCommand()
        {
            return $"{Command} {string.Join(" ", CommandArguments)}";
        }
    }

    public enum UCICommandToEngine
    {
        [UCICommand(command: "uci", expectedArgCount: 0, expectedResponse: UCIResponseFromEngine.UCIOk)]
        UCI,
        [UCICommand("isready", expectedArgCount:0, expectedResponse: UCIResponseFromEngine.Ready, exactMatch: true)]
        IsReady,
        [UCICommand("quit")]
        Quit,
        [UCICommand("stop")]
        Stop,
        PonderHit,
        [UCICommand("position")]
        Position

    }

    public enum UCIResponseFromEngine
    {
        [Description("readyok")]
        Ready,
        [Description("uciok")]
        UCIOk
    }

}
