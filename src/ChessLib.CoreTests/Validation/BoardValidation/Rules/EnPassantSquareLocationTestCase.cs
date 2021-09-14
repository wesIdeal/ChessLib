#region

using ChessLib.Core.Types.Enums;

#endregion

namespace ChessLib.Core.Tests.Validation.BoardValidation.Rules
{
    public class EnPassantPawnLocationTestCase : EnPassantSquareLocationTestCase
    {
        public EnPassantPawnLocationTestCase(ulong pawnPlacement, ushort squareIndex, bool expectedValue, string description) : base(squareIndex, Color.White, expectedValue, description)
        {
            Occupancy = pawnPlacement;
        }

        public ulong Occupancy { get; }
    }
    public class EnPassantSquareLocationTestCase
    {
        public EnPassantSquareLocationTestCase(ushort squareIndex, Color activePlayerColor, bool expectedValue, string description)
        {
            SquareIndex = squareIndex;
            ActivePlayerColor = activePlayerColor;
            ExpectedValue = expectedValue;
            Description = description;
        }
        public string Description { get; }
        public bool ExpectedValue { get; }
        public ushort SquareIndex { get; }
        public Color ActivePlayerColor { get; }

    }
}