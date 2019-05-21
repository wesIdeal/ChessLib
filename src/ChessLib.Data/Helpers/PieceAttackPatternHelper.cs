using ChessLib.Data.PieceMobility;
using ChessLib.Types.Enums;

namespace ChessLib.Data.Helpers
{
    public static class PieceAttackPatternHelper
    {
        public static readonly Board BishopMoveMask = new Board();
        public static readonly Board RookMoveMask = new Board();
        public static readonly Board QueenMoveMask = new Board();
        public static readonly Board KnightAttackMask = new Board();
        public static readonly Board BishopAttackMask = new Board();
        public static readonly Board RookAttackMask = new Board();
        public static readonly Board QueenAttackMask = new Board();
        public static readonly Board KingMoveMask = new Board();
        public static readonly Board[] PawnAttackMask = new Board[2];
        public static readonly Board[] PawnMoveMask = new Board[2];

        static PieceAttackPatternHelper()
        {
            InitializeRookMovesAndAttacks();
            InitializeKnightAttacks();
            InitializeBishopMovesAndAttacks();
            InitializeQueenAttacks();
            InitializePawnAttacksAndMoves();
            InitializeKingAttacks();
        }


        private static void InitializeKingAttacks()
        {
            for (var i = 0; i < 64; i++)
            {
                var square = 1ul << i;
                KingMoveMask[i] = square.ShiftN() | square.ShiftNE() | square.ShiftE() | square.ShiftSE() |
                                           square.ShiftS() | square.ShiftSW() | square.ShiftW() | square.ShiftNW();
            }
        }

        private static void InitializePawnAttacksAndMoves()
        {
            PawnAttackMask[0] = new Board();
            PawnAttackMask[1] = new Board();
            PawnMoveMask[0] = new Board();
            PawnMoveMask[1] = new Board();
            for (var i = 8; i < 56; i++)
            {
                var square = 1ul << i;
                PawnAttackMask[Color.White.ToInt()][i] = square.ShiftNE() | square.ShiftNW();
                PawnMoveMask[Color.White.ToInt()][i] =
                    square.ShiftN() | (square.Shift2N() & BoardHelpers.RankMasks[Rank.R4.ToInt()]);
            }

            for (var i = 8; i < 56; i++)
            {
                var square = (ulong)1 << i;
                PawnAttackMask[Color.Black.ToInt()][i] = square.ShiftSE() | square.ShiftSW();
                PawnMoveMask[Color.Black.ToInt()][i] =
                    square.ShiftS() | (square.Shift2S() & BoardHelpers.RankMasks[Rank.R5.ToInt()]);
            }
        }

        private static void InitializeQueenAttacks()
        {
            for (var i = 0; i < 64; i++)
            {
                QueenAttackMask[i] = BishopAttackMask[i] | RookAttackMask[i];
                QueenMoveMask[i] = BishopMoveMask[i] | BishopMoveMask[i];
            }
        }

        private static void InitializeKnightAttacks()
        {
            for (var i = 0; i < 64; i++)
            {
                var index = (ulong)1 << i;
                var kAttack = index.ShiftNNE() | index.ShiftNNW() | index.ShiftENE() | index.ShiftWNW() |
                              index.ShiftESE() | index.ShiftSSE() | index.ShiftSSW() | index.ShiftWSW();

                //kAttack |= ();
                KnightAttackMask[i] = kAttack;
            }
        }

        private static void InitializeBishopMovesAndAttacks()
        {
            for (var i = 0; i < 64; i++)
            {
                var bishopAttackForSquare =
                    MoveInitializer.CalculateMovesFromPosition(i, 0, SlidingPieceDirectionConstants.BishopDirections,
                        true);
                var bishopMovesForSquare =
                    MoveInitializer.CalculateMovesFromPosition(i, 0, SlidingPieceDirectionConstants.BishopDirections);
                BishopAttackMask[i] = bishopAttackForSquare;
                BishopMoveMask[i] = bishopMovesForSquare;
            }
        }

        private static void InitializeRookMovesAndAttacks()
        {
            for (var i = 0; i < 64; i++)
            {
                var rookAttackForSquare =
                    MoveInitializer.CalculateMovesFromPosition(i, 0, SlidingPieceDirectionConstants.RookDirections,
                        true);
                var rookMovesForSquare =
                    MoveInitializer.CalculateMovesFromPosition(i, 0, SlidingPieceDirectionConstants.RookDirections,
                        true);
                RookAttackMask[i] = rookAttackForSquare;
                RookMoveMask[i] = rookMovesForSquare;
            }
        }
    }
}