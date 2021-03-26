using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules
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
