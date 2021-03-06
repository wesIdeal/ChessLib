﻿using System;
using System.Collections.Generic;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;
using ChessLib.EngineInterface.UCI.Commands.ToEngine;
using EnumsNET;

namespace ChessLib.EngineInterface.UCI.Commands
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandAttribute : Attribute
    {
        private static readonly EnumFormat ArgumentCountFormat =
            Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<CommandAttribute>().ExpectedArgCount.ToString());

        public static readonly EnumFormat UciCommandFormat =
            Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<CommandAttribute>().Command);

        public static readonly EnumFormat UCIResponseFormat =
           Enums.RegisterCustomEnumFormat(member =>
           {
               var responses = member.Attributes.Get<CommandAttribute>().ExpectedResponseDelimited;
               return responses;
           });

        private static readonly EnumFormat UCIExactMatchFormat =
          Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<CommandAttribute>().ExactMatch.ToString());



        public int ExpectedArgCount { get; }
        public string Command { get; set; }
        public IEnumerable<string> ExpectedResponses
        {
            get
            {
                var rv = new List<string>();
                foreach (var flag in ExpectedResponseObj.GetFlags())
                {
                    var strFlag = flag.AsString(UciCommandFormat);
                    rv.Add(strFlag);
                }
                return rv;
            }
        }
        public string ExpectedResponseDelimited => string.Join("|", ExpectedResponses);
        public EngineToAppCommand ExpectedResponseObj { get; set; }
        public bool ExactMatch { get; }

        public CommandAttribute(string command, int expectedArgCount = 0)
        {

            Command = command;
            ExpectedArgCount = expectedArgCount;
            ExactMatch = true;
            ExpectedResponseObj = EngineToAppCommand.None;
        }

        public CommandAttribute(string command, int expectedArgCount, EngineToAppCommand expectedResponse) : this(command, expectedArgCount)
        {
            ExpectedResponseObj = expectedResponse;
        }

        public CommandAttribute(string command, int expectedArgCount, EngineToAppCommand expectedResponse, bool exactMatch) : this(command, expectedArgCount, expectedResponse)
        {
            ExactMatch = exactMatch;
        }

        public static string GetCommandString(AppToUCICommand command)
        {
            return command.AsString(UciCommandFormat);
        }

       
        public static string[] GetExpectedResponse(AppToUCICommand command)
        {
            var str = command.AsString(UCIResponseFormat);
            return str.Split('|');
        }

        //public static UCIToAppCommand GetExpectedResponseFlags(AppToUCICommand command)
        //{
        //    return Enum.Parse(typeof(UCIToAppCommand), command.AsString(uciResponseFlagsFormat));
        //}

        public static bool GetExactMatch(AppToUCICommand command)
        {
            return bool.Parse(command.AsString(UCIExactMatchFormat));
        }

        public static int GetExpectedArgCount(AppToUCICommand command)
        {
            var countStr = command.AsString(ArgumentCountFormat);
            return int.Parse(countStr);
        }
    }

}
