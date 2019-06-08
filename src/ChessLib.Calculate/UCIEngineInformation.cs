using System.Linq;

namespace ChessLib.UCI
{
    public class UCIEngineInformation
    {
        public IUCIOption[] Options { get; private set; }
        public string Name { get; private set; }
        public string Author { get; private set; }
        public bool UCIOk { get; private set; }
        public UCIEngineInformation(string optResponse)
        {
            var optsUnfiltered = optResponse.Split('\n').Select(x => x.Trim()).Where(x => x != "").ToArray();
            Name = optsUnfiltered.FirstOrDefault(x => x.StartsWith("id name"))?.Replace("id name", "").Trim();
            Author = optsUnfiltered.FirstOrDefault(x => x.StartsWith("id author"))?.Replace("id author", "").Trim();
            Options = optsUnfiltered.Where(x => x.StartsWith("option")).GetOptions();
            UCIOk = optsUnfiltered.Last().Equals("uciok");
        }

        public UCIEngineInformation()
        {
        }
    }
}
