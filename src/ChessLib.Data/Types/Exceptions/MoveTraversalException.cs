using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Data.Types.Exceptions
{
    public class MoveTraversalException : Exception
    {
       

        public static MoveTraversalException NextMoveNotFoundException(MoveExt move)
        {
            if (string.IsNullOrWhiteSpace(move.SAN))
            {
                return new MoveTraversalException($"Move {move.SAN} not found. ");
            }
            return NextMoveNotFoundException(move);
        }

        protected MoveTraversalException(string message) : base(message)
        {
        }
    }
}
