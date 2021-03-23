﻿using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules
{
    public class KingNotInCheckAfterMove : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var activeKingIndex = postMoveBoard[(int)boardInfo.ActivePlayer][(int)Piece.King].GetSetBits()[0];
            if (activeKingIndex.IsSquareAttackedByColor(boardInfo.OpponentColor(), postMoveBoard, activeKingIndex))
            {
                return MoveError.MoveLeavesKingInCheck;
            }
            return MoveError.NoneSet;
        }
    }
}
