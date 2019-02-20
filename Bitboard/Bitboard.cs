using MagicBitboard.Enums;
using System;
using System.Collections.Generic;

namespace MagicBitboard
{
    public class Bitboard
    {

        public readonly ulong[,] KnightAttackMask = new ulong[8, 8];
        public readonly ulong[,] BishopAttackMask = new ulong[8, 8];
        public readonly ulong[,] RookAttackMask = new ulong[8, 8];
        public readonly ulong[,] QueenAttackMask = new ulong[8, 8];
        public readonly ulong[,,] PawnAttackMask = new ulong[2, 8, 8];
        public readonly ulong[,,] PawnMoveMask = new ulong[2, 8, 8];
        public readonly ulong[,] KingMoveMask = new ulong[8, 8];
        //public readonly ulong[,] KnightBlockMask = new ulong[8, 8];
        //public readonly ulong[,] BishopBlockMask = new ulong[8, 8];
        //public readonly ulong[,] RookBlockMask = new ulong[8, 8];
        //public readonly ulong[,] QueenBlockMask = new ulong[8, 8];
        //public readonly ulong[,,] PawnBlockMask = new ulong[2, 8, 8];


        public readonly ulong[,] KingBlockMask = new ulong[8, 8];

        public Bitboard()
        {
            //InitializeRankAndFileAttacks();
            var tOld = DateTime.Now;

            var trial = MoveHelpers.GenerateSlidingPieceOccupancyBoards(out RookAttackMask, out BishopAttackMask);
            InitializeRookAttacks();
            InitializeKnightAttacks();
            //InitializeBishopAttacks();
            InitializeQueenAttacks();
            InitializePawnAttacksAndMoves();
            InitializeKingAttacks();
            InitializeBlockMasks();
        }

        private void InitializeBlockMasks()
        {

        }

        private void InitializeKingAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.GetRank().ToInt();
                var file = i.GetFile().ToInt();
                var square = MoveHelpers.IndividualSquares[rank, file];
                KingMoveMask[rank, file] = square.ShiftN() | square.ShiftNE() | square.ShiftE() | square.ShiftSE() | square.ShiftS() | square.ShiftSW() | square.ShiftW() | square.ShiftNW();
            }
        }

        private void InitializePawnAttacksAndMoves()
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

        private void InitializeQueenAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.GetRank().ToInt();
                var file = i.GetFile().ToInt();
                QueenAttackMask[rank, file] = BishopAttackMask[rank, file] | RookAttackMask[rank, file];
            }
        }

        private void InitializeKnightAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                ulong index = (ulong)1 << i;
                var kAttack = index.ShiftNNE() | index.ShiftNNW() | index.ShiftENE() | index.ShiftWNW() | index.ShiftESE() | index.ShiftSSE() | index.ShiftSSW() | index.ShiftWSW();

                //kAttack |= ();
                KnightAttackMask[(i / 8), (i % 8)] = kAttack;
            }
        }

        private void InitializeBishopAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.GetRank().ToInt();
                var file = i.GetFile().ToInt();
                var start = (ulong)1 << i;
                var str = Convert.ToString((long)start, 2).PadLeft(64, '0');

                var bAttack = (ulong)0;
                var current = start;

                //NE
                for (var sq = i; (sq / 8) < 8 && (sq % 8) > 0; sq += 6) bAttack |= ((ulong)1 << sq);
                //SE
                for (var sq = i; (sq / 8) > 0 && (sq % 8) > 0; sq -= 6) bAttack |= ((ulong)1 << sq);


                //kAttack |= ();
                BishopAttackMask[rank, file] = bAttack;
            }
        }

        public void InitializeRookAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rv = (ulong)0;
                var startingValue = (ulong)1 << i;
                //N
                var positionalValue = startingValue;
                while ((i / 8) != 7 && ((positionalValue = positionalValue.ShiftN()) & MoveHelpers.RankMasks[7]) == 0)
                {
                    rv |= positionalValue;
                }
                //E
                positionalValue = startingValue;
                while ((i % 8) != 7 && ((positionalValue = positionalValue.ShiftE()) & MoveHelpers.FileMasks[7]) == 0)
                {
                    rv |= positionalValue;
                }
                //S
                positionalValue = startingValue;
                while ((i/8) != 0 && ((positionalValue = positionalValue.ShiftS()) & MoveHelpers.RankMasks[0]) == 0)
                {
                    rv |= positionalValue;
                }
                //W
                positionalValue = startingValue;
                while ((i%8) != 0 &&((positionalValue = positionalValue.ShiftW()) & MoveHelpers.FileMasks[0]) == 0)
                {
                    rv |= positionalValue;

                }
                RookAttackMask[(i / 8), (i % 8)] = rv;
            }
        }


    }
}
