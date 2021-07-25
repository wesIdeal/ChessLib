using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules
{
    public class EnPassantSquareIndexRule : IEnPassantSquareRule
    {
        public BoardExceptionType Validate(in Board boardState)
        {
            var enPassantSquare = boardState.EnPassantIndex;
            var activeColor = boardState.ActivePlayer;
            return Validate(enPassantSquare, activeColor);
        }

        public BoardExceptionType Validate(BoardState boardState)
        {
            var enPassantSquare = boardState.EnPassantIndex;
            var activeColor = boardState.ActivePlayer;
            return Validate(enPassantSquare, activeColor);
        }

        internal virtual BoardExceptionType Validate(ushort? enPassantSquare, Color activeColor)
        {
            if (enPassantSquare == null)
            {
                return BoardExceptionType.None;
            }

            var enPassantRangeMask = activeColor == Color.White ? BoardConstants.Rank6 : BoardConstants.Rank3;
            var boardValueOfIndex = enPassantSquare.Value.GetBoardValueOfIndex();
            if ((enPassantRangeMask & boardValueOfIndex) == boardValueOfIndex)
            {
                return BoardExceptionType.None;
            }

            return BoardExceptionType.BadEnPassant;
        }
    }
}