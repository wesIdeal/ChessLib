using System.Linq;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules
{
    public class HasValidDestinationSquare : IMoveRule
    {
        public readonly ushort[] ValidDestinationSquares = {2, 6, 58, 62};

        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            if (move.MoveType != MoveType.Castle) return MoveError.NoneSet;
            return ValidDestinationSquares.Contains(move.DestinationIndex)
                ? MoveError.NoneSet
                : MoveError.CastleBadDestinationSquare;
        }
    }
}