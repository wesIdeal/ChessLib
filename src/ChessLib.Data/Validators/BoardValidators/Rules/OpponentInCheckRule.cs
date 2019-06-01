using System;
using System.Collections.Generic;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.BoardValidators.Rules
{
    public class OpponentInCheckRule : IBoardRule
    {
        public BoardException Validate(in IBoard boardInfo)
        {
            if (boardInfo.IsOpponentInCheck()) return BoardException.OppositeCheck;
            return BoardException.None;
        }
    }
}
