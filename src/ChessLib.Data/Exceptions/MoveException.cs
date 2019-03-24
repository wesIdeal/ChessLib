using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using System;

using System.Runtime.Serialization;


namespace ChessLib.Data.Exceptions
{

    [Serializable]
    public class MoveException : Exception
    {
        [NonSerialized]
        private readonly MoveExt move;
        private readonly Color activePlayer;

        public MoveException()
        {
        }

        public MoveException(string message) : base(message)
        {
        }

        public MoveException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MoveException(string message, MoveExt move, Color activePlayer)
            : this(message + $"\r\n{activePlayer.ToString()}'s move from {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}")
        {
            this.move = move;
            this.activePlayer = activePlayer;
        }

        protected MoveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
