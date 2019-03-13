using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using MagicBitboard.SlidingPieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MagicBitboard
{
    public class Bitboard
    {
       
        public readonly MovePatternStorage Bishop;
        public readonly MovePatternStorage Rook;
        
        public Bitboard()
        {
            Bishop = new MovePatternStorage();
            Bishop.Initialize(PieceAttackPatternHelper.BishopAttackMask, new BishopMovesInitializer());
            Rook = new MovePatternStorage();
            Rook.Initialize(PieceAttackPatternHelper.RookAttackMask, new RookMovesInitializer());
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int Rank(ushort idx) => idx / 8;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int File(ushort idx) => idx % 8;

        public ulong GetAttackedSquares(Piece piece, ushort pieceIndex, ulong occupancy, Color color = Color.White)
        {
            var r = Rank(pieceIndex);
            var f = File(pieceIndex);
            var bishopSquares = Bishop.GetLegalMoves(pieceIndex, occupancy);
            var rookSquares = Rook.GetLegalMoves(pieceIndex, occupancy);
            switch (piece)
            {
                case Piece.Bishop:
                    return bishopSquares;
                case Piece.Rook:
                    return rookSquares;
                case Piece.Queen:
                    return bishopSquares | rookSquares;
                case Piece.Pawn:
                    return PieceAttackPatternHelper.PawnAttackMask[color.ToInt(), r, f];
                case Piece.King:
                    return PieceAttackPatternHelper.KingMoveMask[r, f];
                case Piece.Knight:
                    return PieceAttackPatternHelper.KnightAttackMask[r, f];
                default:
                    throw new Exception("Piece not supported for GetAttackSquares().");
            }
        }

        public ulong[] RookOccupancyBoards(ushort index) => Rook.OccupancyAndMoveBoards[index].Select(x => x.Occupancy).ToArray();
        public ulong[] BishopOccupancyBoards(ushort index) => Bishop.OccupancyAndMoveBoards[index].Select(x => x.Occupancy).ToArray();
    }
}
