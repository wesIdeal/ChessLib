using System.Linq;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;

namespace ChessLib.Validators.FENValidation.Rules
{
    public class ActiveColorRule : IFENRule
    {
        public FENError Validate(in string fen)
        {
            var activeColor = fen.GetFENPiece(FENPieces.ActiveColor);
            return (new[] { "w", "b" }).Contains(activeColor) ? FENError.None : FENError.InvalidActiveColor;
        }
    }
}

