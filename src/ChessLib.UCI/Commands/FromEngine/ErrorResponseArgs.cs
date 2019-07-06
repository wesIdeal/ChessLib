namespace ChessLib.UCI.Commands.FromEngine
{
    public class ErrorResponseArgs : EngineResponseArgs
    {
        public string ErrorMessage { get; protected set; }
        public ErrorResponseArgs(string errorMessage, string response) : base(null, response)
        {
            ErrorMessage = errorMessage;
        }
    }
}
