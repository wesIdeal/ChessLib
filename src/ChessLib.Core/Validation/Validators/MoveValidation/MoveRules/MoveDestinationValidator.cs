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
            var piece = boardInfo.Occupancy.GetPieceOfColorAtIndex(move.SourceIndex);
            if (piece == null)
            {
                return MoveError.ActivePlayerHasNoPieceOnSourceSquare;
            }

            if ((boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer) & move.DestinationValue) != 0)
            {
                return MoveError.ActiveColorPieceAtDestination;
            }

            var totalOccupancy = boardInfo.Occupancy.Occupancy();
            if (piece.Value.Piece == Piece.Pawn)
            {
                totalOccupancy |= boardInfo.EnPassantIndex?.GetBoardValueOfIndex() ?? 0;
            }
            var moves = _bitboard.GetPseudoLegalMoves(move.SourceIndex, piece.Value.Piece, boardInfo.ActivePlayer,
                totalOccupancy);
            
            var isInMoveList = (moves & move.DestinationValue) == move.DestinationValue;
            if (piece.Value.Piece == Piece.Pawn && isInMoveList)
            {
                // if this is a capture and nothing occupies the space, it could be invalid or en passant
                if (move.DestinationIndex.GetFile() != move.SourceIndex.GetFile() &&
                    (boardInfo.Occupancy.Occupancy(boardInfo.OpponentColor) & move.DestinationValue) == 0)
                {
                    if (move.DestinationIndex == boardInfo.EnPassantIndex)
                    {
                        if (move.MoveType != MoveType.EnPassant)
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