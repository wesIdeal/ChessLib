using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.MoveRules
{
    public class PieceCanMoveToDestination : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {

            var piece = BoardHelpers.GetPieceAtIndex(boardInfo.Occupancy, move.SourceIndex);
            if (piece == null)
            {
                return MoveError.ActivePlayerHasNoPieceOnSourceSquare;
            }

            var moves = Bitboard.Instance.GetPseudoLegalMoves(move.SourceIndex, piece.Value, boardInfo.ActivePlayer,
                 boardInfo.Occupancy.Occupancy());
            var isInMoveList = (moves & move.DestinationValue) == move.DestinationValue;
            return isInMoveList ? MoveError.NoneSet : MoveError.BadDestination;
        }


    }
}
