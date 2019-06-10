namespace ChessLib.UCI.Commands.FromEngine.Options
{
    public class UCISpinOption : IUCIOption
    {
        public string Name { get; set; }
        public double? Min { get; internal set; }
        public double? Default { get; internal set; }
        public double? Max { get; internal set; }
    }
}
