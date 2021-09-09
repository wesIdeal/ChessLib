using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules
{
    public class KingDestinationValidator : IMoveRule
    {
        public readonly ushort[] ValidDestinationSquares = { 2, 6, 58, 62 };

        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            if (move.MoveType != MoveType.Castle)
            {
                return MoveError.NoneSet;
            }

            var castlingMoves = boardInfo.ActivePlayer == Color.Black
                ? MoveHelpers.BlackCastlingMoves
                : MoveHelpers.WhiteCastlingMoves;
            var moveIsCastlingMove = castlingMoves.Select(x => x.DestinationIndex).Contains(move.DestinationIndex);
            return moveIsCastlingMove ? MoveError.NoneSet : MoveError.CastleBadDestinationSquare;
        }
    }
}