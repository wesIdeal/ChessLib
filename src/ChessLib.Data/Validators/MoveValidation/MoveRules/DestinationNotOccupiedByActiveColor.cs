using System;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules
{
    public class DestinationNotOccupiedByActiveColor : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var activeOccupancy = boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer);
            Console.WriteLine($"Active Occupancy: {activeOccupancy}");
            Console.WriteLine($"Destination: {move.DestinationValue}");

            if ((activeOccupancy & move.DestinationValue) != 0)
            {
                return MoveError.ActiveColorPieceAtDestination;
            }

            return MoveError.NoneSet;
        }
    }
}
