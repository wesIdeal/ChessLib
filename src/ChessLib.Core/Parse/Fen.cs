using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Parse
{
    internal sealed class Fen
    {
        public ulong[][] PiecePlacement;
        public Color ActiveColor;
        public CastlingAvailability CastlingAvailability;
        public ushort? EnPassantIndex;
        public byte HalfmoveClock;
        public uint FullmoveClock;
        public uint TotalPlies => (uint) ((2 * FullmoveClock) - (ActiveColor == Color.White ? 2 : 1));

        public Board AsBoard => new Board(PiecePlacement, HalfmoveClock, EnPassantIndex, null, CastlingAvailability,
            ActiveColor, FullmoveClock, true, null);
    }
}