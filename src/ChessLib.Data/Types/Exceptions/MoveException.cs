using System;
using System.Runtime.Serialization;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Types.Exceptions
{
    public enum MoveError
    {
        NoneSet,
        CastleThroughCheck,
        CastleKingNotOnStartingSquare,
        CastleRookNotOnStartingSquare,
        ActivePlayerHasNoPieceOnSourceSquare,
        MoveLeavesKingInCheck,
        EpNotAvailable,
        CastleBadDestinationSquare,
        CastleOccupancyBetween,
        CastleUnavailable,
        BadDestination,
        CastleKingInCheck,
        ActiveColorPieceAtDestination,
        EpSourceIsNotPawn,
        EpWrongSourceRank,
        EpNotAttackedBySource,
        Stalemate
    }
    [Serializable]
    public class MoveException : Exception
    {
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
        }

        public MoveException(string message, IMoveExt move, Color activePlayer)
            : this(message + $"\r\n{activePlayer.ToString()}'s move: {move.ToString()}")
        {
            Error = MoveError.NoneSet;
        }

        protected MoveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
