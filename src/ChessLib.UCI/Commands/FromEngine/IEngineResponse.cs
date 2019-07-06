using System;

namespace ChessLib.UCI.Commands.FromEngine
{
    public interface IEngineResponse
    {
        string ResponseText { get; }
        Guid Id { get; set; }
    };
}
