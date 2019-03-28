using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using System;

using System.Runtime.Serialization;


namespace ChessLib.Data.Exceptions
{
    public enum MoveExceptionType
    {
        NoneSet,
        Castle_ThroughCheck,
        Castle_KingNotOnStartingSquare,
        Castle_RookNotOnStartingSquare,
        ActivePlayerHasNoPieceOnSourceSquare,
        MoveLeavesKingInCheck
    }
    [Serializable]
    public class MoveException : Exception
    {
        [NonSerialized]
        private readonly MoveExt move;
        private readonly Color activePlayer;
        public MoveExceptionType ExceptionType { get; set; }
        public MoveException()
        {
            ExceptionType = MoveExceptionType.NoneSet;
        }

        public MoveException(string message) : base(message)
        {
            ExceptionType = MoveExceptionType.NoneSet;
        }

        public MoveException(string message, Exception innerException) : base(message, innerException)
        {
            ExceptionType = MoveExceptionType.NoneSet;
        }

        public MoveException(string message, MoveExceptionType type, MoveExt move, Color activePlayer)
           : this(message + $"\r\n{activePlayer.ToString()}'s move from {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}")
        {
            ExceptionType = type;
            this.move = move;
            this.activePlayer = activePlayer;
        }

        public MoveException(string message, MoveExt move, Color activePlayer)
            : this(message + $"\r\n{activePlayer.ToString()}'s move from {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}")
        {
            ExceptionType = MoveExceptionType.NoneSet;
            this.move = move;
            this.activePlayer = activePlayer;
        }

        protected MoveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
