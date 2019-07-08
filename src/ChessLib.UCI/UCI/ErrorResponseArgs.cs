using ChessLib.EngineInterface.UCI.Commands.FromEngine;

namespace ChessLib.EngineInterface.UCI
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
