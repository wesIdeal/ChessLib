using EnumsNET;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ChessLib.UCI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UCICommandAttribute : Attribute
    {
        private static readonly EnumFormat argumentCountFormat =
            Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<UCICommandAttribute>().ExpectedArgCount.ToString());

        private static readonly EnumFormat uciCommandFormat =
            Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<UCICommandAttribute>().Command);

        private static readonly EnumFormat uciResponseFormat =
           Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<UCICommandAttribute>().ExpectedResponse);
        private string v1;
        private int v2;
        private UCIResponseFromEngine uCIOk;

        public int ExpectedArgCount { get; }
        public string Command { get; set; }
        public string ExpectedResponse { get; set; }

        public UCICommandAttribute(string command, int expectedArgCount = 0)
        {
            Command = command;
            ExpectedArgCount = expectedArgCount;
        }

        public UCICommandAttribute(string command, int expectedArgCount, UCIResponseFromEngine expectedResponse) : this(command, expectedArgCount)
        {
            ExpectedResponse = expectedResponse.AsString(EnumFormat.Description);
        }

        public static string GetCommandString(UCICommandToEngine command)
        {
            return command.AsString(uciCommandFormat);
        }

        public static string GetExpectedResponse(UCICommandToEngine command)
        {
            return command.AsString(uciResponseFormat);
        }

        public static int GetExpectedArgCount(UCICommandToEngine command)
        {
            var countStr = command.AsString(argumentCountFormat);
            return int.Parse(countStr);
        }
    }
    public delegate void ReceiveOutput(Guid engineId, string engineName, string strOutput);

    internal struct UCICommandInfo
    {
        public readonly string Command;
        public readonly int ArgumentCount;
        public readonly string ExpectedResponse;
        public bool AwaitResponse => !string.IsNullOrEmpty(ExpectedResponse);
        public UCICommandInfo(UCICommandToEngine command)
        {
            ArgumentCount = UCICommandAttribute.GetExpectedArgCount(command);
            Command = UCICommandAttribute.GetCommandString(command);
            ExpectedResponse = UCICommandAttribute.GetExpectedResponse(command);
        }
    }

    public enum UCICommandToEngine
    {
        [UCICommand("uci", 0, UCIResponseFromEngine.UCIOk)]
        UCI,
        [UCICommand("isready")]
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
