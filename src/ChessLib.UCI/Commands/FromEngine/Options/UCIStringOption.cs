namespace ChessLib.UCI.Commands.FromEngine.Options
{
    public class UCIStringOption : IUCIOption
    {
        public string Name { get; set; }
        public string Default { get; set; }
        public string Value { get; set; }
    }
}
