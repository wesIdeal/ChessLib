using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation.Rules
{
    public class OpponentInCheckRule : IBoardRule
    {
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            if (boardInfo.IsOpponentInCheck()) return BoardExceptionType.OppositeCheck;
            return BoardExceptionType.None;
        }
    }
}
