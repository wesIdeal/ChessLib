using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Data.Types.Exceptions
{
    public class PieceException : Exception
    {
        public PieceException(string message) : base(message)
        {
        }
    }
}
