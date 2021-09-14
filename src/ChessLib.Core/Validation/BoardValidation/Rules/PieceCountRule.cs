using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.BoardValidation.Rules
{
    public class PieceCountRule : IBoardRule
    {
        public BoardExceptionType Validate(in Board boardInfo)
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