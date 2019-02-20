using System.Collections.Generic;
using System.Linq;

namespace MagicBitboard
{
    public abstract class MoveInitializer : IMoveInitializer
    {
        public abstract ulong CalculateMovesFromPosition(int positionIndex, ulong occupancyBoard);

        public abstract ulong GenerateKey(BlockerAndMoveBoards[] blockerAndMoveBoards, int maskLength);

        public abstract IEnumerable<BlockerAndMoveBoards> GetPermutationsForMask(ulong attackMask, IEnumerable<ulong> n, int piecePositionIndex);

        public static IEnumerable<ulong> GetAllPermutations(ulong mask)
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
    }
}