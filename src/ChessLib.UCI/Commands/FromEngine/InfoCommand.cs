using ChessLib.Data.Helpers;
using System;

namespace ChessLib.UCI.Commands.FromEngine
{

    public abstract class EngineResponseArgs : EventArgs, IEngineResponse
    {
        public EngineResponseArgs(string response)
        {
            EngineResponse = response;
        }
        public string EngineResponse { get; set; }
        public Guid Id { get; set; }
    }
}
