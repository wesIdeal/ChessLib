﻿using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Validators;
using ChessLib.Validators.MoveValidation;

namespace ChessLib.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantIsPossible : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardFENInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            return (boardInfo.EnPassantSquare != move.DestinationIndex)
                ? MoveExceptionType.Ep_NotAvailalbe
                : (MoveExceptionType?)null;
        }
    }
}