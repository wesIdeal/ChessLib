using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.FENValidation
{
    public interface IFENRule
    {
        FENError Validate(in string fen);
    }
}

