using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules
{
    public class CastlingMoveIsAvailableValidator : IMoveRule
    {
        /// <summary>
        ///     Validates castling move with availability flags
        /// </summary>
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
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
                    throw new MoveException("Bad destination square for castling move.",
                        MoveError.CastleBadDestinationSquare, move, boardInfo.ActivePlayer);
            }

            if (!boardInfo.CastlingAvailability.HasFlag(castleChar.Value))
            {
                return MoveError.CastleUnavailable;
            }

            return MoveError.NoneSet;
        }
    }
}