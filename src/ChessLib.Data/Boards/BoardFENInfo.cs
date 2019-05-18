using ChessLib.Data.Helpers;
using ChessLib.Data.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Data
{
    public class BoardFENInfo : ICloneable
    {
        public BoardFENInfo() : this(FENHelpers.FENInitial, false) { }
        public BoardFENInfo(string fen, bool chess960 = false)
        {
            PiecePlacement = FENHelpers.BoardFromFen(fen, out Color active, out CastlingAvailability ca, out ushort? enPassant, out uint hmClock, out uint fmClock);
            ActivePlayer = active;
            CastlingAvailability = ca;
            EnPassantSquare = enPassant;
            HalfmoveClock = hmClock;
            FullmoveCounter = fmClock;
            Chess960 = chess960;
            InitialFEN = ToFEN();
        }
        public BoardFENInfo(ulong[][] piecePlacement, Color activePlayer, CastlingAvailability castlingAvailability, ushort? enPassantSquare, uint? halfmoveClock, uint fullmoveClock, bool chess960 = false)
        {
            PiecePlacement = piecePlacement;
            ActivePlayer = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassantSquare = enPassantSquare;
            HalfmoveClock = halfmoveClock;
            FullmoveCounter = fullmoveClock;
            InitialFEN = ToFEN();
        }

        public ulong[][] PiecePlacement { get; set; }
        public Color ActivePlayer { get; set; }
        public CastlingAvailability CastlingAvailability { get; set; }
        public ushort? EnPassantSquare { get; set; }
        public uint? HalfmoveClock { get; set; }
        public uint FullmoveCounter { get; set; }
        public bool Chess960 { get; }

        public readonly string InitialFEN;

        public Color OpponentColor => ActivePlayer.Toggle();
        public ulong ActiveTotalOccupancy => PiecePlacement.Occupancy(ActivePlayer);
        public ulong OpponentOccupancy => PiecePlacement.Occupancy(OpponentColor);
        public ulong TotalOccupancy => PiecePlacement.Occupancy();

        public ushort ActivePlayerKingIndex => PiecePlacement.Occupancy(ActivePlayer, Piece.King).GetSetBits()[0];
        public ulong ActivePlayerOccupancy(Piece p) => PiecePlacement.Occupancy(ActivePlayer, p);

        #region FEN String Retrieval

        public string GetPiecePlacement()
        {
            return PiecePlacement.GetPiecePlacement();
        }

        public string GetSideToMoveStrRepresentation()
        {
            return ActivePlayer == Color.Black ? "b" : "w";
        }

        public string GetCastlingAvailabilityString()
        {
            return FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability);
        }

        public string GetEnPassantString()
        {
            return EnPassantSquare == null ? "-" : EnPassantSquare.Value.IndexToSquareDisplay();
        }

        public string GetHalfMoveClockString()
        {
            return HalfmoveClock.ToString();
        }

        public string GetMoveCounterString()
        {
            return FullmoveCounter.ToString();
        }

        public string ToFEN()
        {
            return
                $"{GetPiecePlacement()} {GetSideToMoveStrRepresentation()} {GetCastlingAvailabilityString()} {GetEnPassantString()} {GetHalfMoveClockString()} {GetMoveCounterString()}";
        }

        public object Clone()
        {
            return new BoardFENInfo(PiecePlacement, ActivePlayer, CastlingAvailability, EnPassantSquare, HalfmoveClock, FullmoveCounter, Chess960);
        }


        #endregion
    }
}
