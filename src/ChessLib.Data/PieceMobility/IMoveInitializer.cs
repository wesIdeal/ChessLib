using System.Collections.Generic;

namespace ChessLib.Data.PieceMobility
{
    public interface IMoveInitializer
    {
        ulong GenerateMagicKey(BlockerAndMoveBoards[] blockerAndMoveBoards, int maskLength, out ulong[] attackArray);
        IEnumerable<BlockerAndMoveBoards> GetAllPermutationsForAttackMask(int piecePositionIndex, ulong attackMask, IEnumerable<ulong> n);
    }
}