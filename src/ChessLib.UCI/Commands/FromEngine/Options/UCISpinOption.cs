namespace ChessLib.UCI.Commands.FromEngine.Options
{
    public class UCISpinOption : UCIOption<double>
    {
        public double? Min { get; internal set; }
        public double? Default { get; internal set; }
        public double? Max { get; internal set; }
    }
}
