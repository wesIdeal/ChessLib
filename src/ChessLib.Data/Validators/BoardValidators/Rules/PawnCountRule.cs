using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.BoardValidators.Rules 
{
    public class PawnCountRule : IBoardRule
    {

        public BoardException Validate(in IBoard boardInfo)
        {
            var rv = BoardException.None;
            rv |= boardInfo.GetPiecePlacement().Occupancy(Color.White, Piece.Pawn).CountSetBits() > 8 ? BoardException.WhiteTooManyPawns : BoardException.None;
            rv |= boardInfo.GetPiecePlacement().Occupancy(Color.Black, Piece.Pawn).CountSetBits() > 8 ? BoardException.BlackTooManyPawns : BoardException.None;
            return rv;
        }

    }
}

