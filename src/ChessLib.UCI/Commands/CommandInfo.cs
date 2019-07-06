using System.Linq;
using ChessLib.EngineInterface.Commands.ToEngine;

namespace ChessLib.EngineInterface.Commands
{

    public class CommandInfo
    {
        public readonly AppToUCICommand CommandSent;
        public string CommandText { get; private set; }
        public readonly int ArgumentCount;
        public readonly string[] ExpectedResponses;
        public readonly bool ExactMatch;
        public string[] CommandArguments { get; private set; }

        public void SetCommandArguments(string[] arguments)
        {
            CommandArguments = arguments ?? new string[] { };
        }

        public bool AwaitResponse => ExpectedResponses.Any();

       
        protected CommandInfo()
        {

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
            if(CommandArguments.Any())
            {
                return GetFullCommand();
            }
            return CommandText;
        }

       


    }
}


