using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Translate
{
    internal sealed class Fen
    {
        public ulong[][] PiecePlacement;
        public Color ActiveColor;
        public CastlingAvailability CastlingAvailability;
        public ushort? EnPassantIndex;
        public byte HalfmoveClock;
        public uint FullmoveClock;

        public Board AsBoard => new Board(PiecePlacement, HalfmoveClock, EnPassantIndex, null, CastlingAvailability,
            ActiveColor, FullmoveClock, false);
    }
}