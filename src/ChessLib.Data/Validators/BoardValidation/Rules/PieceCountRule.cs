using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation.Rules
{
    public class PieceCountRule : IBoardRule
    {
        public BoardException Validate(in IBoard boardInfo)
        {
            var rv = BoardException.None;
            if (boardInfo.GetPiecePlacement().Occupancy(Color.White).CountSetBits() > 16)
                rv |= BoardException.WhiteTooManyPieces;
            if (boardInfo.GetPiecePlacement().Occupancy(Color.Black).CountSetBits() > 16)
                rv |= BoardException.BlackTooManyPieces;
            return rv;
        }
    }
}
