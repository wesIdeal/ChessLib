using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Interfaces;
using ChessLib.Validators;
using ChessLib.Validators.MoveValidation;

namespace ChessLib.Validators.MoveValidation.MoveRules
{
    public class DestinationNotOccupiedByActiveColor : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            if ((boardInfo.PiecePlacement.ColorOccupancy(boardInfo.ActivePlayer) & move.DestinationValue) != 0)
            {
                return MoveExceptionType.ActiveColorPieceAtDestination;
            }

            return null;
        }
    }
}
