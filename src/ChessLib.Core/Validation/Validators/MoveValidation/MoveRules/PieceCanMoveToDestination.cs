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
        public PieceCanMoveToDestination() : this(Bitboard.Instance)
        {
        }

        public PieceCanMoveToDestination(IBitboard bitboard)
        {
            _bitboard = bitboard;
        }

        private readonly IBitboard _bitboard;

        /// <summary>
        ///     Validates that a proposed move is legally able to reach its destination
        /// </summary>
        /// <param name="boardInfo"></param>
        /// <param name="postMoveBoard"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var piece = BoardHelpers.GetPieceAtIndex(boardInfo.Occupancy, move.SourceIndex);
            if (piece == null)
            {
                return MoveError.ActivePlayerHasNoPieceOnSourceSquare;
            }

            var moves = _bitboard.GetPseudoLegalMoves(move.SourceIndex, piece.Value, boardInfo.ActivePlayer,
                boardInfo.Occupancy.Occupancy());
            var isInMoveList = (moves & move.DestinationValue) == move.DestinationValue;
            if (isInMoveList && piece == Piece.Pawn)
            {
                if (move.DestinationIndex.GetFile() != move.SourceIndex.GetFile() &&
                    (boardInfo.Occupancy.Occupancy(boardInfo.OpponentColor) & move.DestinationValue) == 0)
                {
                    return MoveError.BadDestination;
                }
            }

            return isInMoveList ? MoveError.NoneSet : MoveError.BadDestination;
        }
    }
}