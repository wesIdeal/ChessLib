using ChessLib.Data.MoveRepresentation;
using System;

namespace ChessLib.UCI
{
    public delegate void OnCommandFinishedCallback(Guid engineId, object state, string rawOutput);
    /// <summary>
    /// To get response from 'go' command for best move and ponder
    /// </summary>
    /// <param name="engineId">Guid of Engine</param>
    /// <param name="bestMoves"></param>
    /// <param name="rawOutput"></param>
    public delegate void OnStopCommandFinishedCallback(Guid engineId, MoveExt[] bestMoves, string rawOutput);

    public class UCICommandInfo
    {
        public readonly AppToUCICommand CommandSent;
        public readonly string CommandText;
        public readonly int ArgumentCount;
        public readonly string ExpectedResponse;
        public readonly bool ExactMatch;
        public string[] CommandArguments { get; private set; }
        public OnCommandFinishedCallback OnCommandFinished;
        public void SetCommandArguments(string[] arguments)
        {
            if ((arguments == null && ArgumentCount != 0) || arguments.Length < ArgumentCount)
            {
                throw new UCICommandException($"The {CommandText} command requires {ArgumentCount} arguments.");
            }
            CommandArguments = arguments ?? new string[] { };
        }

        public bool AwaitResponse => !string.IsNullOrEmpty(ExpectedResponse);

        public UCICommandInfo(AppToUCICommand command, OnCommandFinishedCallback onCommandFinishedCallback = null)
        {
            ArgumentCount = UCICommandAttribute.GetExpectedArgCount(command);
            CommandSent = command;
            CommandText = UCICommandAttribute.GetCommandString(command);
            ExpectedResponse = UCICommandAttribute.GetExpectedResponse(command);
            ExactMatch = UCICommandAttribute.GetExactMatch(command);
            OnCommandFinished = onCommandFinishedCallback;
            CommandArguments = new string[] { };
        }

        internal string GetFullCommand()
        {
            return $"{CommandText} {string.Join(" ", CommandArguments)}";
        }

        public override string ToString()
        {
            return CommandText;
        }
    }

}
