
using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;

namespace ChessLib.Validators.MoveValidation.MoveRules
{
    public class PieceCanMoveToDestination : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {

            var piece = BoardHelpers.GetTypeOfPieceAtIndex(move.SourceIndex, boardInfo.PiecePlacement);
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
