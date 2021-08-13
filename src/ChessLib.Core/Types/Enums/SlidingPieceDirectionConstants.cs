namespace ChessLib.Core.Types.Enums
{
    public static class SlidingPieceDirectionConstants
    {
        public const MoveDirection RookDirections =
            MoveDirection.N | MoveDirection.E | MoveDirection.S | MoveDirection.W;

        public const MoveDirection BishopDirections =
            MoveDirection.NE | MoveDirection.SE | MoveDirection.SW | MoveDirection.NW;
    }
}