

using ChessLib.Data.MoveInitializers;
using ChessLib.Data.Types;

namespace ChessLib.Data.Helpers
{
    public class Board
    {
        public ulong[] MoveBoard = new ulong[64];

        public Board()
        {
            MoveBoard = new ulong[64];
        }

        public Board(ulong[] moveBoard)
        {
            MoveBoard = moveBoard;
        }

        public Board(ulong[,] moveBoard) : this()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    this[rank, file] = moveBoard[rank, file];
                }
            }
        }

        public ulong this[int k]
        {
            get => MoveBoard[k];
            set => MoveBoard[k] = value;
        }

        public ulong this[int rank, int file]
        {
            get => MoveBoard[(rank * 8) + file];
            set => MoveBoard[(rank * 8) + file] = value;
        }

    }
    public static class PieceAttackPatternHelper
    {
        public static readonly Board BishopMoveMask = new Board();
        public static readonly Board RookMoveMask = new Board();
        public static readonly ulong[,] QueenMoveMask = new ulong[8, 8];
        public static readonly Board KnightAttackMask = new Board();
        public static readonly ulong[,] BishopAttackMask = new ulong[8, 8];
        public static readonly ulong[,] RookAttackMask = new ulong[8, 8];
        public static readonly ulong[,] QueenAttackMask = new ulong[8, 8];
        public static readonly ulong[,] KingMoveMask = new ulong[8, 8];
        public static readonly ulong[,] KingBlockMask = new ulong[8, 8];
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
            for (int i = 0; i < 64; i++)
            {
                var rank = (i / 8);
                var file = (i % 8);
                var square = BoardHelpers.IndividualSquares[rank, file];
                KingMoveMask[rank, file] = square.ShiftN() | square.ShiftNE() | square.ShiftE() | square.ShiftSE() | square.ShiftS() | square.ShiftSW() | square.ShiftW() | square.ShiftNW();
            }
        }

        private static void InitializePawnAttacksAndMoves()
        {
            PawnAttackMask[0] = new Board();
            PawnAttackMask[1] = new Board();
            PawnMoveMask[0] = new Board();
            PawnMoveMask[1] = new Board();
            for (int i = 8; i < 56; i++)
            {
                var square = (ulong)1 << i;
                PawnAttackMask[Color.White.ToInt()][i] = square.ShiftNE() | square.ShiftNW();
                PawnMoveMask[Color.White.ToInt()][i] = square.ShiftN() | (square.Shift2N() & BoardHelpers.RankMasks[Rank.R4.ToInt()]);
            }
            for (int i = 8; i < 56; i++)
            {
                var square = (ulong)1 << i;
                PawnAttackMask[Color.Black.ToInt()][i] = square.ShiftSE() | square.ShiftSW();
                PawnMoveMask[Color.Black.ToInt()][i] = square.ShiftS() | (square.Shift2S() & BoardHelpers.RankMasks[Rank.R5.ToInt()]);
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
