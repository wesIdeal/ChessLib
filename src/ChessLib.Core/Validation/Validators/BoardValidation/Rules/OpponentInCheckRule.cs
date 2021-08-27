using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules
{
    public class OpponentInCheckRule : IBoardRule
    {
        public BoardExceptionType Validate(in Board boardInfo)
        {
            if (BoardHelpers.IsColorInCheck(boardInfo.Occupancy, boardInfo.ActivePlayer.Toggle()))
                return BoardExceptionType.OppositeCheck;
            return BoardExceptionType.None;
        }
    }
}