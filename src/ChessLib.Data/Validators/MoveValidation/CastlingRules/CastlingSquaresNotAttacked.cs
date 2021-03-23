using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.CastlingRules
{
    public class CastlingSquaresNotAttacked : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            if (IsKingsPathInCheck(boardInfo.OpponentColor(), boardInfo.Occupancy, move))
            {
                return MoveError.CastleThroughCheck;
            }
            return MoveError.NoneSet;
        }
        /// <summary>
        /// Returns a boolean representing if castling path is attacked
        /// </summary>
        /// <param name="opponentColor"></param>
        /// <param name="occupancy"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        protected static bool IsKingsPathInCheck(in Color opponentColor, in ulong[][] occupancy, in IMoveExt move)
        {
            var moveToAndFromValues = move.SourceValue | move.DestinationValue;
            var squaresBetween = BoardHelpers.InBetween(move.SourceIndex, move.DestinationIndex) | moveToAndFromValues;
            while (squaresBetween != 0)
            {
                var square = BitHelpers.BitScanForward(squaresBetween);
                if (square.IsSquareAttackedByColor(opponentColor, occupancy, null)) return true;
                squaresBetween &= squaresBetween - 1;
            }
            return false;
        }
    }
}
