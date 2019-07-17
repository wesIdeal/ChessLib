using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules
{
    public class DestinationNotOccupiedByActiveColor : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            if ((boardInfo.GetPiecePlacement().Occupancy(boardInfo.ActivePlayer) & move.DestinationValue) != 0)
            {
                return MoveError.ActiveColorPieceAtDestination;
            }

            return MoveError.NoneSet;
        }
    }
}
