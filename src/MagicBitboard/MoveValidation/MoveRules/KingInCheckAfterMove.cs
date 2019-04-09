using System;
using System.Collections.Generic;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.MoveRules
{
    class KingInCheckAfterMove : IMoveRule
    {
        public ValidationResult Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var vr = new ValidationResult();
            if (Bitboard.IsAttackedBy(boardInfo.OpponentColor, boardInfo.ActivePlayerKingIndex, postMoveBoard))
            {
                vr.Severity = ValidationSeverity.Error;
                vr.ValidationIssue = Data.Exceptions.MoveExceptionType.MoveLeavesKingInCheck;
            }
            return vr;
        }
    }

    class PieceMovingIsActiveColor : IMoveRule
    {
        public ValidationResult Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var vr = new ValidationResult();
            var activePieces = boardInfo.A
            if ()
            {
                vr.Severity = ValidationSeverity.Error;
                vr.ValidationIssue = Data.Exceptions.MoveExceptionType.MoveLeavesKingInCheck;
            }
            return vr;
        }
    }
}
