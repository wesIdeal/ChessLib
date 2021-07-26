using System.Collections.Generic;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Helpers
{
    public static class MoveHelpers
    {
        public static IEnumerable<IMove> WhiteCastlingMoves
        {
            get
            {
                yield return WhiteCastleKingSide;
                yield return WhiteCastleQueenSide;
            }
        }

        public static IEnumerable<Move> BlackCastlingMoves
        {
            get
            {
                yield return BlackCastleKingSide;
                yield return BlackCastleQueenSide;
            }
        }
        public static readonly Move
            BlackCastleKingSide,
            BlackCastleQueenSide,
            WhiteCastleKingSide,
            WhiteCastleQueenSide;

        private static readonly Move BlackCastleKingSideRookMove,
            BlackCastleQueenSideRookMove,
            WhiteCastleKingSideRookMove,
            WhiteCastleQueenSideRookMove;

        static MoveHelpers()
        {
            BlackCastleKingSide = GenerateMove(60, 62, MoveType.Castle);
            BlackCastleQueenSide = GenerateMove(60, 58, MoveType.Castle);
            WhiteCastleKingSide = GenerateMove(4, 6, MoveType.Castle);
            WhiteCastleQueenSide = GenerateMove(4, 2, MoveType.Castle);
            BlackCastleKingSideRookMove = GenerateMove(63, 61);
            BlackCastleQueenSideRookMove = GenerateMove(56, 59);
            WhiteCastleKingSideRookMove = GenerateMove(7, 5);
            WhiteCastleQueenSideRookMove = GenerateMove(0, 3);
        }

        /// <summary>
        ///     Creates a <see cref="ChessLib.Core.Move" /> object from move details.
        /// </summary>
        /// <remarks>Basically makes a ushort from the 4 details needed to make a move</remarks>
        /// <returns></returns>
        public static Move GenerateMove(ushort fromIdx, ushort toIdx, MoveType moveType = MoveType.Normal,
            PromotionPiece promotionPiece = 0)
        {
            var mt = (ushort)moveType << 14;
            var pp = (ushort)promotionPiece << 12;
            var origin = fromIdx << 6;
            var dest = toIdx << 0;
            return new Move((ushort)(mt | pp | origin | dest));
        }

        public static IMove GetRookMoveForCastleMove(in IMove move)
        {
            switch (move.MoveValue)
            {
                case 53054:
                    return new Move(4093);
                case 53050:
                    return new Move(3643);
                case 49414:
                    return new Move(453);
                case 49410:
                    return new Move(3);
                default:
                    throw new MoveException(
                        $"{move} is probably not a castling move.\r\nError from GetRookMoveForCastleMove()");
            }
        }
    }
}