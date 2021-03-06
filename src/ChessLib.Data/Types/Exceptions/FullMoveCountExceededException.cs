using System;

namespace ChessLib.Data.Types.Exceptions
{
    internal class FullMoveCountExceededException : Exception
    {
        public FullMoveCountExceededException(int moveCount) : base(
            $"Full Move Count exceeded limit of a maximum of 511 moves. Actual count was {moveCount}.")
        {
        }
    }
}