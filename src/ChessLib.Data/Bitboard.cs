﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ChessLib.Data.Helpers;
using ChessLib.Data.PieceMobility;
using ChessLib.Data.Types;


namespace ChessLib.Data
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

        /// <summary>
        /// Gets both moves and attacks/captures for a piece
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="pieceSquare"></param>
        /// <param name="activeOcc"></param>
        /// <param name="oppOcc"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ulong GetPseudoLegalMoves(Piece piece, ushort pieceSquare, ulong activeOcc, ulong oppOcc, Color color, ushort? enPassantIndex = null, CastlingAvailability ca = CastlingAvailability.NoCastlingAvailable)
        {
            var totalOccupancy = activeOcc | oppOcc;
            ulong possibleMoves;
            switch (piece)
            {
                case Piece.Pawn:
                    var opponentOccupancy = oppOcc | (enPassantIndex ?? 0);
                    var pawnMoves = PieceAttackPatternHelper.PawnMoveMask[(int)color][pieceSquare] & ~(totalOccupancy);
                    var pawnAttacks = PieceAttackPatternHelper.PawnAttackMask[(int)color][pieceSquare] & opponentOccupancy;
                    possibleMoves = pawnMoves | pawnAttacks;
                    break;
                case Piece.Knight:
                    var totalAttacks = PieceAttackPatternHelper.KnightAttackMask[pieceSquare];
                    possibleMoves = totalAttacks & ~(activeOcc);
                    break;
                case Piece.Bishop:
                    possibleMoves = Bishop.GetLegalMoves(pieceSquare, totalOccupancy) & ~(activeOcc);
                    break;
                case Piece.Rook:
                    possibleMoves = Rook.GetLegalMoves(pieceSquare, totalOccupancy) & ~(activeOcc);
                    break;
                case Piece.Queen:
                    possibleMoves = (Bishop.GetLegalMoves(pieceSquare, totalOccupancy) | Rook.GetLegalMoves(pieceSquare, totalOccupancy)) & ~(activeOcc);
                    break;
                case Piece.King:

                    possibleMoves = PieceAttackPatternHelper.KingMoveMask[pieceSquare] & ~(activeOcc);
                    if (ca != CastlingAvailability.NoCastlingAvailable)
                    {
                        if (color == Color.Black)
                        {
                            if (ca.HasFlag(CastlingAvailability.BlackKingside))
                            {
                                possibleMoves |= (1ul << 62);
                            }
                            if (ca.HasFlag(CastlingAvailability.BlackQueenside))
                            {
                                possibleMoves |= (1ul << 58);
                            }
                        }
                        if (color == Color.White)
                        {
                            if (ca.HasFlag(CastlingAvailability.WhiteKingside))
                            {
                                possibleMoves |= (1ul << 6);
                            }
                            if (ca.HasFlag(CastlingAvailability.WhiteQueenside))
                            {
                                possibleMoves |= (1ul << 2);
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception("Piece argument passed to GetPossibleMoves()");
            }
            return possibleMoves;
        }

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

        /// <summary>
        /// Determines if piece on <paramref name="squareIndex"/> is attacked by <paramref name="color"/>
        /// </summary>
        /// <param name="squareIndex">Index of possible attack target</param>
        /// <param name="color">Color of attacker</param>
        /// <param name="piecesOnBoard">Occupancy arrays for both colors, indexed as [color_enum][piece_enum]</param>
        /// <returns>true if <paramref name="squareIndex"/> is attacked by any piece of <paramref name="color"/></returns>
        public static bool IsAttackedBy(this ushort squareIndex, Color color, ulong[][] piecesOnBoard)
        {

            var nColor = (int)color;
            var notNColor = nColor ^ 1;
            var r = squareIndex / 8;
            var f = squareIndex % 8;
            var totalOcc = 0ul;
            var oppositeOccupancy = piecesOnBoard[(int)color.Toggle()].Aggregate((x, y) => x |= y);
            var activeOccupancy = piecesOnBoard[(int)color].Aggregate((x, y) => x |= y);
            totalOcc = oppositeOccupancy | activeOccupancy;
            var bishopAttack = GetAttackedSquares(Piece.Bishop, squareIndex, totalOcc);
            var rookAttack = GetAttackedSquares(Piece.Rook, squareIndex, totalOcc);
            if ((PieceAttackPatternHelper.PawnAttackMask[notNColor][squareIndex] & piecesOnBoard[nColor][Piece.Pawn.ToInt()]) != 0) return true;
            if ((PieceAttackPatternHelper.KnightAttackMask[r, f] & piecesOnBoard[nColor][Piece.Knight.ToInt()]) != 0) return true;
            if ((bishopAttack & (piecesOnBoard[nColor][Piece.Bishop.ToInt()] | piecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((rookAttack & (piecesOnBoard[nColor][Piece.Rook.ToInt()] | piecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((PieceAttackPatternHelper.KingMoveMask[r, f] & piecesOnBoard[nColor][Piece.King.ToInt()]) != 0) return true;
            return false;
        }
    }
}