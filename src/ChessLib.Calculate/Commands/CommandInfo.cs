using ChessLib.Data.MoveRepresentation;
using ChessLib.UCI.Commands.FromEngine;
using ChessLib.UCI.Commands.ToEngine;
using EnumsNET;
using System;
using System.Linq;
using System.Text;

namespace ChessLib.UCI.Commands
{

    public class CommandInfo
    {
        public readonly AppToUCICommand CommandSent;
        public readonly string CommandText;
        public readonly int ArgumentCount;
        public readonly string[] ExpectedResponses;
        public readonly bool ExactMatch;
        public string[] CommandArguments { get; private set; }

        public void SetCommandArguments(string[] arguments)
        {
            if ((arguments == null && ArgumentCount != 0) || arguments.Length < ArgumentCount)
            {
                throw new UCICommandException($"The {CommandText} command requires {ArgumentCount} arguments.");
            }
            CommandArguments = arguments ?? new string[] { };
        }

        public bool AwaitResponse => ExpectedResponses.Any();

        public static CommandInfo IsReady()
        {
            return new CommandInfo(AppToUCICommand.IsReady);
        }

        public static CommandInfo UCI()
        {
            return new CommandInfo(AppToUCICommand.UCI);
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
            return CommandText;
        }
    }

}
