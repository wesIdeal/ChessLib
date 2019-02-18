using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib
{
    internal class RandomInt64
    {
        private readonly Random _random;
        public RandomInt64()
        {
            _random = new Random();
        }
        public ulong GetNextRandom()
        {
            byte[] b = new byte[8];
            _random.NextBytes(b);
            return BitConverter.ToUInt64(b, 0);
        }
    }
}
