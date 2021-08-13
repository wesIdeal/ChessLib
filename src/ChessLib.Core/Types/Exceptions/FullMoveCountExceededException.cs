using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Helpers")]

namespace ChessLib.Core.Types.Exceptions
{
    public class FullMoveCountExceededException : Exception
    {
        internal const string MessageFormat =
            "Full Move Counter exceeded limit of a maximum of 511 moves. Actual count was {0}.";

        public uint ExcessiveMoveCount { get; }

        public FullMoveCountExceededException(uint moveCount) : base(string.Format(MessageFormat, moveCount))
        {
            ExcessiveMoveCount = moveCount;
        }
    }
}