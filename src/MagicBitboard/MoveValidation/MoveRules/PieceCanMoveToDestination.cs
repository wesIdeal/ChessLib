using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.MoveRules
{
    public class PieceCanMoveToDestination : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {

            var piece = BoardHelpers.GetTypeOfPieceAtIndex(move.SourceIndex, boardInfo.PiecesOnBoard);
            if (piece == null)
            {
                return MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare;
            }

            var moves = Bitboard.GetAttackedSquares(piece.Value, move.SourceIndex, boardInfo.TotalOccupancy,
                boardInfo.ActivePlayerColor);
            
            if ((moves & move.DestinationValue) == 0)
            {
                return MoveExceptionType.BadDestination;
            }

            return null;
        }
    }
}
