using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation.Rules
{
    public class PieceCountRule : IBoardRule
    {
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            var rv = BoardExceptionType.None;
            if (boardInfo.Occupancy.Occupancy(Color.White).CountSetBits() > 16)
                rv |= BoardExceptionType.WhiteTooManyPieces;
            if (boardInfo.Occupancy.Occupancy(Color.Black).CountSetBits() > 16)
                rv |= BoardExceptionType.BlackTooManyPieces;
            return rv;
        }
    }
}
