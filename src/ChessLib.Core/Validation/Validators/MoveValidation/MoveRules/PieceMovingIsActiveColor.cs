using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.MoveRules
{
    public class PieceMovingIsActiveColor : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var activeOccupancy = boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer);
            var activeColorOccupiesSquare = (activeOccupancy & move.SourceValue) != 0;
            return
                activeColorOccupiesSquare ? MoveError.NoneSet : MoveError.ActivePlayerHasNoPieceOnSourceSquare;
        }
    }
}