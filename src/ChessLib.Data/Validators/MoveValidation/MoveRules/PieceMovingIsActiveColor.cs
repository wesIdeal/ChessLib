using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules
{
    public class PieceMovingIsActiveColor : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var activePieces = boardInfo.GetPiecePlacement().Occupancy(boardInfo.ActivePlayer);
            if ((activePieces & move.SourceValue) == 0)
            {
                return MoveError.ActivePlayerHasNoPieceOnSourceSquare;
            }
            return MoveError.NoneSet;
        }
    }
}