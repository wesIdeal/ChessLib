using System;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Exceptions
{
    public class MoveTraversalException : Exception
    {
       

        public static MoveTraversalException NextMoveNotFoundException(IMove move)
        {
            if (string.IsNullOrWhiteSpace(move.ToString()))
            {
                return new MoveTraversalException($"MoveValue {move} not found. ");
            }
            return NextMoveNotFoundException(move);
        }

        protected MoveTraversalException(string message) : base(message)
        {
        }
    }
}
