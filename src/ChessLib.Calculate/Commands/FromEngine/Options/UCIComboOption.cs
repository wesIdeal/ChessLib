namespace ChessLib.UCI.Commands.FromEngine.Options
{
    public class UCIComboOption : IUCIOption
    {
        public string Name { get; set; }
        public string Default { get; set; }
        public string[] Options { get; set; }

    }
}
