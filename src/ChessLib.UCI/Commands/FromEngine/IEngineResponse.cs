using System;

namespace ChessLib.EngineInterface.Commands.FromEngine
{
    public interface IEngineResponse
    {
        string ResponseText { get; }
        Guid Id { get; set; }
    };
}
