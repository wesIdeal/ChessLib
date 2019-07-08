using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Helpers
{
    public static class MoveHelpers
    {
        /// <summary>
        /// Creates a <see cref="MoveExt"/> object from move details.
        /// </summary>
        ///<remarks >Basically makes a ushort from the 4 details needed to make a move</remarks>
        /// <returns></returns>
        public static MoveExt GenerateMove(ushort fromIdx, ushort toIdx, MoveType moveType = MoveType.Normal, PromotionPiece promotionPiece = 0)
        {
            var mt = (ushort)moveType << 14;
            var pp = (ushort)promotionPiece << 12;
            var origin = fromIdx << 6;
            var dest = toIdx << 0;
            return new MoveExt((ushort)(mt | pp | origin | dest));
        }

    }

}
