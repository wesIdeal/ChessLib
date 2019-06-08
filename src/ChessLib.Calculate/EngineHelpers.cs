using ChessLib.Data.MoveRepresentation;
using EnumsNET;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLib.Data.Helpers;
using System.Collections.Generic;
using System.IO;

namespace ChessLib.UCI
{
    public delegate void ReceiveOutput(Guid engineId, string engineName, string strOutput);

    public static class EngineHelpers
    {
        public static readonly string[] OptionKeywords = new[] { "name", "default", "min", "max", "var", "type" };
        private static string GetMoves(MoveExt[] moves)
        {
            if (moves != null && moves.Any())
            {
                StringBuilder sb = new StringBuilder("searchmoves");
                foreach (var move in moves)
                {
                    sb.Append($" {move.SourceIndex.IndexToSquareDisplay()}{move.DestinationIndex.IndexToSquareDisplay()}");
                }

                return sb.ToString().Trim();
            }

            return "";
        }

        public static void SendIsReady(this Engine engine)
        {
            var commandInfo = new UCICommandInfo(AppToUCICommand.IsReady);
            engine.IsReady = false;
            engine.QueueCommand(commandInfo);
        }

        public static void SendUCI(this Engine engine, OnCommandFinishedCallback onUciFinished = null)
        {
            var commandInfo = new UCICommandInfo(AppToUCICommand.UCI, onUciFinished);
            engine.QueueCommand(commandInfo);
        }

        private static void SendStop(this Engine engine)
        {
            var commandInfo = new UCICommandInfo(AppToUCICommand.Stop);
            engine.QueueCommand(commandInfo);
        }

        public static bool IsResponseTheExpectedResponse(this UCICommandInfo command, string engineResponse)
        {
            return command.ExpectedResponse == engineResponse || (command.ExactMatch == false && engineResponse.StartsWith(command.ExpectedResponse));
        }

        public static bool IsInterruptCommand(this AppToUCICommand command) =>
            (new AppToUCICommand[] { AppToUCICommand.Stop, AppToUCICommand.Quit }).Contains(command);

        public static void WriteToEngine(this StreamWriter writer, UCICommandInfo command)
        {
            writer.WriteLine(command.ToString());
            writer.Flush();
        }

        public static bool GetDefaultForCheckbox(this string option)
        {
            if (string.IsNullOrWhiteSpace(option)) { return false; }
            Debug.Assert(option.GetOptionType() == UCIOptionType.Check);
            var val = option.GetStringDefault();
            bool rv = false;
            if (bool.TryParse(val, out rv))
            {
                return rv;
            }
            return false;
        }

        public static string GetOptionName(this string option)
        {
            var rv = "";
            if (string.IsNullOrWhiteSpace(option)) { return rv; }
            var splitOptions = option.Split(' ').Select(x => x.Trim()).ToArray();
            var keyFound = false;
            var value = "";
            for (int i = 0; i < splitOptions.Length; i++)
            {

                string opt = (string)splitOptions[i];
                if (keyFound && OptionKeywords.Contains(opt)) { break; }
                if (keyFound && !string.IsNullOrWhiteSpace(opt))
                {
                    value += opt + " ";
                }
                if (opt == "name")
                {
                    keyFound = true;
                }
            }
            return value.Trim();
        }

        public static string GetValueForUCIKeyValuePair(this string option, string key)
        {
            Debug.Assert(string.IsNullOrEmpty(option) == false);
            var splitOptions = option.Split(' ').Select(x => x.Trim()).ToArray();
            var keyFound = false;
            var value = "";
            for (int i = 0; i < splitOptions.Length; i++)
            {
                string opt = (string)splitOptions[i];
                if (keyFound && !string.IsNullOrWhiteSpace(opt))
                {
                    value = opt;
                    break;
                }
                if (opt == key)
                {
                    keyFound = true;
                }
            }
            return value.Trim();
        }

        public static string[] GetComboOptionValues(this string option)
        {
            var splitOptions = option.Split(' ').Select(x => x.Trim()).ToArray();
            var keyFound = false;
            var rv = new List<string>();
            for (int i = 0; i < splitOptions.Length; i++)
            {
                string opt = (string)splitOptions[i];
                if (keyFound && !string.IsNullOrWhiteSpace(opt))
                {
                    rv.Add(opt);
                    keyFound = false;

                }
                if (opt == "var")
                {
                    keyFound = true;
                }
            }
            return rv.ToArray();
        }

        public static double? GetNumericDefault(this string option)
        {
            if (string.IsNullOrWhiteSpace(option)) { return null; }
            var val = option.GetValueForUCIKeyValuePair("default");
            if (double.TryParse(val, out double parsedVal))
            {
                return parsedVal;
            }
            return null;
        }

        public static string GetStringDefault(this string option)
        {
            if (string.IsNullOrWhiteSpace(option)) { return null; }
            var val = option.GetValueForUCIKeyValuePair("default");
            return val;
        }

        public static double? GetNumericOptionType(this string option, string key)
        {
            double rv = 0;
            if (string.IsNullOrWhiteSpace(option)) return null;
            var val = option.GetValueForUCIKeyValuePair(key);
            if (string.IsNullOrWhiteSpace(val)) return null;
            if (double.TryParse(val, out rv))
            {
                return rv;
            }
            return null;
        }

        public static UCIOptionType GetOptionType(this string option)
        {
            const string key = "type";
            if (string.IsNullOrWhiteSpace(option))
            {
                return UCIOptionType.Null;
            }
            var value = option.GetValueForUCIKeyValuePair(key);
            foreach (var uciOptionType in Enum.GetValues(typeof(UCIOptionType)).Cast<UCIOptionType>())
            {
                if (value == uciOptionType.GetEnumDesc())
                {
                    return uciOptionType;
                }
            }
            return UCIOptionType.Null;
        }

        public static string GetEnumDesc<T>(this T tEnum) where T : struct, Enum
        {
            return tEnum.AsString(EnumFormat.Description);
        }

        public static IUCIOption[] GetOptions(this IEnumerable<string> optionsResponse)
        {
            var rv = new List<IUCIOption>();
            foreach (var opt in optionsResponse)
            {
                switch (opt.GetOptionType())
                {
                    case UCIOptionType.Spin:
                        rv.Add(opt.ProcessSpinOption());
                        break;
                    case UCIOptionType.Check:
                        rv.Add(opt.ProcessCheckOption());
                        break;
                    case UCIOptionType.Combo:
                        rv.Add(opt.ProcessComboOption());
                        break;
                    case UCIOptionType.Button:
                        rv.Add(opt.ProcessButtonOption());
                        break;
                    case UCIOptionType.String:
                        rv.Add(opt.ProcessStringOption());
                        break;
                    default:
                        break;
                }
            }
            return rv.ToArray();
        }

        public static IUCIOption[] GetOptions(this string optionsResponse)
        {
            var options = optionsResponse.Split('\n').Select(x => x.Trim()).ToList();
            return options.GetOptions();
        }

        private static UCIButtonOption ProcessButtonOption(this string opt)
            => new UCIButtonOption() { Name = opt.GetOptionName() };

        private static UCIStringOption ProcessStringOption(this string opt)
            => new UCIStringOption() { Default = opt.GetStringDefault(), Name = opt.GetOptionName() };

        private static UCIComboOption ProcessComboOption(this string opt)
        {
            return new UCIComboOption()
            {
                Name = opt.GetOptionName(),
                Default = opt.GetStringDefault(),
                Options = opt.GetComboOptionValues()
            };
        }

        private static UCISpinOption ProcessSpinOption(this string opt)
        {
            return new UCISpinOption()
            {
                Name = opt.GetOptionName(),
                Min = opt.GetNumericOptionType("min"),
                Max = opt.GetNumericOptionType("max"),
                Default = opt.GetNumericDefault()
            };
        }

        private static UCICheckOption ProcessCheckOption(this string opt)
        {
            return new UCICheckOption()
            {
                Name = opt.GetOptionName(),
                Default = opt.GetDefaultForCheckbox()
            };
        }

        /// <summary>
        /// Starts a search for set depth
        /// </summary>
        /// <param name="eng"></param>
        /// <param name="nodesToSearch">search x nodes only</param>
        ///  /// <param name="depth">search x plies only</param>
        /// <param name="searchMoves">only consider these moves</param>
        public static void SendGo(this Engine eng, int? nodesToSearch, int depth,
            MoveExt[] searchMoves = null)
        {
            StringBuilder sb = new StringBuilder("go");
            sb.Append(GetMoves(searchMoves));
            if (nodesToSearch.HasValue) sb.Append($" nodes {nodesToSearch.Value}");
            sb.Append($" depth {depth}");
        }

        public static void SendQuit(this Engine engine)
        {
            var commandInfo = new UCICommandInfo(AppToUCICommand.Quit);
            engine.QueueCommand(commandInfo);
        }

        /// <summary>
        /// Starts a search for set amount of time
        /// </summary>
        /// <param name="eng"></param>
        /// <param name="searchDepth">search x plies only</param>
        /// <param name="nodesToSearch">search x nodes only</param>
        /// <param name="searchTime">Time to spend searching</param>
        /// <param name="searchMoves">only consider these moves</param>
        public static void SendGo(this Engine eng, int? nodesToSearch, TimeSpan searchTime,
            MoveExt[] searchMoves = null)
        {
            //StringBuilder sb = new StringBuilder("go");
            //sb.Append(GetMoves(searchMoves));
            //if (nodesToSearch.HasValue) sb.Append($" nodes {nodesToSearch.Value}");
            //var timeInMsToSearch = searchTime.TotalMilliseconds.ToString();
            //sb.Append($" movetime {timeInMsToSearch}");
            //eng.SendCommand(sb.ToString().Trim());

        }

        ///// <summary>
        ///// Starts a search for infinite amount of time. Must send "stop" command end search.
        ///// </summary>
        ///// <param name="eng"></param>
        ///// <param name="nodesToSearch">search x nodes only</param>
        ///// <param name="searchMoves">estrict search to these moves only</param>
        //public static void SendGo(this Engine eng, int? nodesToSearch, MoveExt[] searchMoves = null)
        //{
        //    StringBuilder sb = new StringBuilder("go");
        //    sb.Append(GetMoves(searchMoves));
        //    if (nodesToSearch.HasValue) sb.Append($" nodes {nodesToSearch.Value}");
        //    eng.SendCommand(sb.ToString().Trim());
        //}
    }
}
