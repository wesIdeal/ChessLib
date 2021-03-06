﻿using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules
{
    public class PieceCanMoveToDestination : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {

            var piece = BoardHelpers.GetPieceAtIndex(boardInfo.GetPiecePlacement(), move.SourceIndex);
            if (piece == null)
            {
                return MoveError.ActivePlayerHasNoPieceOnSourceSquare;
            }
            
            return boardInfo.CanPieceMoveToDestination(move.SourceIndex, move.DestinationIndex)
                ? MoveError.NoneSet
                : MoveError.BadDestination;
        }


    }
}
