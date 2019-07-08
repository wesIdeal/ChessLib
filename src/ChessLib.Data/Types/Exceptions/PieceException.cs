using System;

namespace ChessLib.Data.Types.Exceptions
{
    public class PieceException : Exception
    {
        public PieceException(string message) : base(message)
        {
        }
    }
}
