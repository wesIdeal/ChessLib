using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.BoardValidation.Rules
{
    public class PawnCountRule : IBoardRule
    {
        public BoardExceptionType Validate(in Board boardInfo)
        {
            var rv = BoardExceptionType.None;
            rv |= boardInfo.Occupancy.Occupancy(Color.White, Piece.Pawn).CountSetBits() > 8
                ? BoardExceptionType.WhiteTooManyPawns
                : BoardExceptionType.None;
            rv |= boardInfo.Occupancy.Occupancy(Color.Black, Piece.Pawn).CountSetBits() > 8
                ? BoardExceptionType.BlackTooManyPawns
                : BoardExceptionType.None;
            return rv;
        }
    }
}