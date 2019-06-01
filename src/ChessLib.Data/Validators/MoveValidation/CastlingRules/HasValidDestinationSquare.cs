using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Interfaces;
using ChessLib.Validators;
using ChessLib.Validators.MoveValidation;
using System.Linq;
using ChessLib.Types.Enums;

namespace ChessLib.Validators.MoveValidation.CastlingRules
{
    public class HasValidDestinationSquare : IMoveRule
    {
        public readonly ushort[] ValidDestinationSquares = { 2, 6, 58, 62 };
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            if (move.MoveType != MoveType.Castle) return null;
            return ValidDestinationSquares.Contains(move.DestinationIndex)
                ? (MoveExceptionType?)null
                : MoveExceptionType.Castle_BadDestinationSquare;
        }
    }
}