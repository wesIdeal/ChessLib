namespace ChessLib.UCI.Commands.FromEngine
{

    public class ReadyOkResponseArgs : EngineResponseArgs
    {
        public ReadyOkResponseArgs(string optResponse) : base(null, optResponse)
        {
        }
    }
}
