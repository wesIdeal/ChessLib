using ChessLib.UCI.Commands.FromEngine;
using ChessLib.UCI.Commands.FromEngine.Options;
using System;
using System.Linq;

namespace ChessLib.UCI
{
    public class UCIEngineInformation : IEngineResponse
    {
        public Guid Id { get; private set; }
        public IUCIOption[] Options { get; private set; }
        public string Name { get; private set; }
        public string Author { get; private set; }
        public bool UCIOk { get; private set; }
        public UCIEngineInformation(Guid id, string optResponse)
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
                

        public UCIEngineInformation()
        {
        }
    }
}
