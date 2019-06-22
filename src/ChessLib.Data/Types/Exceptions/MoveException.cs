using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using System;

using System.Runtime.Serialization;


namespace ChessLib.Data.Exceptions
{
    public enum MoveError
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
        public MoveError Error { get; set; }
        public MoveException()
        {
            Error = MoveError.NoneSet;
        }

        public MoveException(string message) : base(message)
        {
            Error = MoveError.NoneSet;
        }

        public MoveException(string message, Exception innerException) : base(message, innerException)
        {
            Error = MoveError.NoneSet;
        }

        public MoveException(string message, MoveError type, IMoveExt move, Color activePlayer)
           : this(message + $"\r\n{activePlayer.ToString()}'s move: {move.ToString()}")
        {
            Error = type;
            this.move = move;
            this.activePlayer = activePlayer;
        }

        public MoveException(string message, IMoveExt move, Color activePlayer)
            : this(message + $"\r\n{activePlayer.ToString()}'s move: {move.ToString()}")
        {
            Error = MoveError.NoneSet;
            this.move = move;
            this.activePlayer = activePlayer;
        }

        protected MoveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
