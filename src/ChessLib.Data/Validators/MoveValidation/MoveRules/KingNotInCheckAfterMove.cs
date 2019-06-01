﻿using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.MoveValidation.MoveRules
{
    public class KingNotInCheckAfterMove : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var activeKingIndex = postMoveBoard[(int)boardInfo.ActivePlayer][(int)Piece.King].GetSetBits()[0];
            if (Bitboard.IsSquareAttackedByColor(activeKingIndex, boardInfo.OpponentColor(), postMoveBoard))
            {
                return MoveExceptionType.MoveLeavesKingInCheck;
            }
            return null;
        }
    }
}