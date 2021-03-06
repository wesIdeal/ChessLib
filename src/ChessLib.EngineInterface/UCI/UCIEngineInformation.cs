﻿using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;
using ChessLib.EngineInterface.UCI.Commands.FromEngine.Options;

namespace ChessLib.EngineInterface.UCI
{
    public class UCIResponse : IResponseObject
    {
        public UCIResponse()
        {
            Options = new List<IUCIOption>();
        }

        public Guid Id { get; }
        public List<IUCIOption> Options { get; }
        public string Name { get; private set; }
        public string Author { get; private set; }
        public bool UCIOk { get; private set; }

        public void SetInfoFromString(string optResponse)
        {
            var optsUnfiltered = optResponse.Split('\n').Select(x => x.Trim()).Where(x => x != "").ToArray();
            foreach (var opt in optsUnfiltered)
            {
                if (opt.StartsWith("id name"))
                {
                    Name = opt.Replace("id name ", "").Trim();
                }
                else if (opt.StartsWith("id author"))
                {
                    Author = opt.Replace("id author ", "").Trim();
                }
                else if (opt.StartsWith("option"))
                {
                    Options.Add(opt.GetOption());
                }
                else if (opt == "uciok")
                {
                    UCIOk = true;
                }

            }
        }

        public UCIResponse(Guid id, string optResponse)
        {
            Id = id;
            var optsUnfiltered = optResponse.Split('\n').Select(x => x.Trim()).Where(x => x != "").ToArray();
            Name = optsUnfiltered.FirstOrDefault(x => x.StartsWith("id name"))?.Replace("id name", "").Trim();
            Author = optsUnfiltered.FirstOrDefault(x => x.StartsWith("id author"))?.Replace("id author", "").Trim();
            Options = optsUnfiltered.Where(x => x.StartsWith("option")).GetOptions();
            UCIOk = optsUnfiltered.Any(x => x.Equals("uciok"));
        }

        public bool SupportsOption(string optionName)
        {
            var options = Options.Where(x => x.Name == optionName);
            return options.Any();
        }

        public bool IsOptionValid(string name, string value, out string message)
        {
            message = "";
            var option = Options.FirstOrDefault(x => x.Name == name);
            if (option == null)
            {
                message = $"Cannot find option with the name of {name}.";
                return false;
            }
            var optionType = option.GetType();
            if (optionType == typeof(UCICheckOption))
            {
                return IsCheckOptionValid(value, out message);
            }
            else if (optionType == typeof(UCIComboOption))
            {
                return IsComboOptionValid((UCIComboOption)option, value, out message);
            }
            else if (optionType == typeof(UCISpinOption))
            {
                return IsSpinOptionValid((UCISpinOption)option, value, out message);
            }
            return true;

        }

        private static bool IsComboOptionValid(UCIComboOption opt, string value, out string message)
        {
            message = "";
            if (!opt.Options.Contains(value))
            {
                message = $"Options {value} is not in the list of valid options: {string.Join("|", opt.Options)}.";
                return false;
            }
            return true;
        }

        private static bool IsSpinOptionValid(UCISpinOption opt, string value, out string message)
        {
            message = "";
            if (!double.TryParse(value, out double dValue))
            {
                message = $"Option value {value} cannot be interpreted as a number.";
                return false;
            }
            if (opt.Min.HasValue)
            {
                if (dValue < opt.Min.Value)
                {
                    message = $"Option value {value} cannot be less than {opt.Min.Value}.";
                    return false;
                }
            }
            if (opt.Max.HasValue)
            {
                if (dValue > opt.Max.Value)
                {
                    message = $"Option value {value} cannot be more than {opt.Max.Value}.";
                    return false;
                }
            }
            return true;
        }

        private static bool IsCheckOptionValid(string value, out string message)
        {
            message = "";
            if (!(new[] { "true", "false" }.Contains(value)))
            {
                message = "Check option should only be true or false";
                return false;
            }
            return true;
        }

        public override string ToString() => $"{Name} {Id}";
    }
}
