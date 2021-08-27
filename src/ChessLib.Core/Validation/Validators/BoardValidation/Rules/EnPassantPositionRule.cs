using System.Runtime.CompilerServices;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules
{
    /// <summary>
    ///     Validates that board parameters are correct in regards to en passant square
    /// </summary>
    public class EnPassantPositionRule : IBoardRule
    {
        public virtual BoardExceptionType Validate(in Board boardInfo)
        {
            //Debug.WriteLine("Here in Board Validation->EP Validation");
            var enPassantSquare = boardInfo.EnPassantIndex;
            var returnVal = BoardExceptionType.BadEnPassant;
            if (enPassantSquare == null)
            {
                return BoardExceptionType.None;
            }


            var opponentPawnOccupancy = boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer.Toggle(), Piece.Pawn);
            var isSquareOnCorrectRank =
                IsEnPassantSquareCorrectForActive(boardInfo.ActivePlayer, enPassantSquare.Value);
            if (isSquareOnCorrectRank)
            {
                if (IsPawnPresentNorthOfEnPassantSquare(opponentPawnOccupancy, enPassantSquare.Value))
                {
                    returnVal = BoardExceptionType.None;
                }
            }

            return returnVal;
        }

        internal virtual bool IsEnPassantSquareCorrectForActive(Color boardInfoActivePlayer, ushort enPassantSquare)
        {
            var opponentColor = boardInfoActivePlayer.Toggle();
            //Should be the third rank, relative to opponent's perspective.
            var rank = enPassantSquare.GetRank();
            var isCorrect = opponentColor == Color.Black && rank == (ushort)Rank.R6;
            isCorrect |= opponentColor == Color.White && rank == (ushort)Rank.R3;
            return isCorrect;
        }


        internal virtual bool IsPawnPresentNorthOfEnPassantSquare(ulong opponentPawnOccupancy, ushort enPassantSquare)
        {
            var epRank = enPassantSquare.GetRank();
            var epValue = enPassantSquare.GetBoardValueOfIndex();
            var possiblePawnLocation = epRank == (ushort)Rank.R3 ? epValue.ShiftN() : epValue.ShiftS();
            var isPawnPresentNorthOfEnPassantSquare = (opponentPawnOccupancy & possiblePawnLocation) != 0;
            return isPawnPresentNorthOfEnPassantSquare;
        }
    }
}