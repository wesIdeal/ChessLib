using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace MagicBitboard
{
    public static class PermutationHelpers
    {
       

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
