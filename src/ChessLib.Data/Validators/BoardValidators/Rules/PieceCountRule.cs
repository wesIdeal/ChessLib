using System;
using System.Collections.Generic;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.BoardValidators.Rules
{
    public class PieceCountRule : IBoardRule
    {
        public BoardException Validate(in IBoard boardInfo)
        {
            var rv = BoardException.None;
            if (boardInfo.GetPiecePlacement().Occupancy(Color.White).CountSetBits() > 16)
                rv |= BoardException.WhiteTooManyPieces;
            if (boardInfo.GetPiecePlacement().Occupancy(Color.Black).CountSetBits() > 16)
                rv |= BoardException.BlackTooManyPieces;
            return rv;
        }
    }
}
