using System;
using System.ComponentModel;

namespace MagicBitboard.Enums
{
    public enum Rank { R1 = 0, R2, R3, R4, R5, R6, R7, R8 };
    public enum File { A = 0, B, C, D, E, F, G, H };
    public enum Color { White = 0, Black }

    public enum FENPieces { PiecePlacement = 0, ActiveColor, CastlingRights, EnPassentSquare, HalfmoveClock, FullMoveNumber }

    [Flags]
    public enum MoveDirection
    {
        N = 1,
        NE = 2,
        E = 4,
        SE = 8,
        S = 16,
        SW = 32,
        W = 64,
        NW = 128
    }
    [Flags]
    public enum Check { NULL = 0, None = 1, Normal = 2, Opposite = 4, Double = 8 }

    [Flags]
    public enum CastlingAvailability
    {
        [Description("k")]
        BlackKingside = 1,
        [Description("q")]
        BlackQueenside = 2,
        [Description("K")]
        WhiteKingside = 4,
        [Description("Q")]
        WhiteQueenside = 8,
        [Description("-")]
        NoCastlingAvailable = 16
    }
    public static class SlidingPieceDirectionContants
    {
        public const MoveDirection RookDirections = MoveDirection.N | MoveDirection.E | MoveDirection.S | MoveDirection.W;
        public const MoveDirection BishopDirections = MoveDirection.NE | MoveDirection.SE | MoveDirection.SW | MoveDirection.NW;
        public const MoveDirection QueenDirections = RookDirections | BishopDirections;
    }
}
