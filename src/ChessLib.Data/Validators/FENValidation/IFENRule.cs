using ChessLib.Types.Enums;

namespace ChessLib.Data.Validators.FENValidation
{
    public interface IFENRule
    {
        FENError Validate(in string fen);
    }
}

