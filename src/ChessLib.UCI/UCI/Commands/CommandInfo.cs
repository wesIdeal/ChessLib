using System;
using System.Linq;
using System.Threading;
using ChessLib.EngineInterface.UCI.Commands.ToEngine;

namespace ChessLib.EngineInterface.UCI.Commands
{
    public class InterruptCommand : CommandInfo
    {
        public InterruptCommand(AppToUCICommand command) : base(command)
        {
        }
    }
    public class AwaitableCommandInfo : CommandInfo, IDisposable
    {


        public AwaitableCommandInfo(AppToUCICommand command) : base(command)
        {
            if (ExpectedResponses.Length != 1)
            {
                throw new ArgumentException("Awaitable Command must have one expected response.");
            }

            ExpectedResponse = ExpectedResponses[0];
        }


        public ManualResetEvent ResetEvent = new ManualResetEvent(false);
        public string ExpectedResponse { get; }

        public bool IsResponseExpected(string response)
        {
            return ExpectedResponse == response;
        }

        public void Dispose()
        {
            ResetEvent?.Dispose();
        }
    }


    public class CommandInfo
    {

        public readonly AppToUCICommand CommandSent;
        public string CommandText { get; }
        public readonly int ArgumentCount;
        public readonly string[] ExpectedResponses;
        public readonly bool ExactMatch;


        public string[] CommandArguments { get; private set; }

        public void SetCommandArguments(string[] arguments)
        {
            CommandArguments = arguments ?? new string[] { };
        }


        public CommandInfo(AppToUCICommand command)
        {
            ArgumentCount = CommandAttribute.GetExpectedArgCount(command);
            CommandSent = command;
            CommandText = CommandAttribute.GetCommandString(command);
            // ExpectedResponseFlags = command.
            ExpectedResponses = CommandAttribute.GetExpectedResponse(command);
            ExactMatch = CommandAttribute.GetExactMatch(command);
            CommandArguments = new string[] { };
        }

        internal string GetFullCommand()
        {
            return $"{CommandText} {string.Join(" ", CommandArguments)}";
        }

        public override string ToString()
        {
            if (CommandArguments.Any())
            {
                return GetFullCommand();
            }
            return CommandText;
        }




    }
}


