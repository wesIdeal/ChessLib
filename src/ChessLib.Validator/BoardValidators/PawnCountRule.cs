using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.BoardValidators
{
    public class PawnCountRule : IBoardRule
    {

        public BoardException Validate(in IBoard boardInfo)
        {
            var pawn = BoardHelpers.PAWN;
            var white = BoardHelpers.WHITE;
            var black = BoardHelpers.BLACK;
            var rv = BoardException.None;
            rv |= boardInfo.PiecePlacement.PieceOfColorOccupancy((Color)white, (Piece)pawn).CountSetBits() > 8 ? BoardException.WhiteTooManyPawns : BoardException.None;
            rv |= boardInfo.PiecePlacement.PieceOfColorOccupancy((Color)black, (Piece)pawn).CountSetBits() > 8 ? BoardException.BlackTooManyPawns : BoardException.None;
            return rv;
        }

    }
}

