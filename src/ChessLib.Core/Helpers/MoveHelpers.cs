using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Helpers
{
    public static class MoveHelpers
    {
        public static readonly Move
            BlackCastleKingSide,
            BlackCastleQueenSide,
            WhiteCastleKingSide,
            WhiteCastleQueenSide;

        public static IEnumerable<Move> WhiteCastlingMoves
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

        public static IEnumerable<Move> CastlingMoves => WhiteCastlingMoves.Concat(BlackCastlingMoves);

        public static IEnumerable<Move> QueenSideCastlingMoves => new[] { WhiteCastleQueenSide, BlackCastleQueenSide };
        public static IEnumerable<Move> KingSideCastlingMoves => new[] { WhiteCastleKingSide, BlackCastleKingSide };

        static MoveHelpers()
        {
            BlackCastleKingSide = GenerateMove(60, 62, MoveType.Castle);
            BlackCastleQueenSide = GenerateMove(60, 58, MoveType.Castle);
            WhiteCastleKingSide = GenerateMove(4, 6, MoveType.Castle);
            WhiteCastleQueenSide = GenerateMove(4, 2, MoveType.Castle);
        }

        /// <summary>
        ///     Creates a <see cref="Move" /> object from move details.
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

        /// <summary>
        ///     Gets the corresponding rook move for a castling move
        /// </summary>
        /// <param name="move"></param>
        /// <returns>A move containing rook source and destination</returns>
        /// <exception cref="MoveException">Thrown when move is not a castling move.</exception>
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