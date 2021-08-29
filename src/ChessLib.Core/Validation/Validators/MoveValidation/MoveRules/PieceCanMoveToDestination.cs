using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.MoveRules
{
    public class PieceCanMoveToDestination : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var piece = BoardHelpers.GetPieceAtIndex(boardInfo.Occupancy, move.SourceIndex);
            if (piece == null)
            {
                return MoveError.ActivePlayerHasNoPieceOnSourceSquare;
            }

            var moves = Bitboard.Instance.GetPseudoLegalMoves(move.SourceIndex, piece.Value, boardInfo.ActivePlayer,
                boardInfo.Occupancy.Occupancy());
            if (piece == Piece.Pawn)
            {
                if (move.DestinationIndex.GetFile() != move.SourceIndex.GetFile())
                {
                    if ((boardInfo.Occupancy.Occupancy(boardInfo.OpponentColor) & move.DestinationValue) == 0)
                    {
                        return MoveError.BadDestination;
                    }
                }
            }
            var attemptedMove = move;
            var isInMoveList = (moves & move.DestinationValue) == move.DestinationValue;
            return isInMoveList ? MoveError.NoneSet : MoveError.BadDestination;
        }
    }
}