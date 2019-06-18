using ChessLib.Data.Helpers;
using System;

namespace ChessLib.UCI.Commands.FromEngine
{

    public abstract class EngineResponseArgs : EventArgs, IEngineResponse
    {
        public string EngineResponse { get; set; }
    }
}
