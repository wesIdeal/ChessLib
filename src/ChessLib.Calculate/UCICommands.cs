using EnumsNET;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ChessLib.UCI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UCICommand : Attribute
    {
        private static readonly EnumFormat argumentCountFormat =
            Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<UCICommand>().ExpectedArgCount.ToString());

        private static readonly EnumFormat uciCommandFormat =
            Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<UCICommand>().Command);

        public int ExpectedArgCount { get; }
        public string Command { get; set; }

        

        public UCICommand(string command, int expectedArgCount = 0)
        {
            Command = command;
            ExpectedArgCount = expectedArgCount;
        }
        public static string GetCommandString(CommandToUCI command)
        {
            return command.AsString(uciCommandFormat);
        }

        public static int GetExpectedArgCount(CommandToUCI command)
        {
            var countStr = command.AsString(argumentCountFormat);
            return int.Parse(countStr);
        }
    }
    public delegate void ReceiveOutput(Guid engineId, string engineName, string strOutput);

    public enum CommandToUCI
    {
        [UCICommand("uci")]
        UCI,
        [UCICommand("isready")]
        IsReady,
        [UCICommand("quit")]
        Quit,
        Stop,
        PonderHit,
        [UCICommand("position")]
        Position

    }
    
}
