using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules
{
    public class PieceMovingIsActiveColor : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var activeOccupancy = boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer);
            var activeColorOccupiesSquare = (activeOccupancy & move.SourceValue) != 0;
            return
                activeColorOccupiesSquare ? MoveError.NoneSet : MoveError.ActivePlayerHasNoPieceOnSourceSquare;
        }
    }
};