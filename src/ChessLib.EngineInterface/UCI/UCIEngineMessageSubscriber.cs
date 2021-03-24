using ChessLib.Core.Types.Helpers;
using ChessLib.Data.Helpers;
using ChessLib.EngineInterface.UCI.Commands;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;

namespace ChessLib.EngineInterface.UCI
{
    public interface IEngineMessageSubscriber
    {
        string FEN { get; set; }
        void ProcessEngineResponse(string engineResponseText, CommandInfo awaitedCommand);
    }
    public delegate void EngineResponseCallback(EngineResponseArgs engineResponse);

    public class UCIEngineMessageSubscriber : IEngineMessageSubscriber
    {
        public EngineResponseCallback EngineResponseCallback { get; set; }
        private readonly IEngineResponseFactory _responseFactory;
        public UCIEngineMessageSubscriber(EngineResponseCallback callback)
            : this(callback, new UCIResponseFactory())
        { }

        public UCIEngineMessageSubscriber(EngineResponseCallback callback, IEngineResponseFactory responseFactory)
        {
            EngineResponseCallback = callback;
            _responseFactory = responseFactory;
        }

        public string FEN { get; set; } = FENHelpers.FENInitial;

        public void ProcessEngineResponse(string engineResponseText, CommandInfo currentCommand)
        {
            if (string.IsNullOrEmpty(engineResponseText)) { return; }

            if (currentCommand is AwaitableCommandInfo awaitedCommand)
            {
                HandleAwaitedResponse(engineResponseText, awaitedCommand);
            }
            var response = _responseFactory.MakeResponseArgs(FEN, engineResponseText);
            if (response != null)
            {
                EngineResponseCallback(response);
            }
        }

        private void HandleAwaitedResponse(string response, AwaitableCommandInfo awaitedCommand)
        {
            if (awaitedCommand != null)
            {
                if (awaitedCommand.ExpectedResponse == response)
                {
                    awaitedCommand.ResetEvent.Set();
                }
            }
        }
    }
}