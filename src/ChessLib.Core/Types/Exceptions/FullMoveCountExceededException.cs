using System;

namespace ChessLib.Core.Types.Exceptions
{
    public class FullMoveCountExceededException : Exception
    {
        public FullMoveCountExceededException(uint moveCount) : base(
            $"Full MoveValue Count exceeded limit of a maximum of 511 moves. Actual count was {moveCount}.")
        {
        }
    }
}