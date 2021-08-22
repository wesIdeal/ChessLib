namespace ChessLib.Core.Types.PgnExport
{
    public abstract class PgnSerializer
    {
        public PGNFormatterOptions Options { get; }

        protected PgnSerializer(PGNFormatterOptions options)
        {
            Options = options;
        }
    }
}