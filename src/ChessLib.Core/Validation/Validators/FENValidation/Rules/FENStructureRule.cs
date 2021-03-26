using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.Validators.FENValidation.Rules
{
    public class FENStructureRule : IFENRule
    {
        public FENError Validate(in string fen)
        {
            if (string.IsNullOrEmpty(fen)
                || fen.Split(' ').Length != 6)
            {
                return FENError.InvalidFENString;
            }

            return FENError.None;
        }

        public static void ValidateFENStructure(string fen)
        {

        }
    }
}
