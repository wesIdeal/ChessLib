using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.CastlingRules
{
    public class CastlingHasNoPiecesBlocking : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
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