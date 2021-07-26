using ChessLib.Core.Helpers;

namespace ChessLib.Parse.PGN
{
    internal class PolyglotTriplet
    {
        public PolyglotTriplet(ulong boardHash, ushort move, ushort weight, uint learn)
        {
            BoardHash = boardHash;
            Move = new PolyglotMove(move);
            Weight = weight;
            Learn = learn;
        }

        public ulong BoardHash { get; } // 64 bits (8 bytes)
        public PolyglotMove Move { get; } //16 bits (2 bytes)
        public ushort Weight { get; } //16 bits (2 bytes)
        public uint Learn { get; } //32 bits (4 bytes)
    }
}