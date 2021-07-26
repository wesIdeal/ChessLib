namespace ChessLib.Core.Types.EventArgs
{
    public class MoveMadeEventArgs : System.EventArgs
    {
        public MoveMadeEventArgs(Move[] previousMoves, string fen)
        {
            CurrentFen = fen;
            PreviousMoves = previousMoves;
        }

        public string CurrentFen { get; }
        public Move[] PreviousMoves { get; }
    }

}
