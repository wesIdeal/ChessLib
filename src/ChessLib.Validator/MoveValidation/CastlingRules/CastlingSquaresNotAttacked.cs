using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using ChessLib.Validators;
using ChessLib.Validators.MoveValidation;

namespace ChessLib.Validators.MoveValidation.CastlingRules
{
    public class CastlingSquaresNotAttacked : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardFENInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            if (IsKingsPathInCheck(boardInfo.OpponentColor, boardInfo.PiecePlacement, move))
            {
                return MoveExceptionType.Castle_ThroughCheck;
            }
            return null;
        }
        /// <summary>
        /// Returns a boolean representing if castling path is attacked
        /// </summary>
        /// <param name="opponentColor"></param>
        /// <param name="occupancy"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        protected static bool IsKingsPathInCheck(in Color opponentColor, in ulong[][] occupancy, in MoveExt move)
        {
            var moveToAndFromValues = move.SourceValue | move.DestinationValue;
            var squaresBetween = BoardHelpers.InBetween(move.SourceIndex, move.DestinationIndex) | moveToAndFromValues;
            while (squaresBetween != 0)
            {
                var square = BitHelpers.BitScanForward(squaresBetween);
                if (Bitboard.IsSquareAttackedByColor(square, opponentColor, occupancy)) return true;
                squaresBetween &= squaresBetween - 1;
            }
            return false;
        }
    }
}
