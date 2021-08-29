using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.MoveRules
{
    public class MoveDestinationValidator : IMoveRule
    {
        public MoveDestinationValidator() : this(Bitboard.Instance)
        {
        }

        public MoveDestinationValidator(IBitboard bitboard)
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
            var totalOccupancy = boardInfo.Occupancy.Occupancy();
            var moves = _bitboard.GetPseudoLegalMoves(move.SourceIndex, piece.Value, boardInfo.ActivePlayer,
                totalOccupancy);
            var isInMoveList = (moves & move.DestinationValue) == move.DestinationValue;
            if (piece == Piece.Pawn && isInMoveList)
            {
                // if this is a capture and nothing occupies the space, it could be invalid or en passant
                if (move.DestinationIndex.GetFile() != move.SourceIndex.GetFile() &&
                    (boardInfo.Occupancy.Occupancy(boardInfo.OpponentColor) & move.DestinationValue) == 0)
                {
                    if (move.DestinationIndex == boardInfo.EnPassantIndex)
                    {
                        if (move.MoveType == MoveType.EnPassant)
                        {
                            isInMoveList = true;
                        }
                        else
                        {
                            return MoveError.EnPassantNotMarked;
                        }
                    }
                    else
                    {
                        isInMoveList = false;
                    }
                }
            }

            return isInMoveList ? MoveError.NoneSet : MoveError.BadDestination;
        }
    }
}