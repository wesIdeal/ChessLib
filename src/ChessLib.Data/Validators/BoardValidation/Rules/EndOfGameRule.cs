using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation.Rules
{
    public class EndOfGameRule : IBoardRule
    {
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            var isDrawn = true;
            for (int c = 0; c < 2; c++)
            {
                for (int p = 0; p < 5; p++)
                {
                    if (boardInfo.GetPiecePlacement().Occupancy((Color)c, (Piece)p) != 0)
                    {
                        isDrawn = false;
                        break;
                    }
                }
            }

            if (isDrawn)
            {
                return BoardExceptionType.MaterialDraw;
            }
            if (boardInfo.IsActivePlayerInCheck())
            {
                if (boardInfo.IsCheckmate())
                {
                    return BoardExceptionType.Checkmate;
                }
            }

            if (boardInfo.IsStalemate())
            {
                return BoardExceptionType.Stalemate;
            }

            return BoardExceptionType.None;
        }
    }
}

