using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.MoveValidation.CastlingRules
{
    public class NotInCheckBeforeMoveValidator : IMoveRule
    {
        private readonly IBitboard _bitboard;

        public NotInCheckBeforeMoveValidator() : this(Bitboard.Instance)
        {

        }
        public NotInCheckBeforeMoveValidator(IBitboard bitboard)
        {
            _bitboard = bitboard;
        }

        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var kingIndex = boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer, Piece.King).GetSetBits()[0];
            var isOpponentInCheck = _bitboard.IsSquareAttackedByColor(kingIndex, boardInfo.OpponentColor, boardInfo.Occupancy, boardInfo.EnPassantIndex);
            return isOpponentInCheck ? MoveError.CastleKingInCheck : MoveError.NoneSet;
        }
    }
}