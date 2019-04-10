using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.MoveRules
{
    public class PawnDestinationValid : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var pawnAttacks = Bitboard.GetAttackedSquares(Piece.Pawn, move.SourceIndex, 0,
                boardInfo.ActivePlayerColor);
            var pawnMoves = PieceAttackPatternHelper.PawnMoveMask[(int)boardInfo.ActivePlayerColor][move.SourceIndex];
            var validMove = ((pawnMoves & move.DestinationValue) != 0) & ((move.DestinationValue & boardInfo.TotalOccupancy) == 0);
            var validAttack = ((pawnAttacks & move.DestinationValue) != 0 &&
                               (boardInfo.OpponentTotalOccupancy & move.DestinationValue) != 0);

            if (validMove || validAttack)
                return null;

            return MoveExceptionType.BadDestination;
        }
    }
}
