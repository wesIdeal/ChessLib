using System;
using System.Collections;
using MagicBitboard.Helpers;

namespace MagicBitboard
{

    public class GameInfo
    {
        public const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private readonly Bitboard BitBoard;
        public readonly BoardInfo BoardInfo;

        public GameInfo() : this(new Bitboard())
        {

        }

        public GameInfo(Bitboard bitboard) : this(bitboard, InitialFEN)
        {

        }

        public GameInfo(Bitboard bitboard, string fen)
        {
            _fen = fen;
            Bitboard = bitboard;
            BoardInfo = FENHelpers.BoardInfoFromFen(fen, bitboard);
        }

        private string _fen;

        public Bitboard Bitboard { get; }
    }
}
