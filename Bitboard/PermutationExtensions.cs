using System.Collections.Generic;
using System.Linq;
namespace MagicBitboard
{
    public static class PermutationHelpers
    {
        public static IEnumerable<ulong> GetAllPermutations(this ulong mask)
        {
            var setBitIndices = BitHelpers.GetSetBits(mask);
            return GetAllPermutations(setBitIndices, 0, 0).Distinct();
        }

        private static IEnumerable<ulong> GetAllPermutations(List<int> SetBits, int Index, ulong Value)
        {
            BitHelpers.SetBit(ref Value, SetBits[Index]);
            yield return Value;
            int index = Index + 1;
            if (index < SetBits.Count)
            {
                using (IEnumerator<ulong> occupancyPermutations = GetAllPermutations(SetBits, index, Value).GetEnumerator())
                {
                    while (occupancyPermutations.MoveNext())
                    {
                        yield return occupancyPermutations.Current;
                    }
                }
            }
            BitHelpers.ClearBit(ref Value, SetBits[Index]);
            yield return Value;
            if (index < SetBits.Count)
            {
                using (IEnumerator<ulong> occupancyPermutations = GetAllPermutations(SetBits, index, Value).GetEnumerator())
                {
                    while (occupancyPermutations.MoveNext())
                    {
                        yield return occupancyPermutations.Current;
                    }
                }
            }

        }
        public static ulong[] GenerateOccupancyBoardsForMasks(this int[] setBits)
        {
            var rv = new List<ulong>();
            for (int i = 1; i < setBits.Length; i++)
            {
                foreach (var combination in DifferentCombinations(setBits, i))
                {
                    var mask = (ulong)0;
                    foreach (var index in combination)
                    {
                        mask |= (ulong)1 << (short)index;
                    }
                    rv.Add(mask);
                }
            }
            return rv.Distinct().ToArray();
        }
        private static IEnumerable<IEnumerable<T>> DifferentCombinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } : k == 1 ? new[] { elements } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).DifferentCombinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
        public static ulong[] GetMaskPermutations(ulong mask)
        {
            List<int> setBits = mask.GetSetBits();

            var permutations = setBits.ToArray().GenerateOccupancyBoardsForMasks();

            return permutations;
        }
    }
}
