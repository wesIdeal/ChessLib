using System;
using ChessLib.Types.Interfaces;

namespace ChessLib.Types.Exceptions
{
    public class MoveTraversalException : Exception
    {
       

        public static MoveTraversalException NextMoveNotFoundException(IMoveExt move)
        {
            if (string.IsNullOrWhiteSpace(move.ToString()))
            {
                return new MoveTraversalException($"Move {move} not found. ");
            }
            return NextMoveNotFoundException(move);
        }

        protected MoveTraversalException(string message) : base(message)
        {
        }
    }
}
