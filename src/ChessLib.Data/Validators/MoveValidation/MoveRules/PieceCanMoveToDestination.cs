using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules
{
    public class PieceCanMoveToDestination : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {

            var piece = BoardHelpers.GetPieceAtIndex(boardInfo.Occupancy, move.SourceIndex);
            if (piece == null)
            {
                return MoveError.ActivePlayerHasNoPieceOnSourceSquare;
            }
            
            return boardInfo.CanPieceMoveToDestination(move.SourceIndex, move.DestinationIndex)
                ? MoveError.NoneSet
                : MoveError.BadDestination;
        }


    }
}
