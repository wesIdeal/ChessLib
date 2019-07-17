using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation.Rules
{
    public class EndOfGameRule : IBoardRule
    {
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            if (boardInfo.IsCheckmate())
            {
                return BoardExceptionType.Checkmate;
            }

            if (boardInfo.IsStalemate())
            {
                return BoardExceptionType.Stalemate;
            }

            return BoardExceptionType.None;
        }
    }
}

