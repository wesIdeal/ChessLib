using ChessLib.Data.Helpers;
using ChessLib.Data.MoveInitializers;
using ChessLib.Data.Types;
using MagicBitboard.Helpers;
using MagicBitboard.SlidingPieces;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MagicBitboard
{
    public static class Bitboard
    {

        public static readonly MovePatternStorage Bishop = new MovePatternStorage();
        public static readonly MovePatternStorage Rook = new MovePatternStorage();

        static Bitboard()
        {
            Bishop.Initialize(PieceAttackPatternHelper.BishopAttackMask, new BishopMovesInitializer());
            Rook.Initialize(PieceAttackPatternHelper.RookAttackMask, new RookMovesInitializer());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Rank(ushort idx) => idx / 8;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int File(ushort idx) => idx % 8;

        public static ulong GetAttackedSquares(Piece piece, ushort pieceIndex, ulong occupancy, Color color = Color.White)
        {
            var r = Rank(pieceIndex);
            var f = File(pieceIndex);
            switch (piece)
            {
                case Piece.Bishop:
                    return Bishop.GetLegalMoves(pieceIndex, occupancy);
                case Piece.Rook:
                    return Rook.GetLegalMoves(pieceIndex, occupancy);
                case Piece.Queen:
                    return Bishop.GetLegalMoves(pieceIndex, occupancy) | Rook.GetLegalMoves(pieceIndex, occupancy);
                case Piece.Pawn:
                    return PieceAttackPatternHelper.PawnAttackMask[color.ToInt()][pieceIndex];
                case Piece.King:
                    return PieceAttackPatternHelper.KingMoveMask[r, f];
                case Piece.Knight:
                    return PieceAttackPatternHelper.KnightAttackMask[r, f];
                default:
                    throw new Exception("Piece not supported for GetAttackSquares().");
            }
        }

        public static ulong[] RookOccupancyBoards(ushort index) => Rook.OccupancyAndMoveBoards[index].Select(x => x.Occupancy).ToArray();
        public static ulong[] BishopOccupancyBoards(ushort index) => Bishop.OccupancyAndMoveBoards[index].Select(x => x.Occupancy).ToArray();
    }
}
