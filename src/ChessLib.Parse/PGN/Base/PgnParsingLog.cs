namespace ChessLib.Parse.PGN.Base
{
    public enum ErrorLevel
    {
        Info,
        Warning,
        Error
    }

    public class PgnParsingLog
    {
        public ErrorLevel ErrorLevel { get; set; }
        public string Message { get; set; }
    }
}