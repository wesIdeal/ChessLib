using MagicBitboard.Enums;
using System;

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
        public Bitboard()
        {

            InitializeSingleSquares();
            //InitializeRankAndFileAttacks();
            InitializeRookAttacks();
            InitializeKnightAttacks();
            InitializeBishopAttacks();
            InitializeQueenAttacks();
            InitializePawnAttacksAndMoves();
            InitializeKingAttacks();
        }

        private void InitializeKingAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                var square = IndividialSquares[rank, file];
                KingMoveMask[rank, file] = square.ShiftN() | square.ShiftNE() | square.ShiftE() | square.ShiftSE() | square.ShiftS() | square.ShiftSW() | square.ShiftW() | square.ShiftNW();
            }
        }

        private void InitializePawnAttacksAndMoves()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                var square = IndividialSquares[rank, file];
                PawnAttackMask[Color.White.ToInt(), rank, file] = square.ShiftNE() | square.ShiftNW();
            }
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                var square = IndividialSquares[rank, file];
                PawnAttackMask[Color.Black.ToInt(), rank, file] = square.ShiftSE() | square.ShiftSW();
            }
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                var square = IndividialSquares[rank, file];
                PawnMoveMask[Color.White.ToInt(), rank, file] = square.ShiftN() | (square.Shift2N() & MoveHelpers.RankMasks[Rank.R4.ToInt()]);
            }
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                var square = IndividialSquares[rank, file];
                PawnMoveMask[Color.Black.ToInt(), rank, file] = square.ShiftS() | (square.Shift2S() & MoveHelpers.RankMasks[Rank.R5.ToInt()]);
            }
        }

        private void InitializeQueenAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                QueenAttackMask[rank, file] = BishopAttackMask[rank, file] | RookAttackMask[rank, file];
            }
        }

        private void InitializeKnightAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                var start = IndividialSquares[rank, file];
                var str = Convert.ToString((long)start, 2).PadLeft(64, '0');
                var kAttack = (start.ShiftNNE() | start.ShiftNNW() | start.ShiftENE() | start.ShiftWNW() | start.ShiftESE() | start.ShiftSSE() | start.ShiftSSW() | start.ShiftWSW());

                //kAttack |= ();
                KnightAttackMask[rank, file] = kAttack;
            }
        }

        private void InitializeBishopAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                var start = IndividialSquares[rank, file];
                var str = Convert.ToString((long)start, 2).PadLeft(64, '0');

                var bAttack = (ulong)0;
                var value = (ulong)0;
                var current = start;
                while ((current = current.ShiftNE()) != 0)
                {
                    bAttack |= current;
                }
                current = start;
                while ((current = current.ShiftNW()) != 0)
                {
                    bAttack |= current;
                }
                current = start;
                while ((current = current.ShiftSE()) != 0)
                {
                    bAttack |= current;
                }
                current = start;
                while ((current = current.ShiftSW()) != 0)
                {
                    bAttack |= current;
                }

                //kAttack |= ();
                BishopAttackMask[rank, file] = bAttack;
            }
        }

        public void InitializeRookAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                RookAttackMask[rank, file] = (MoveHelpers.RankMasks[rank] | MoveHelpers.FileMasks[file]) ^ IndividialSquares[rank, file];
            }
        }
        public readonly ulong[,] IndividialSquares = new ulong[8, 8];
        public void InitializeSingleSquares()
        {
            for (int i = 0; i < 64; i++)
            {
                var rank = i.Rank().ToInt();
                var file = i.GetFile().ToInt();
                ulong rankStart = ((ulong)0x80) << (8 * rank);
                IndividialSquares[rank, file] = (ulong)(rankStart >> file);
            }
        }






    }
}
