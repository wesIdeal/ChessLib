using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation.Rules
{
    public class EndOfGameRule : IBoardRule
    {
        public BoardException Validate(in IBoard boardInfo)
        {
            if (boardInfo.IsCheckmate())
            {
                return BoardException.Checkmate;
            }

            if (boardInfo.IsStalemate())
            {
                return BoardException.Stalemate;
            }

            return BoardException.None;
        }
    }
}

