using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.MoveRules
{
    public class NotInCheckAfterMoveValidator : IMoveRule
    {
        public NotInCheckAfterMoveValidator() : this(Bitboard.Instance)
        {
        }

        internal NotInCheckAfterMoveValidator(IBitboard bitboardInstance)
        {
            _bitboardInstance = bitboardInstance;
        }

        private readonly IBitboard _bitboardInstance;

        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var activeKingIndex = postMoveBoard[(int)boardInfo.ActivePlayer][(int)Piece.King].GetSetBits()[0];
            var attackerColor = boardInfo.ActivePlayer.Toggle();
            var enPassantSquareIndex = boardInfo.EnPassantIndex;
            var isKingSquareAttacked =
                _bitboardInstance.IsSquareAttackedByColor(activeKingIndex, attackerColor, postMoveBoard,
                    enPassantSquareIndex);
            return isKingSquareAttacked ? MoveError.MoveLeavesKingInCheck : MoveError.NoneSet;
        }
    }
}