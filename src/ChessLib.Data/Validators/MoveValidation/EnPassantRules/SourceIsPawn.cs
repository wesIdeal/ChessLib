using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.EnPassantRules
{
    public class SourceIsPawn : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var isPawn = (boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer, Piece.Pawn) & move.SourceValue) != 0;
            return isPawn ? MoveError.NoneSet : MoveError.EpSourceIsNotPawn;
        }
    }
}