using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.EngineInterface.UCI.Commands;

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
        private readonly IEngineResponseFactory _responseFactory;

        public UCIEngineMessageSubscriber(EngineResponseCallback callback)
            : this(callback, new UCIResponseFactory())
        {
        }

        public UCIEngineMessageSubscriber(EngineResponseCallback callback, IEngineResponseFactory responseFactory)
        {
            EngineResponseCallback = callback;
            _responseFactory = responseFactory;
        }

        public EngineResponseCallback EngineResponseCallback { get; set; }

        public string FEN { get; set; } = BoardConstants.FenStartingPosition;

        public void ProcessEngineResponse(string engineResponseText, CommandInfo currentCommand)
        {
            if (string.IsNullOrEmpty(engineResponseText))
            {
                return;
            }

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