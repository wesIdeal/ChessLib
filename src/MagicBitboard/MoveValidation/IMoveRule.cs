using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.MagicBitboard.MoveValidation
{

    interface IMoveRule
    {
        MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move);
    }
}
