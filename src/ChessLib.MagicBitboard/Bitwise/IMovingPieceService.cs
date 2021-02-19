using ChessLib.MagicBitboard.Storage;
using System.Collections.Generic;

namespace ChessLib.MagicBitboard.Bitwise
{
    internal interface IMovingPieceService
    {
        ulong GetInBetweenSquares(ushort from, ushort to);
        ulong[] GetAllPermutations(ulong mask);
        //IEnumerable<MoveObstructionBoard> GetAllPermutationsForAttackMask(ushort pieceLocationIndex, ulong moveMask,
        //     ulong attackMask, IEnumerable<ulong> occupancyBoards);
    }
}