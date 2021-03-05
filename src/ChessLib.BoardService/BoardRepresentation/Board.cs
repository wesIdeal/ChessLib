using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
namespace ChessLib.BoardService.BoardRepresentation
{
    public class Board
    {
        private ulong[][] _occupancy;

        public Board()
        {
            InitializeOccupancy();
        }

        private void InitializeOccupancy()
        {
            _occupancy = new ulong[2][];
            _occupancy[(int)Color.Black] = new ulong[6];
            _occupancy[(int)Color.White] = new ulong[6];
            InitializeBlackOccupancy();
            InitializeWhiteOccupancy();
        }

        private void InitializeBlackOccupancy()
        {
            BlackOccupancy[(int)Piece.Pawn] = BoardConstants.Rank7;
            BlackOccupancy[(int)Piece.Knight] = 0x4200000000000000;
            BlackOccupancy[(int)Piece.Bishop] = 0x2400000000000000;
            BlackOccupancy[(int)Piece.Rook] = 0x8100000000000000;
            BlackOccupancy[(int)Piece.Queen] = 0x800000000000000;
            BlackOccupancy[(int)Piece.King] = 0x1000000000000000;
        }

        private void InitializeWhiteOccupancy()
        {
            WhiteOccupancy[(int)Piece.Pawn] = BoardConstants.Rank2;
            WhiteOccupancy[(int)Piece.Knight] = 0x42;
            WhiteOccupancy[(int)Piece.Bishop] = 0x24;
            WhiteOccupancy[(int)Piece.Rook] = 0x81;
            WhiteOccupancy[(int)Piece.Queen] = 0x8;
            WhiteOccupancy[(int)Piece.King] = 0x10;
        }

        public ulong[][] Occupancy
        {
            get => _occupancy;
            set => _occupancy = value;
        }

        public ulong[] BlackOccupancy => Occupancy[(int)Color.Black];
        public ulong[] WhiteOccupancy => Occupancy[(int)Color.White];

        /// <summary>
        /// Represents the player who is about to move.
        /// </summary>
        public Color ActivePlayer { get; set; }
        /// <summary>
        /// Enumeration for all castling-moves available on the board.
        /// </summary>
        public CastlingAvailability CastlingAvailability { get; set; }

        /// <summary>
        /// Index of en passant square, if available. Null if no en passant capture exists.
        /// </summary>
        public ushort? EnPassant { get; set; }

        /// <summary>
        /// The number of the full move. Starts at 1. After each move from black, it is incremented.
        /// </summary>
        public ushort FullMoveCount { get; set; }

        /// <summary>
        /// Number of halfmoves since either a) a pawn advance or b) capture. Used to determine draws for 50-move rule.
        /// </summary>
        public ushort HalfMoveClock { get; set; }

    }
}
