using MagicBitboard.Enums;

namespace MagicBitboard.Helpers
{
    public static class PieceAttackPatternHelper
    {
        public static readonly ulong[,] BishopMoveMask = new ulong[8, 8];
        public static readonly ulong[,] RookMoveMask = new ulong[8, 8];
        public static readonly ulong[,] QueenMoveMask = new ulong[8, 8];
        public static readonly ulong[,] KnightAttackMask = new ulong[8, 8];
        public static readonly ulong[,] BishopAttackMask = new ulong[8, 8];
        public static readonly ulong[,] RookAttackMask = new ulong[8, 8];
        public static readonly ulong[,] QueenAttackMask = new ulong[8, 8];
        public static readonly ulong[,] KingMoveMask = new ulong[8, 8];
        public static readonly ulong[,] KingBlockMask = new ulong[8, 8];
        public static readonly ulong[,,] PawnAttackMask = new ulong[2, 8, 8];
        public static readonly ulong[,,] PawnMoveMask = new ulong[2, 8, 8];


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
            for (int i = 0; i < 64; i++)
            {
                var rank = i.GetRank().ToInt();
                var file = i.GetFile().ToInt();
                var square = MoveHelpers.IndividualSquares[rank, file];
                KingMoveMask[rank, file] = square.ShiftN() | square.ShiftNE() | square.ShiftE() | square.ShiftSE() | square.ShiftS() | square.ShiftSW() | square.ShiftW() | square.ShiftNW();
            }
        }

        private static void InitializePawnAttacksAndMoves()
        {
            for (int i = 8; i < 56; i++)
            {
                var square = (ulong)1 << i;
                PawnAttackMask[Color.White.ToInt(), i / 8, i % 8] = square.ShiftNE() | square.ShiftNW();
                PawnMoveMask[Color.White.ToInt(), i / 8, i % 8] = square.ShiftN() | (square.Shift2N() & MoveHelpers.RankMasks[Rank.R4.ToInt()]);
            }
            for (int i = 8; i < 56; i++)
            {
                var square = (ulong)1 << i;
                PawnAttackMask[Color.Black.ToInt(), i / 8, i % 8] = square.ShiftSE() | square.ShiftSW();
                PawnMoveMask[Color.Black.ToInt(), i / 8, i % 8] = square.ShiftS() | (square.Shift2S() & MoveHelpers.RankMasks[Rank.R5.ToInt()]);
            }
        }

        private static void InitializeQueenAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                QueenAttackMask[i / 8, i % 8] = BishopAttackMask[i / 8, i % 8] | RookAttackMask[i / 8, i % 8];
                QueenMoveMask[i / 8, i % 8] = BishopMoveMask[i / 8, i % 8] | BishopMoveMask[i / 8, i % 8];
            }
        }

        private static void InitializeKnightAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                ulong index = (ulong)1 << i;
                var kAttack = index.ShiftNNE() | index.ShiftNNW() | index.ShiftENE() | index.ShiftWNW() | index.ShiftESE() | index.ShiftSSE() | index.ShiftSSW() | index.ShiftWSW();

                //kAttack |= ();
                KnightAttackMask[(i / 8), (i % 8)] = kAttack;
            }
        }

        private static void InitializeBishopMovesAndAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var bishopAttackForSquare = MoveInitializer.CalculateMovesFromPosition(i, 0, SlidingPieceDirectionContants.BishopDirections, true);
                var bishopMovesForSquare = MoveInitializer.CalculateMovesFromPosition(i, 0, SlidingPieceDirectionContants.BishopDirections);
                BishopAttackMask[i / 8, i % 8] = bishopAttackForSquare;
                BishopMoveMask[i / 8, i % 8] = bishopMovesForSquare;
            }
        }

        private static void InitializeRookMovesAndAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rookAttackForSquare = MoveInitializer.CalculateMovesFromPosition(i, 0, SlidingPieceDirectionContants.RookDirections, true);
                var rookMovesForSquare = MoveInitializer.CalculateMovesFromPosition(i, 0, SlidingPieceDirectionContants.RookDirections, true);
                RookAttackMask[(i / 8), (i % 8)] = rookAttackForSquare;
                RookMoveMask[(i / 8), (i % 8)] = rookMovesForSquare;
            }
        }


    }
}
