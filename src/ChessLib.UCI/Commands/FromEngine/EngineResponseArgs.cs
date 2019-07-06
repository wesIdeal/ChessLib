using System;

namespace ChessLib.UCI.Commands.FromEngine
{
    public class EngineResponseArgs : EventArgs, IEngineResponse
    {
        public EngineResponseArgs(IResponseObject responseObject, string response)
        {
            ResponseObject = responseObject;
            ResponseText = response;
        }

        public IResponseObject ResponseObject;
        public string ResponseText { get; set; }
        public Guid Id { get; set; }
    }
}