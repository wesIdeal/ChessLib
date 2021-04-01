using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;
using EnumsNET;

namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules
{
    public class EndOfGameRule : IBoardRule
    {

        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            if (BoardHelpers.IsDrawn(boardInfo.Occupancy))
            {
                return BoardExceptionType.MaterialDraw;
            }

            if (BoardHelpers.IsCheckmate(boardInfo))
            {
                return BoardExceptionType.Checkmate;
            }

            if (BoardHelpers.IsStalemate(boardInfo.Occupancy,boardInfo.ActivePlayer,boardInfo.EnPassantIndex,boardInfo.CastlingAvailability))
            {
                return BoardExceptionType.Stalemate;
            }

            return BoardExceptionType.None;
        }
    }
}

