using System.Collections.Generic;

namespace ChessLib.Magic
{
    static class MagicHelpers
    {
        public static List<int> GetBitIndexesForPosition(this ulong position)
        {
            var indexes = new List<int>();

            while (position != 0)
            {
                var lsb = position.LSB();
                var index = lsb.GetLSBIndex();
                indexes.Add(index);
                position = position.PopLSB();
            }

            return indexes;
        }

        public static ulong GeneratePermutation(this int permIndex, List<int> bitIndexes)
        {
            var permutation = 0ul;

            while (permIndex != 0)
            {
                var lsb = permIndex.LSB();
                permIndex = permIndex.PopLSB();
                var lsbIndex = ((ulong)lsb).GetLSBIndex();
                permutation |= 1ul << bitIndexes[lsbIndex];
            }

            return permutation;
        }
    }

}
