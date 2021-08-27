using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules
{
    public class CastlingHasNoPiecesBlocking : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            ulong piecesBetween;
            if (boardInfo.ActivePlayer == Color.Black)
            {
                if (move.Equals(MoveHelpers.BlackCastleKingSide) || move.Equals(MoveHelpers.BlackCastleQueenSide))
                {
                    piecesBetween = BoardHelpers.InBetween(move.SourceIndex, move.DestinationIndex);
                }
                else
                {
                    return MoveError.CastleBadDestinationSquare;
                }
            }
            else
            {
                if (move.Equals(MoveHelpers.WhiteCastleKingSide) || move.Equals(MoveHelpers.WhiteCastleQueenSide))
                {
                    piecesBetween = BoardHelpers.InBetween(move.SourceIndex, move.DestinationIndex);
                }
                else
                {
                    return MoveError.CastleBadDestinationSquare;
                }
            }

            if ((boardInfo.Occupancy.Occupancy() & piecesBetween) != 0)
            {
                return MoveError.CastleOccupancyBetween;
            }

            return MoveError.NoneSet;
        }
    }
}