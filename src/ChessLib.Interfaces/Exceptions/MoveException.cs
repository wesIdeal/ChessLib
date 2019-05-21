using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
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
        MoveLeavesKingInCheck,
        Ep_NotAvailalbe,
        Castle_BadDestinationSquare,
        Castle_OccupancyBetween,
        Castle_Unavailable,
        BadDestination,
        Castle_KingInCheck,
        ActiveColorPieceAtDestination,
        EP_SourceIsNotPawn,
        EP_WrongSourceRank,
        EP_NotAttackedBySource,
        Stalemate
    }
    [Serializable]
    public class MoveException : Exception
    {
        [NonSerialized]
        private readonly IMoveExt move;
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

        public MoveException(string message, MoveExceptionType type, IMoveExt move, Color activePlayer)
           : this(message + $"\r\n{activePlayer.ToString()}'s move: {move.ToString()}")
        {
            ExceptionType = type;
            this.move = move;
            this.activePlayer = activePlayer;
        }

        public MoveException(string message, IMoveExt move, Color activePlayer)
            : this(message + $"\r\n{activePlayer.ToString()}'s move: {move.ToString()}")
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
