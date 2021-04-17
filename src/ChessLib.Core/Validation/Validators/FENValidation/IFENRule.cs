using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.Validators.FENValidation
{
    public interface IFENRule
    {
        FENError Validate(string fen);
    }
}

