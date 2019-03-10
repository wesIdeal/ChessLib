using MagicBitboard.Enums;
using MagicBitboard.Helpers;

namespace MagicBitboard
{
    public class BoardInfo
    {
        private readonly string _fen;

        public BoardInfo(string fen)
        {
            _fen = fen;
            PiecesOnBoard[0] = new ulong[6];
            PiecesOnBoard[1] = new ulong[6];
        }

        public ulong[][] PiecesOnBoard = new ulong[2][];
        public CastlingAvailability CastlingAvailability { get; set; }
        public string FEN { get => _fen; }
        public uint HalfmoveClock { get; set; }
        public uint MoveCounter { get; set; }
        public Color ActivePlayer { get; set; }
        public ushort? EnPassentIndex { get; set; }
    }

    public class GameInfo : BoardInfo
    {
        public const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        private readonly Bitboard BitBoard;


        public GameInfo() : this(new Bitboard())
        {

        }

        public GameInfo(Bitboard bitboard) : this(bitboard, InitialFEN)
        {

        }

        public GameInfo(Bitboard bitboard, string fen) : base(fen)
        {
            Bitboard = bitboard;
        }

        public Bitboard Bitboard { get; }
    }
}
