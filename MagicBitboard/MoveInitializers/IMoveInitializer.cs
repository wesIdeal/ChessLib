using System.Collections.Generic;

namespace MagicBitboard
{
    public interface IMoveInitializer
    {
        ulong GenerateMagicKey(BlockerAndMoveBoards[] blockerAndMoveBoards, int maskLength, out ulong[] attackArray);
        IEnumerable<BlockerAndMoveBoards> GetAllPermutationsForAttackMask(int piecePositionIndex, ulong attackMask, IEnumerable<ulong> n);
    }
}