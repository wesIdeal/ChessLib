using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.MoveValidation;

namespace ChessLib.Validators.BoardValidators.Rules
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

