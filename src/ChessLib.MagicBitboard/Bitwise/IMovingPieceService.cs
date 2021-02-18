using ChessLib.MagicBitboard.Storage;
using System.Collections.Generic;

namespace ChessLib.MagicBitboard.Bitwise
{
    internal interface IMovingPieceService
    {
        ushort[] GetSetBits(ulong u);
        ulong GetInBetweenSquares(ushort from, ushort to);
        ushort BitScanForward(ulong boardRep);
        ulong GetBoardValueOfIndex(ushort idx);
        ulong[] GetAllPermutations(ulong mask);
        IEnumerable<MoveObstructionBoard> GetAllPermutationsForAttackMask(ushort pieceLocationIndex, ulong moveMask,
             ulong attackMask, IEnumerable<ulong> occupancyBoards);
    }
}