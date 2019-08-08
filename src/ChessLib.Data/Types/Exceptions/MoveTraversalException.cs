using ChessLib.Data.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Data.Types.Exceptions
{
    public class MoveTraversalException : Exception
    {
        public static MoveTraversalException NextMoveNotFoundException(IMove move)
        {
            return new MoveTraversalException("Move {move} not found.");
        }

        public static MoveTraversalException NextMoveNotFoundException(IMoveStorage move)
        {
            if (string.IsNullOrWhiteSpace(move.SAN))
            {
                return new MoveTraversalException($"Move {move.SAN} not found. ");
            }
            return NextMoveNotFoundException(move.MoveData);
        }

        protected MoveTraversalException(string message) : base(message)
        {
        }
    }
}
