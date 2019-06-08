using EnumsNET;
using System;

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

        private static readonly EnumFormat uciExactMatchFormat =
          Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<UCICommandAttribute>().ExactMatch.ToString());

        private string v1;
        private int v2;
        private UCIToAppCommand uCIOk;
        private UCIToAppCommand ready;
        private bool v3;

        public int ExpectedArgCount { get; }
        public string Command { get; set; }
        public string ExpectedResponse { get; set; }
        public bool ExactMatch { get; }

        public UCICommandAttribute(string command, int expectedArgCount = 0)
        {
            Command = command;
            ExpectedArgCount = expectedArgCount;
            ExactMatch = true;
        }

        public UCICommandAttribute(string command, int expectedArgCount, UCIToAppCommand expectedResponse) : this(command, expectedArgCount)
        {
            ExpectedResponse = expectedResponse.AsString(EnumFormat.Description);
        }

        public UCICommandAttribute(string command, int expectedArgCount, UCIToAppCommand expectedResponse, bool exactMatch) : this(command, expectedArgCount, expectedResponse)
        {
            this.ExactMatch = exactMatch;
        }

        public static string GetCommandString(AppToUCICommand command)
        {
            return command.AsString(uciCommandFormat);
        }

        public static string GetExpectedResponse(AppToUCICommand command)
        {
            return command.AsString(uciResponseFormat);
        }

        public static bool GetExactMatch(AppToUCICommand command)
        {
            return bool.Parse(command.AsString(uciExactMatchFormat));
        }

        public static int GetExpectedArgCount(AppToUCICommand command)
        {
            var countStr = command.AsString(argumentCountFormat);
            return int.Parse(countStr);
        }
    }

}
