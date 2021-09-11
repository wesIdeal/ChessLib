using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.MoveValidation.EnPassantRules
{
    /// <summary>
    /// Validates that the destination square is the specified en passant square
    /// </summary>
    public class EnPassantDestinationValidator : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            return boardInfo.EnPassantIndex != move.DestinationIndex
                ? MoveError.EpNotAvailable
                : MoveError.NoneSet;
        }
    }
}