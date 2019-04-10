using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.CastlingRules
{
    public class HasCastlingAvailability : IMoveRule
    {
        /// <summary>
        /// Validates castling move with availability flags
        /// </summary>
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
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
                    throw new MoveException("Bad destination square for castling move.", MoveExceptionType.Castle_BadDestinationSquare, move, boardInfo.ActivePlayerColor);
            }

            if (!boardInfo.CastlingAvailability.HasFlag(castleChar.Value))
            {
                return MoveExceptionType.Castle_Unavailable;
            }
            return null;

        }
    }
}