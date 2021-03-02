using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ChessLib.Data.Magic.Init;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.MovingPieces;

[assembly: InternalsVisibleTo("ChessLib.MagicBitboard.Bitwise.Tests")]
namespace ChessLib.MagicBitboard
{

    public sealed class Bitboard
    {
        private static readonly List<string> lLock = new List<string>();
        private static Bitboard instance;
        internal Pawn Pawn;
        private static IMovingPiece knight;
        internal Bishop Bishop;
        internal Rook Rook;
        private static IMovingPiece queen;
        private static IMovingPiece king;

        private Bitboard()
        {
            Rook = new Rook();
            Pawn = new Pawn();
            Bishop = new Bishop();
            king = new King();
        }

        public static Bitboard Instance
        {
            get
            {
                lock (lLock)
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
                    return Pawn.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Knight:

                case Piece.Bishop:
                    return Bishop.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Rook:
                    return Rook.GetPseudoLegalMoves(squareIndex, color, occupancy);
                case Piece.Queen:
                    var rookMoves = Rook.GetPseudoLegalMoves(squareIndex, color, occupancy);
                    var bishopMoves = Bishop.GetPseudoLegalMoves(squareIndex, color, occupancy);
                    return rookMoves | bishopMoves;
                case Piece.King:
                    return king.GetPseudoLegalMoves(squareIndex, color, occupancy);
                default:
                    throw new ArgumentOutOfRangeException(nameof(bishop), bishop, null);
            }
        }
    }
}