using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.EnPassantRules
{
    public class SourceIsCorrectRank : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var rank = move.SourceIndex / 8;
            var error = MoveExceptionType.EP_WrongSourceRank;
            if ((boardInfo.ActivePlayerColor == Color.Black && rank == 3) ||
                boardInfo.ActivePlayerColor == Color.White && rank == 4)
                return null;
            return error;
        }
    }
}