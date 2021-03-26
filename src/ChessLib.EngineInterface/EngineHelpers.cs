using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Data.Helpers;
using ChessLib.EngineInterface.UCI.Commands;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;
using ChessLib.EngineInterface.UCI.Commands.FromEngine.Options;
using ChessLib.EngineInterface.UCI.Commands.ToEngine;
using EnumsNET;

namespace ChessLib.EngineInterface
{

    public static class EngineHelpers
    {
        public static readonly string[] OptionKeywords = { "name", "default", "min", "max", "var", "type" };


        public static EngineToAppCommand GetResponseType(string engineResponse)
        {
            var success = Enums.TryParse<EngineToAppCommand>(engineResponse, true, out var matchingFlag, CommandAttribute.UciCommandFormat);
            return success ? matchingFlag : EngineToAppCommand.None;
        }

        public static bool IsInterruptCommand(this AppToUCICommand command) =>
            (new[] { AppToUCICommand.Stop, AppToUCICommand.Quit }).Contains(command);



        public static bool GetDefaultForCheckbox(this string option)
        {
            if (string.IsNullOrWhiteSpace(option)) { return false; }
            Debug.Assert(option.GetOptionType() == OptionType.Check);
            var val = option.GetStringDefault();
            if (bool.TryParse(val, out var rv))
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
            foreach (var fields in splitOptions)
            {
                string opt = fields;
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
            foreach (var field in splitOptions)
            {
                string opt = field;
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
            foreach (var field in splitOptions)
            {
                string opt = field;
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
            if (string.IsNullOrWhiteSpace(option)) return null;
            var val = option.GetValueForUCIKeyValuePair(key);
            if (string.IsNullOrWhiteSpace(val)) return null;
            if (double.TryParse(val, out var rv))
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

        public static List<IUCIOption> GetOptions(this IEnumerable<string> optionsResponse)
        {
            var rv = new List<IUCIOption>();
            foreach (var opt in optionsResponse)
            {
                var option = GetOption(opt);
                if (option != null) rv.Add(option);
            }
            return rv;
        }

        public static IUCIOption GetOption(this string opt)
        {
            switch (opt.GetOptionType())
            {
                case OptionType.Spin:
                    return opt.ProcessSpinOption();

                case OptionType.Check:
                    return opt.ProcessCheckOption();

                case OptionType.Combo:
                    return opt.ProcessComboOption();

                case OptionType.Button:
                    return opt.ProcessButtonOption();

                case OptionType.String:
                    return opt.ProcessStringOption();

                default:
                    return null;
            }
        }



        private static UCIButtonOption ProcessButtonOption(this string opt)
            => new UCIButtonOption { Name = opt.GetOptionName() };

        private static UCIStringOption ProcessStringOption(this string opt)
            => new UCIStringOption { Default = opt.GetStringDefault(), Name = opt.GetOptionName() };

        private static UCIComboOption ProcessComboOption(this string opt)
        {
            return new UCIComboOption
            {
                Name = opt.GetOptionName(),
                Default = opt.GetStringDefault(),
                Options = opt.GetComboOptionValues()
            };
        }

        private static UCISpinOption ProcessSpinOption(this string opt)
        {
            return new UCISpinOption
            {
                Name = opt.GetOptionName(),
                Min = opt.GetNumericOptionType("min"),
                Max = opt.GetNumericOptionType("max"),
                Default = opt.GetNumericDefault()
            };
        }

        private static UCICheckOption ProcessCheckOption(this string opt)
        {
            return new UCICheckOption
            {
                Name = opt.GetOptionName(),
                Default = opt.GetDefaultForCheckbox()
            };
        }





        public static string UCIMovesFromMoveObjects(this IEnumerable<Move> moveCollection)
        {
            var moves = moveCollection.ToArray();
            if (!moves.Any())
            {
                return "";
            }
            return string.Join(" ", moves.Select(m =>
            {
                var pString = m.MoveType == MoveType.Promotion ?
                    char.ToLower(PieceHelpers.GetCharFromPromotionPiece(m.PromotionPiece)).ToString() : "";
                return $"{m.SourceIndex.IndexToSquareDisplay()}{m.DestinationIndex.IndexToSquareDisplay()}{pString}";
            }));
        }


    }
}
