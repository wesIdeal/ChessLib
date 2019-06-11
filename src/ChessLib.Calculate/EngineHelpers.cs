using ChessLib.Data.MoveRepresentation;
using EnumsNET;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLib.Data.Helpers;
using System.Collections.Generic;
using System.IO;
using ChessLib.UCI.Commands.ToEngine;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.FromEngine;
using ChessLib.UCI.Commands.FromEngine.Options;

namespace ChessLib.UCI
{

    public static class EngineHelpers
    {
        public static readonly string[] OptionKeywords = new[] { "name", "default", "min", "max", "var", "type" };

        public static void SendIsReady(this Engine engine)
        {
            var commandInfo = new CommandInfo(AppToUCICommand.IsReady);
            engine.QueueCommand(commandInfo);
        }

        public static void SendUCI(this Engine engine)
        {
            var commandInfo = new CommandInfo(AppToUCICommand.UCI);
            engine.QueueCommand(commandInfo);
        }

        public static void SendStop(this Engine engine)
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Stop);
            engine.QueueCommand(commandInfo);
        }
        delegate bool CompareString(string response, string expected);
        static CompareString CompareExact = (engineResponse, expectedResponse) => engineResponse.Equals(expectedResponse);
        static CompareString CompareStart = (engineResponsetr, expectedResponse) => engineResponsetr.StartsWith(expectedResponse);

        public static bool IsResponseTheExpectedResponse(this CommandInfo command, string engineResponse, out EngineToAppCommand matchingFlag)
        {
            CompareString compare = command.ExactMatch ? CompareExact : CompareStart;
            string match;
            matchingFlag = EngineToAppCommand.None;
            if ((match = command.ExpectedResponses.FirstOrDefault(x => compare(engineResponse, x))) != null)
            {
                matchingFlag = Enums.Parse<EngineToAppCommand>(match, CommandAttribute.UciCommandFormat);
                return true;
            }
            return false;
        }

        public static EngineToAppCommand GetResponseType(string engineResponse)
        {
            var matchingFlag = EngineToAppCommand.None;
            var success = Enums.TryParse<EngineToAppCommand>(engineResponse, out matchingFlag, CommandAttribute.UciCommandFormat);
            return success ? matchingFlag : EngineToAppCommand.None;
        }

        public static bool IsInterruptCommand(this AppToUCICommand command) =>
            (new AppToUCICommand[] { AppToUCICommand.Stop, AppToUCICommand.Quit }).Contains(command);



        public static bool GetDefaultForCheckbox(this string option)
        {
            if (string.IsNullOrWhiteSpace(option)) { return false; }
            Debug.Assert(option.GetOptionType() == OptionType.Check);
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

        public static OptionType GetOptionType(this string option)
        {
            const string key = "type";
            if (string.IsNullOrWhiteSpace(option))
            {
                return OptionType.Null;
            }
            var value = option.GetValueForUCIKeyValuePair(key);
            foreach (var uciOptionType in Enum.GetValues(typeof(OptionType)).Cast<OptionType>())
            {
                if (value == uciOptionType.GetEnumDesc())
                {
                    return uciOptionType;
                }
            }
            return OptionType.Null;
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
                    case OptionType.Spin:
                        rv.Add(opt.ProcessSpinOption());
                        break;
                    case OptionType.Check:
                        rv.Add(opt.ProcessCheckOption());
                        break;
                    case OptionType.Combo:
                        rv.Add(opt.ProcessComboOption());
                        break;
                    case OptionType.Button:
                        rv.Add(opt.ProcessButtonOption());
                        break;
                    case OptionType.String:
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



        public static void SendLinesToSearch(this Engine eng, int lines)
        {
            eng.QueueCommand(new SetOption("MultiPV", lines.ToString()));
        }

        public static void SendQuit(this Engine engine)
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Quit);
            engine.QueueCommand(commandInfo);
        }

        public static string UCIMovesFromMoveObjects(this IEnumerable<MoveExt> moves)
        {
            if (moves == null || !moves.Any())
            {
                return "";
            }
            return string.Join(" ", moves.Select(m =>
            {
                var pString = m.MoveType == Types.Enums.MoveType.Promotion ?
                    char.ToLower(PieceHelpers.GetCharFromPromotionPiece(m.PromotionPiece)).ToString() : "";
                return $"{m.SourceIndex.IndexToSquareDisplay()}{m.DestinationIndex.IndexToSquareDisplay()}{pString}";
            }));
        }

        /// <summary>
        /// Starts a search for set amount of time
        /// </summary>
        /// <param name="eng"></param>
        /// <param name="searchTime">Time to spend searching</param>
        /// <param name="searchMoves">only consider these moves</param>
        public static void SendGo(this Engine eng, TimeSpan searchTime, MoveExt[] searchMoves = null)
        {
            eng.QueueCommand(new Go(searchTime, searchMoves));
        }

        public static void SetNumberOfLinesToCalculate(this Engine eng, double numberOfLines)
        {
            eng.SetOption("MultiPV", numberOfLines.ToString());
        }

        public static void SetOption(this Engine eng, string optionName, string value)
        {
            var option = new SetOption(optionName, value);
            eng.QueueCommand(option);
        }
    }
}
