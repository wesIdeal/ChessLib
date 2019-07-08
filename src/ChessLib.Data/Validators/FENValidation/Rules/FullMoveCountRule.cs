using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Validators.FENValidation.Rules
{
    public class FullMoveCountRule : IFENRule
    {
        public FENError Validate(in string fen)
        {
            var hmClock = fen.GetFENPiece(FENPieces.FullMoveCounter);
            return ValidateFullMoveCount(hmClock);
        }
        private static FENError ValidateFullMoveCount(string n) => uint.TryParse(n, out _) ? FENError.None : FENError.FullMoveCounter;
    }
}

