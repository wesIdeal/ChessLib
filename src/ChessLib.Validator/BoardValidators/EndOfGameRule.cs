using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
namespace ChessLib.Validators.BoardValidators
{
    public class EndOfGameRule : IBoardRule
    {
        public BoardException Validate(in IBoard boardInfo)
        {
            var rv = BoardException.None;

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

