using System;

namespace ChessLib.EngineInterface.UCI.Commands.FromEngine
{
    public interface IEngineResponse
    {
        string ResponseText { get; }
        Guid Id { get; set; }
    };
}
