using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.MoveValidation.CastlingRules
{
    public class NoPieceBlocksCastlingMoveValidator : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            if (move.MoveType == MoveType.Castle)
            {
                if (!MoveHelpers.CastlingMoves.Contains(move))
                {
                    return MoveError.BadDestination;
                }

                var rookMove = MoveHelpers.GetRookMoveForCastleMove(move);
                var occupancy = boardInfo.Occupancy.Occupancy();
                var squaresBetween = BoardHelpers.InBetween(rookMove.SourceIndex, move.SourceIndex);
                if ((squaresBetween & occupancy) != 0)
                {
                    return MoveError.CastleOccupancyBetween;
                }
            }

            return MoveError.NoneSet;
        }
    }
}