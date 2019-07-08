using ChessLib.Data.Helpers;
using ChessLib.Data.PieceMobility;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChessLib.Data.Types.Enums;

[assembly: InternalsVisibleTo("ChessLib.Data.Tests")]
namespace ChessLib.Data.Magic.Init
{
    /// <summary>
    /// Holds Move and Attack masks for pieces and pawns
    /// </summary>
    internal class PieceAttackPatterns
    {
        public readonly MoveBoard BishopMoveMask = new MoveBoard();
        public readonly MoveBoard RookMoveMask = new MoveBoard();
        public readonly MoveBoard QueenMoveMask = new MoveBoard();
        public readonly MoveBoard KnightAttackMask = new MoveBoard();
        public readonly MoveBoard BishopAttackMask = new MoveBoard();
        public readonly MoveBoard RookAttackMask = new MoveBoard();
        public readonly MoveBoard QueenAttackMask = new MoveBoard();
        public readonly MoveBoard KingMoveMask = new MoveBoard();
        public readonly MoveBoard[] PawnAttackMask = new MoveBoard[2];
        public readonly MoveBoard[] PawnMoveMask = new MoveBoard[2];

        private static readonly Lazy<PieceAttackPatterns> _instance = new Lazy<PieceAttackPatterns>(() => new PieceAttackPatterns());
        public static PieceAttackPatterns Instance => _instance.Value;

        private PieceAttackPatterns()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ParallelInitializePieces();
            sw.Stop();
            Debug.Write($"All Magic Bitboards generated in {sw.ElapsedMilliseconds} ms, parallel.");
        }

        //private void InitializePieces()
        //{
        //    InitializeRookMovesAndAttacks();
        //    InitializeKnightAttacks();
        //    InitializeBishopMovesAndAttacks();
        //    InitializeQueenAttacks();
        //    InitializePawnAttacksAndMoves();
        //    InitializeKingAttacks();
        //}

        private void ParallelInitializePieces()
        {
            Parallel.Invoke(
            InitializeRookMovesAndAttacks,
            InitializeKnightAttacks,
            InitializeBishopMovesAndAttacks,
            InitializeQueenAttacks,
            InitializePawnAttacksAndMoves,
            InitializeKingAttacks
            );
            for(int i = 0; i < 64; i++)
            {
                Debug.WriteLine($"[TestCase({i}," +
                    $"new ulong[]{{ {KingMoveMask[i]}ul, {QueenMoveMask[i]}ul, {RookMoveMask[i]}ul, {BishopMoveMask[i]}ul, 0ul/*KnightMoveMask is same as attack mask*/, {PawnMoveMask[0][i]}ul, {PawnMoveMask[1][i]}ul}}, \"Piece on {i.IndexToSquareDisplay()}\")]");
            }
            for (int i = 0; i < 64; i++)
            {
                Debug.WriteLine($"[TestCase({i}," +
                    $"new ulong[]{{ 0ul/*King moves are same as attacks*/, {QueenAttackMask[i]}ul, {RookAttackMask[i]}ul, {BishopAttackMask[i]}ul, {KnightAttackMask[i]}ul, {PawnAttackMask[0][i]}ul, {PawnAttackMask[1][i]}ul}}, \"Piece on {i.IndexToSquareDisplay()}\")]");
            }
        }

        private void InitializeKingAttacks()
        {
            for (var i = 0; i < 64; i++)
            {
                var square = 1ul << i;
                KingMoveMask[i] = square.ShiftN() | square.ShiftNE() | square.ShiftE() | square.ShiftSE() |
                                           square.ShiftS() | square.ShiftSW() | square.ShiftW() | square.ShiftNW();
            }
        }

        private void InitializePawnAttacksAndMoves()
        {
            PawnAttackMask[0] = new MoveBoard();
            PawnAttackMask[1] = new MoveBoard();
            PawnMoveMask[0] = new MoveBoard();
            PawnMoveMask[1] = new MoveBoard();
            for (var i = 8; i < 56; i++)
            {
                var square = 1ul << i;
                PawnAttackMask[Color.White.ToInt()][i] = square.ShiftNE() | square.ShiftNW();
                PawnMoveMask[Color.White.ToInt()][i] =
                    square.ShiftN() | square.Shift2N() & BoardHelpers.RankMasks[Rank.R4.ToInt()];
            }

            for (var i = 8; i < 56; i++)
            {
                var square = (ulong)1 << i;
                PawnAttackMask[Color.Black.ToInt()][i] = square.ShiftSE() | square.ShiftSW();
                PawnMoveMask[Color.Black.ToInt()][i] =
                    square.ShiftS() | square.Shift2S() & BoardHelpers.RankMasks[Rank.R5.ToInt()];
            }
        }

        private void InitializeQueenAttacks()
        {
            for (var i = 0; i < 64; i++)
            {
                QueenAttackMask[i] = BishopAttackMask[i] | RookAttackMask[i];
                QueenMoveMask[i] = BishopMoveMask[i] | BishopMoveMask[i];
            }
        }

        private void InitializeKnightAttacks()
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

        private void InitializeBishopMovesAndAttacks()
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

        private void InitializeRookMovesAndAttacks()
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