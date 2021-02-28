namespace ChessLib.MagicBitboard
{
    class MagicBitboard
    {
        public MagicBitboard(ulong key, ulong[] attacks)
        {
            Key = key;
            Attacks = attacks;
        }

        public ulong Key { get; private set; }
        public ulong[] Attacks { get; private set; }

        public ulong Hash(ulong occupancy)
        {
            return (occupancy * Key) >> 52;
        }

        public ulong GetAttacks(ulong occupancy)
        {
            var hash = Hash(occupancy);
            return Attacks[hash];
        }
    }
}