using System.Runtime.CompilerServices;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules")]
namespace ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules
{
    public class AttackNotBlockingMoveValidator : IMoveRule
    {
        internal AttackNotBlockingMoveValidator(IBitboard bitboard)
        {
            _bitboard = bitboard;
        }

        public AttackNotBlockingMoveValidator() : this(Bitboard.Instance)
        {

        }

        private readonly IBitboard _bitboard;

        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            if (IsKingsPathInCheck(boardInfo.ActivePlayer.Toggle(), boardInfo.Occupancy, move,
                boardInfo.EnPassantIndex))
            {
                return MoveError.CastleThroughCheck;
            }

            return MoveError.NoneSet;
        }

        /// <summary>
        ///     Returns a boolean representing if castling path is attacked
        /// </summary>
        /// <param name="opponentColor"></param>
        /// <param name="occupancy"></param>
        /// <param name="move"></param>
        /// <param name="enPassant"></param>
        /// <returns></returns>
        private bool IsKingsPathInCheck(in Color opponentColor, in ulong[][] occupancy, in IMove move,
            ushort? enPassant)
        {
            var squaresBetween = BoardHelpers.InBetween(move.SourceIndex, move.DestinationIndex) | move.DestinationValue;
            var rv = false;
            foreach (var square in squaresBetween.GetSetBits())
            {
                rv |= _bitboard.IsSquareAttackedByColor(square, opponentColor, occupancy, enPassant);
            }

            return rv;
        }
    }
}