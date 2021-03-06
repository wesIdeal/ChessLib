﻿using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using System;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Validators.BoardValidation;
using ChessLib.Data.Validators.MoveValidation;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ChessLib.Data.Annotations;
using ChessLib.Data.Types.Interfaces;
using System.Text;

namespace ChessLib.Data.Boards
{
    public class BoardInfo : BoardBase
    {

        public BoardInfo() : this(FENHelpers.FENInitial)
        {

        }

        public BoardInfo(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability,
            ushort? enPassantIdx, ushort halfMoveClock, ushort fullMoveCount)
            : base(occupancy, activePlayer, castlingAvailability, enPassantIdx, halfMoveClock, fullMoveCount)
        {

        }

        public BoardInfo(string fen, bool is960 = false) : base(fen, is960)
        {

        }


        /// <summary>
        /// Clones a board object from this board
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {

            var piecePlacement = new ulong[2][];
            for (int i = 0; i < 2; i++)
            {
                piecePlacement[i] = (ulong[])PiecePlacement[i].Clone();
            }
            var b = new BoardInfo(piecePlacement, ActivePlayer, CastlingAvailability, EnPassantSquare, HalfmoveClock,
                FullmoveCounter);
            return b;
        }

        public string CurrentFEN
        {
            get => this.ToFEN();
        }

        /// <summary>
        /// Applies the given board parameter to this board
        /// </summary>
        /// <param name="newBoard"></param>
        protected void ApplyNewBoard(IBoard newBoard)
        {
            PiecePlacement = newBoard.GetPiecePlacement();
            ActivePlayer = newBoard.ActivePlayer;
            CastlingAvailability = newBoard.CastlingAvailability;
            EnPassantSquare = newBoard.EnPassantSquare;
            HalfmoveClock = newBoard.HalfmoveClock;
            FullmoveCounter = newBoard.FullmoveCounter;
        }


    }
}