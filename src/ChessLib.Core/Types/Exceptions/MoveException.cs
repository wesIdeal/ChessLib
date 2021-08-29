using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Exceptions
{
    public enum MoveError
    {
        NoneSet,
        CastleThroughCheck,
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
        [Description("Move must be marked as type EnPassant in order to make move.")]
        EnPassantNotMarked
    }

    [Serializable]
    public class MoveException : Exception
    {
        public IBoard Board { get; set; }
        public MoveError Error { get; set; }

        public MoveException()
        {
            Error = MoveError.NoneSet;
        }

        public MoveException(string message) : base(message)
        {
            Error = MoveError.NoneSet;
            Board = null;
        }

        public MoveException(string message, IBoard board = null) : base(message)
        {
            Error = MoveError.NoneSet;
            Board = board;
        }

        public MoveException(string message, Exception innerException) : base(message, innerException)
        {
            Error = MoveError.NoneSet;
        }

        public MoveException(string message, MoveError type, IMove move, Color activePlayer, IBoard board = null)
            : this(message + $"\r\n{activePlayer.ToString()}'s move: {move}", board)
        {
            Error = type;
        }

        public MoveException(string message, IMove move, Color activePlayer, IBoard board = null)
            : this(message + $"\r\n{activePlayer.ToString()}'s move: {move}", board)
        {
            Error = MoveError.NoneSet;
        }


        protected MoveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}