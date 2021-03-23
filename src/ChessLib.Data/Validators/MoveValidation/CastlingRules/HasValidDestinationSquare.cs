using System.Linq;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.CastlingRules
{
    public class HasValidDestinationSquare : IMoveRule
    {
        public readonly ushort[] ValidDestinationSquares = { 2, 6, 58, 62 };
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            if (move.MoveType != MoveType.Castle) return MoveError.NoneSet;
            return ValidDestinationSquares.Contains(move.DestinationIndex)
                ? MoveError.NoneSet
                : MoveError.CastleBadDestinationSquare;
        }
    }
}