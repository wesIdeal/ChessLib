using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

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
