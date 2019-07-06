using System;
using System.Diagnostics;
using ChessLib.Data.Helpers;
using ChessLib.EngineInterface.Commands.FromEngine;

namespace ChessLib.EngineInterface
{
    public interface IEngineMessageSubscriber
    {
        string FEN { get; set; }
        void ProcessEngineResponse(string engineResponseText);
    }
    public delegate void EngineResponseCallback(EngineResponseArgs engineResponse);

    public class UCIEngineMessageSubscriber : IEngineMessageSubscriber
    {
        public EngineResponseCallback EngineResponseCallback { get; set; }
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

            var response = _responseFactory.MakeResponseArgs(FEN, engineResponseText);

            if (response != null)
            {
                EngineResponseCallback(response);
            }
        }
    }
}