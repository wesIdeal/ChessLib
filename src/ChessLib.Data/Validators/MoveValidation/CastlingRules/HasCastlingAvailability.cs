﻿using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.MoveValidation.CastlingRules
{
    public class HasCastlingAvailability : IMoveRule
    {
        /// <summary>
        /// Validates castling move with availability flags
        /// </summary>
        public MoveError? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            CastlingAvailability? castleChar;
            switch (move.DestinationIndex)
            {
                case 58:
                    castleChar = CastlingAvailability.BlackQueenside;
                    break;
                case 62:
                    castleChar = CastlingAvailability.BlackKingside;
                    break;
                case 2:
                    castleChar = CastlingAvailability.WhiteQueenside;
                    break;
                case 6:
                    castleChar = CastlingAvailability.WhiteKingside;
                    break;
                default:
                    throw new MoveException("Bad destination square for castling move.", MoveError.Castle_BadDestinationSquare, move, boardInfo.ActivePlayer);
            }

            if (!boardInfo.CastlingAvailability.HasFlag(castleChar.Value))
            {
                return MoveError.Castle_Unavailable;
            }
            return null;

        }
    }
}