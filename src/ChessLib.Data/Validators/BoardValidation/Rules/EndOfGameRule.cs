using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation.Rules
{
    public class EndOfGameRule : IBoardRule
    {

        public BoardExceptionType Validate(ulong[][] occupancy, Color activeColor, 
            ushort? enPassantIdx, CastlingAvailability castlingAvailability)
        {
            var isDrawn = true;
            for (int c = 0; c < 2; c++)
            {
                for (int p = 0; p < 5; p++)
                {
                    if (occupancy.Occupancy((Color)c, (Piece)p) != 0)
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

            if (BoardHelpers.IsCheckmate(occupancy, activeColor))
            {
                return BoardExceptionType.Checkmate;
            }

            if (BoardHelpers.IsStalemate(occupancy, activeColor, enPassantIdx, castlingAvailability))
            {
                return BoardExceptionType.Stalemate;
            }

            return BoardExceptionType.None;
        }
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            return Validate(boardInfo.Occupancy, boardInfo.ActivePlayer, boardInfo.EnPassantSquare,
                boardInfo.CastlingAvailability);
        }
    }
}

