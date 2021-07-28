namespace ChessLib.Core.Types
{
    /// <summary>
    /// Error severity level for a <see cref="PgnParsingLog"/>
    /// </summary>
    public enum ParsingErrorLevel
    {
        Info,
        Warning,
        Error
    }

    public class PgnParsingLog
    {
        public PgnParsingLog(ParsingErrorLevel parsingErrorLevel, string message, string parseInput = "")
        {
            ParsingErrorLevel = parsingErrorLevel;
            Message = message;
            ParseInput = parseInput;
        }

        public ParsingErrorLevel ParsingErrorLevel { get; protected set; }
        public string Message { get; protected set; }
        public string ParseInput { get; protected set; }
    }
}