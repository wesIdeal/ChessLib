using System.Linq;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.CastlingRules
{
    public class HasValidDestinationSquare : IMoveRule
    {
        public readonly ushort[] ValidDestinationSquares = { 2, 6, 58, 62 };
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            return ValidDestinationSquares.Contains(move.DestinationIndex)
                ? (MoveExceptionType?) null
                : MoveExceptionType.Castle_BadDestinationSquare;
        }
    }
}