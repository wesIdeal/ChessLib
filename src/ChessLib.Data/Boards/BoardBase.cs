using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Data.Boards
{
    public abstract class BoardBase : IBoard
    {
        protected ulong[][] _piecePlacement;


        protected BoardBase(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability, ushort? enPassantIdx, uint? halfMoveClock, uint fullMoveCount)
        {
            _piecePlacement = occupancy;
            ActivePlayer = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassantSquare = enPassantIdx;
            HalfmoveClock = halfMoveClock;
            FullmoveCounter = fullMoveCount;
        }

        protected BoardBase()
        {
        }

        public ulong[][] GetPiecePlacement()
        {
            return _piecePlacement;
        }

        public Color ActivePlayer { get; set; }
        public CastlingAvailability CastlingAvailability { get; set; }
        public ushort? EnPassantSquare { get; set; }
        public uint? HalfmoveClock { get; set; }
        public uint FullmoveCounter { get; set; }
        public bool Chess960 { get; protected set; }
        public string InitialFEN { get; protected set; }

        public ulong ActiveOccupancy => GetPiecePlacement().Occupancy(ActivePlayer);
        public ulong OpponentOccupancy => GetPiecePlacement().Occupancy(this.OpponentColor());
        public ulong TotalOccupancy => GetPiecePlacement().Occupancy();

        public ushort ActivePlayerKingIndex => GetPiecePlacement().Occupancy(ActivePlayer, Piece.King).GetSetBits()[0];
        public ulong ActivePlayerOccupancy(Piece p) => GetPiecePlacement().Occupancy(ActivePlayer, p);

        public abstract object Clone();


    }
}
