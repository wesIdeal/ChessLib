﻿using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.Validators.FENValidation.Rules
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
