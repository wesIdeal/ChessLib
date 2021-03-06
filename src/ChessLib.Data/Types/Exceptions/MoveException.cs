﻿using System;
using System.IO;
using System.Runtime.Serialization;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

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

        public MoveException(string message, MoveError type, IMoveExt move, Color activePlayer, IBoard board = null)
           : this(message + $"\r\n{activePlayer.ToString()}'s move: {move.ToString()}", board)
        {
            Error = type;
        }

        public MoveException(string message, IMoveExt move, Color activePlayer, IBoard board = null)
            : this(message + $"\r\n{activePlayer.ToString()}'s move: {move.ToString()}", board)
        {
            Error = MoveError.NoneSet;
        }


        protected MoveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
