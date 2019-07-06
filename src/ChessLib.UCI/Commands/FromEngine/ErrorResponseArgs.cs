namespace ChessLib.EngineInterface.Commands.FromEngine
{
    public class ErrorResponse : IResponseObject
    {
        public string ErrorMessage { get; protected set; }
        public ErrorResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
