using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.Validators.FENValidation.Rules
{
    public class FENStructureRule : IFENRule
    {
        public const char SeperationCharacter = ' ';

        public FENError Validate(string fen)
        {
            fen = FENHelpers.SanitizeFenString(fen);
            if (string.IsNullOrEmpty(fen))
            {
                return FENError.InvalidFENString;
            }

            var splitSectionsLength = fen.Split(SeperationCharacter).Length;
            if (splitSectionsLength != 6)
            {
                return FENError.InvalidFENString;
            }

            return FENError.None;
        }
    }
}