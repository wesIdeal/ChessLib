namespace ChessLib.Core.MagicBitboard.Storage
{
    internal interface IMagicBitboard
    {
        ulong GetAttacks(ulong occupancy);
    }

    internal class MagicBitboard : IMagicBitboard
    {
        public MagicBitboard(ulong key, ulong[] attacks)
        {
            Key = key;
            Attacks = attacks;
        }

        public ulong Key { get; }
        public ulong[] Attacks { get; }

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