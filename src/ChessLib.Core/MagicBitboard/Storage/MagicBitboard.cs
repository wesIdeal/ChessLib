namespace ChessLib.Core.MagicBitboard.Storage
{
    
    internal class MagicBitboard 
    {
        public ulong Key { get; }
        public ulong[] Attacks { get; }

        public MagicBitboard(ulong key, ulong[] attacks)
        {
            Key = key;
            Attacks = attacks;
        }

        public ulong GetAttacks(ulong occupancy)
        {
            var hash = Hash(occupancy);
            return Attacks[hash];
        }

        public ulong Hash(ulong occupancy)
        {
            return (occupancy * Key) >> (64 - 12);
        }
    }
}