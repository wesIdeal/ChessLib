using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using MagicBitboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.MagicBitboard.MoveValidation
{

    interface IMoveRule
    {
        ValidationResult Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move);
    }
}
