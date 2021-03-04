using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.MovingPieces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ChessLib.MagicBitboard.Bitwise.Tests")]
namespace ChessLib.MagicBitboard
{

    public sealed class Bitboard
    {
        private static readonly List<string> LLock = new List<string>();
        private static Bitboard instance;
        private readonly Pawn _pawn;
        private readonly Knight _knight;
        internal readonly SlidingPiece Bishop;
        internal readonly SlidingPiece Rook;
        private readonly IMovingPiece _king;

        private Bitboard()
        {
            Rook = new Rook();
            Bishop = new Bishop();
            _pawn = new Pawn();
            _king = new King();
            _knight = new Knight();
        }

        public static Bitboard Instance
        {
            get
            {
                lock (LLock)
                {
                    if (instance == null)
                    {
                        instance = new Bitboard();
                    }

                    return instance;
                }
            }
        }

        public ulong GetPseudoLegalMoves(ushort squareIndex, Piece bishop, Color color, ulong occupancy)
        {
            switch (bishop)
            {
                case Piece.Pawn:
                    return _pawn.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Knight:
                    return _knight.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Bishop:
                    return Bishop.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Rook:
                    return Rook.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Queen:
                    var rookMoves = Rook.GetPseudoLegalMoves(squareIndex, color, occupancy);
                    var bishopMoves = Bishop.GetPseudoLegalMoves(squareIndex, color, occupancy);
                    return rookMoves | bishopMoves;
                case Piece.King:
                    return _king.GetPseudoLegalMoves(squareIndex, color, occupancy);
                default:
                    throw new ArgumentOutOfRangeException(nameof(bishop), bishop, null);
            }
        }
    }
}