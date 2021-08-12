using System;
using System.Runtime.CompilerServices;
using ChessLib.Core.Types.Enums;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Helpers")]

namespace ChessLib.Core.Types.Exceptions
{
    public class FullMoveCountExceededException : Exception
    {
        internal const string MessageFormat =
            "Full Move Counter exceeded limit of a maximum of 511 moves. Actual count was {0}.";
        public uint ExcessiveMoveCount { get; private set; }
        public FullMoveCountExceededException(uint moveCount) : base(string.Format(MessageFormat, moveCount))
        {
            ExcessiveMoveCount = moveCount;
        }
    }

    public class GameException : Exception
    {

        public uint ExcessiveMoveCount { get; private set; }
        public GameException(GameError error) : base($"Game error {error} occurred.")
        {
            Error = error;
        }

        public GameError Error { get; }
    }
}