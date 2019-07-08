using System;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;

namespace ChessLib.EngineInterface.UCI
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

    public class EngineCalculationResponseArgs : EngineResponseArgs
    {
        public EngineCalculationResponseArgs(ICalculationInfoResponse responseObject, string response) : base(responseObject, response)
        {
            ResponseObject = responseObject;
        }
        public new ICalculationInfoResponse ResponseObject;
    }

}