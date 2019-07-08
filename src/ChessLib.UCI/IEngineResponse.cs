using System;

namespace ChessLib.EngineInterface
{
    public interface IEngineResponse
    {
        string ResponseText { get; }
        Guid Id { get; set; }
    };
}
