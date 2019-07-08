using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Boards
{
    public abstract class BoardBase : IBoard
    {
        protected ulong[][] PiecePlacement;

        protected BoardBase() { }

        protected BoardBase(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability, ushort? enPassantIdx, uint? halfMoveClock, uint fullMoveCount)
        {
            PiecePlacement = occupancy;
            ActivePlayer = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassantSquare = enPassantIdx;
            HalfmoveClock = halfMoveClock;
            FullmoveCounter = fullMoveCount;
        }

        public ulong[][] GetPiecePlacement()
        {
            return PiecePlacement;
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
