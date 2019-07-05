using System;

namespace ChessLib.UCI.Commands.FromEngine
{
    public interface IEngineResponse
    {
        string EngineResponse { get; }
        Guid Id { get; set; }
    };
}
