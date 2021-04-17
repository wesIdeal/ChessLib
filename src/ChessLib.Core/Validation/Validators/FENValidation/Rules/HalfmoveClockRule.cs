using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.Validators.FENValidation.Rules
{
    public class HalfmoveClockRule : IFENRule
    {
        public FENError Validate(string fen)
        {
            var hmClock = fen.GetFENPiece(FENPieces.HalfmoveClock);
            return ValidateHalfmoveClock(hmClock);
        }
        private static FENError ValidateHalfmoveClock(string n) => uint.TryParse(n, out _) ? FENError.None : FENError.HalfmoveClock;
    }
}

