using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation.Rules
{
    public class PawnCountRule : IBoardRule
    {

        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            var rv = BoardExceptionType.None;
            rv |= boardInfo.Occupancy.Occupancy(Color.White, Piece.Pawn).CountSetBits() > 8 ? BoardExceptionType.WhiteTooManyPawns : BoardExceptionType.None;
            rv |= boardInfo.Occupancy.Occupancy(Color.Black, Piece.Pawn).CountSetBits() > 8 ? BoardExceptionType.BlackTooManyPawns : BoardExceptionType.None;
            return rv;
        }

    }
}

