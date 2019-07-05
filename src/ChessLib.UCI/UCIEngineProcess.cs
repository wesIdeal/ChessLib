using ChessLib.Data.Helpers;
using ChessLib.UCI;
using ChessLib.UCI.Commands.FromEngine;
using System;
using System.Diagnostics;

public interface IEngineMessageSubscriber
{
    string FEN { get; set; }
    void ProcessEngineResponse(string engineResponseText);
}
public delegate void EngineResponseCallback(EngineResponseArgs engineResponse);

public class UCIEngineMessageSubscriber : IEngineMessageSubscriber
{
    private EngineResponseCallback EngineResponseCallback;
    private readonly IEngineResponseFactory _responseFactory;
    public UCIEngineMessageSubscriber(EngineResponseCallback callback, bool includeMoveCalcInformation = false)
        : this(callback, new UCIResponseFactory())
    { }

    public UCIEngineMessageSubscriber(EngineResponseCallback callback, IEngineResponseFactory responseFactory, bool includeMoveCalcInformation = false)
    {
        EngineResponseCallback = callback;
        _responseFactory = responseFactory;
    }
    private string _fen = FENHelpers.FENInitial;
    public string FEN { get => _fen; set => _fen = value; }

    public void ProcessEngineResponse(string engineResponseText)
    {
        if (string.IsNullOrEmpty(engineResponseText)) { return; }

        var response = _responseFactory.MakeResponseArgs(FEN, engineResponseText, out string error);

        if (response != null)
        {
            OnEngineObjectReceived(response);
        }
        else if (!IsResponseUCI(engineResponseText))
        {
            var message = $"**Message with no corresponding command received**\r\n\t{engineResponseText}\r\n**End Message**";
            OnDebugEventExecuted(new DebugEventArgs(message));
        }
        if (!string.IsNullOrWhiteSpace(error))
        {
            OnDebugEventExecuted(new DebugEventArgs(error));
        }
    }
}