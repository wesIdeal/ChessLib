using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data
{
    public class BoardFENInfo : IBoard
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
            InitialFEN = this.ToFEN();
        }
        public BoardFENInfo(ulong[][] piecePlacement, Color activePlayer, CastlingAvailability castlingAvailability, ushort? enPassantSquare, uint? halfmoveClock, uint fullmoveClock, bool chess960 = false)
        {
            PiecePlacement = piecePlacement;
            ActivePlayer = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassantSquare = enPassantSquare;
            HalfmoveClock = halfmoveClock;
            FullmoveCounter = fullmoveClock;
            InitialFEN = this.ToFEN();
        }

        public ulong[][] PiecePlacement { get; set; }
        public Color ActivePlayer { get; set; }
        public CastlingAvailability CastlingAvailability { get; set; }
        public ushort? EnPassantSquare { get; set; }
        public uint? HalfmoveClock { get; set; }
        public uint FullmoveCounter { get; set; }
        public bool Chess960 { get; }
        public string InitialFEN { get; }

        public ulong ActiveOccupancy => PiecePlacement.Occupancy(ActivePlayer);
        public ulong OpponentOccupancy => PiecePlacement.Occupancy(this.OpponentColor());
        public ulong TotalOccupancy => PiecePlacement.Occupancy();

        public ushort ActivePlayerKingIndex => PiecePlacement.Occupancy(ActivePlayer, Piece.King).GetSetBits()[0];
        public ulong ActivePlayerOccupancy(Piece p) => PiecePlacement.Occupancy(ActivePlayer, p);

        public object Clone()
        {
            return new BoardFENInfo(PiecePlacement, ActivePlayer, CastlingAvailability, EnPassantSquare, HalfmoveClock, FullmoveCounter, Chess960);
        }
    }
}
