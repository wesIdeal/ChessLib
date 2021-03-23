using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;

namespace ChessLib.Data.Helpers
{
    internal static class MoveHelpers
    {
        public static readonly MoveExt BlackCastleKingSide, BlackCastleQueenSide, WhiteCastleKingSide, WhiteCastleQueenSide;

        private static readonly MoveExt BlackCastleKingSideRookMove,
            BlackCastleQueenSideRookMove,
            WhiteCastleKingSideRookMove,
            WhiteCastleQueenSideRookMove;

        static MoveHelpers()
        {
            BlackCastleKingSide = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            BlackCastleQueenSide = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
            WhiteCastleKingSide = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
            WhiteCastleQueenSide = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
            BlackCastleKingSideRookMove = MoveHelpers.GenerateMove(63, 61);
            BlackCastleQueenSideRookMove = GenerateMove(56, 59);
            WhiteCastleKingSideRookMove = GenerateMove(7, 5);
            WhiteCastleQueenSideRookMove = GenerateMove(0, 3);
        }
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

        public static MoveExt GetRookMoveForCastleMove(in MoveExt move)
        {
            switch (move.Move)
            {
                case 53054:
                    return new MoveExt(4093);
                case 53050:
                    return new MoveExt(3643);
                case 49414:
                    return new MoveExt(453);
                case 49410:
                    return new MoveExt(3);
                default: throw new MoveException($"{move} is probably not a castling move.\r\nError from GetRookMoveForCastleMove()");
            }
        }
    }

}
