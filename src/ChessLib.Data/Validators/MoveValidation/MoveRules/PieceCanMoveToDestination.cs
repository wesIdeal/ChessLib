
using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.MoveValidation.MoveRules
{
    public class PieceCanMoveToDestination : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {

            var piece = BoardHelpers.GetTypeOfPieceAtIndex(move.SourceIndex, boardInfo.GetPiecePlacement());
            if (piece == null)
            {
                return MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare;
            }
            return boardInfo.CanPieceMoveToDestination(move.SourceIndex, move.DestinationIndex)
                ? (MoveExceptionType?)null
                : MoveExceptionType.BadDestination;
        }


    }
}
