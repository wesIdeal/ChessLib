using ChessLib.Data.Boards;
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
            PiecePlacement piecePlacement = null;
            FENHelpers.BoardFromFen(fen, out piecePlacement, out Color active,
                out CastlingAvailability ca, out ushort? enPassant, out uint hmClock, out uint fmClock);
            PiecePlacement = piecePlacement;
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
            PiecePlacement = new PiecePlacement(piecePlacement);
            ActivePlayer = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassantSquare = enPassantSquare;
            HalfmoveClock = halfmoveClock;
            FullmoveCounter = fullmoveClock;
            InitialFEN = this.ToFEN();
        }

        public PiecePlacement PiecePlacement { get; set; }
        public Color ActivePlayer { get; set; }
        public CastlingAvailability CastlingAvailability { get; set; }
        public ushort? EnPassantSquare { get; set; }
        public uint? HalfmoveClock { get; set; }
        public uint FullmoveCounter { get; set; }
        public bool Chess960 { get; }
        public string InitialFEN { get; }

        public ulong ActiveOccupancy => PiecePlacement.ColorOccupancy(this.ActivePlayer);
        public ulong OpponentOccupancy => PiecePlacement.ColorOccupancy(this.OpponentColor());
        public ulong TotalOccupancy => PiecePlacement.BlackOccupancy | PiecePlacement.WhiteOccupancy;

        public ushort ActivePlayerKingIndex => PiecePlacement.PieceOfColorOccupancy(ActivePlayer, Piece.King).GetSetBits()[0];

        IPiecePlacement IBoard.PiecePlacement { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public ulong ActivePlayerOccupancy(Piece p) => PiecePlacement.ColorOccupancy(this.ActivePlayer);

        public object Clone()
        {
            return new BoardFENInfo(PiecePlacement.GetPiecePlacementArray(), ActivePlayer, CastlingAvailability, EnPassantSquare, HalfmoveClock, FullmoveCounter, Chess960);
        }
    }
}
