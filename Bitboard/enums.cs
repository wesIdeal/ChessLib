using System;

namespace MagicBitboard.Enums
{
    public enum Rank { R1 = 0, R2, R3, R4, R5, R6, R7, R8 };
    public enum File { A = 0, B, C, D, E, F, G, H };
    public enum Color { White = 0, Black }

    [Flags]
    public enum MoveDirection
    {
        N = 0,
        NE = 1,
        E = 2,
        SE = 4,
        S = 8,
        SW = 16,
        W = 32,
        NW = 64
    }
    public static class SlidingPieceDirectionContants
    {
        public const MoveDirection RookDirections = MoveDirection.N | MoveDirection.E | MoveDirection.S | MoveDirection.W;
        public const MoveDirection BishopDirections = MoveDirection.NE | MoveDirection.SE | MoveDirection.SW | MoveDirection.NW;
        public const MoveDirection QueenDirections = RookDirections | BishopDirections;
    }
}
