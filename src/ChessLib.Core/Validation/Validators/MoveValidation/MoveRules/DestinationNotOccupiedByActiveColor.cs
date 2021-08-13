using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.MoveRules
{
    public class DestinationNotOccupiedByActiveColor : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var activeOccupancy = boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer);
            //Console.WriteLine($"Active Occupancy: {activeOccupancy}");
            //Console.WriteLine($"Destination: {move.DestinationValue}");

            if ((activeOccupancy & move.DestinationValue) != 0)
            {
                return MoveError.ActiveColorPieceAtDestination;
            }

            return MoveError.NoneSet;
        }
    }
}