using System.Collections.Generic;

namespace ChessLib.Data.Types.Interfaces
{
    public interface IMoveInitializer
    {
        ulong GenerateMagicKey(IBlockerAndMoveBoards[] blockerAndMoveBoards, int maskLength, out ulong[] attackArray);
        IEnumerable<IBlockerAndMoveBoards> GetAllPermutationsForAttackMask(int piecePositionIndex, ulong attackMask, IEnumerable<ulong> n);
    }
}