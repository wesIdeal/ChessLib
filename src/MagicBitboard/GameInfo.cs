using System;
using System.Collections;
using MagicBitboard.Helpers;

namespace MagicBitboard
{

    public class GameInfo
    {
        
        private readonly Bitboard BitBoard;
        public readonly BoardInfo BoardInfo;

        public GameInfo() : this(new Bitboard())
        {

        }

        public GameInfo(Bitboard bitboard) : this(bitboard, FENHelpers.InitialFEN)
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
