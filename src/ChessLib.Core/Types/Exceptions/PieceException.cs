using System;

namespace ChessLib.Core.Types.Exceptions
{
    public class PieceException : Exception
    {
        public PieceException(string message) : base(message)
        {
        }
    }
}