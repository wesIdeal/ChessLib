namespace ChessLib.Core.Types.EventArgs
{
    public class MoveMadeEventArgs : System.EventArgs
    {
        public string CurrentFen { get; }
        public Move[] PreviousMoves { get; }

        public MoveMadeEventArgs(Move[] previousMoves, string fen)
        {
            CurrentFen = fen;
            PreviousMoves = previousMoves;
        }
    }
}