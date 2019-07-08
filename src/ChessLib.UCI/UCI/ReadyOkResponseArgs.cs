namespace ChessLib.EngineInterface.UCI
{

    public class ReadyOkResponseArgs : EngineResponseArgs
    {
        public ReadyOkResponseArgs(string optResponse) : base(null, optResponse)
        {
        }
    }
}
