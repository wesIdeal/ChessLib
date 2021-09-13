using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.MoveValidation.MoveRules
{
    /// <summary>
    /// Validates the move's source square is occupied by the active player
    /// </summary>
    public class ActiveColorValidator : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in IMove move)
        {
            var activeOccupancy = boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer);
            var activeColorOccupiesSquare = (activeOccupancy & move.SourceValue) != 0;
            return
                activeColorOccupiesSquare ? MoveError.NoneSet : MoveError.ActivePlayerHasNoPieceOnSourceSquare;
        }
    }
}