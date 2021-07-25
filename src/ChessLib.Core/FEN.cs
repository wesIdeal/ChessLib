using ChessLib.Core.Types.Enums;

namespace ChessLib.Core
{
    public class FEN
    {
        public string PiecePlacement;
        public Color ActiveColor;
        public CastlingAvailability CastlingAvailability;
        public ushort? EnPassantIndex;
        public int HalfmoveClock;
        public uint FullmoveClock;
        public uint TotalPlies => (uint) ((2 * FullmoveClock) - (ActiveColor == Color.White ? 2 : 1));

    
    }
}