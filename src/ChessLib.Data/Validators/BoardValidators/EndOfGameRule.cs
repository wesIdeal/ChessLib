using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.MoveValidation;

namespace ChessLib.Validators.BoardValidators
{
    public class EndOfGameRule : IBoardRule
    {
        public BoardException Validate(in IBoard boardInfo)
        {
            if (boardInfo.IsCheckmate())
            {
                return BoardException.Checkmate;
            }

            if (boardInfo.IsStalemate<MoveValidator>())
            {
                return BoardException.Stalemate;
            }

            return BoardException.None;
        }





    }
}

