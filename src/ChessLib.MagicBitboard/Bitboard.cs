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
        internal Pawn _pawn;
        private static IMovingPiece knight;
        internal Bishop Bishop;
        internal Rook Rook;
        private static IMovingPiece queen;
        private static IMovingPiece king;
            
        private Bitboard()
        {
            
            Rook = new Rook();
            _pawn = new Pawn();
            Bishop = new Bishop();
          
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

        public ulong GetMoves(ushort squareIndex, Piece bishop, Color color, ulong playerOccupancy,
            ulong opponentOccupancy)
        {
            switch (bishop)
            {
                case Piece.Pawn:
                    return _pawn.GetPsuedoLegalMoves(squareIndex, color, playerOccupancy, opponentOccupancy);
                case Piece.Knight:

                case Piece.Bishop:
                    return Bishop.GetPsuedoLegalMoves(squareIndex, color, playerOccupancy, opponentOccupancy);
                case Piece.Rook:
                    return Rook.GetPsuedoLegalMoves(squareIndex, color, playerOccupancy, opponentOccupancy);
                case Piece.Queen:

                case Piece.King:

                default:
                    throw new ArgumentOutOfRangeException(nameof(bishop), bishop, null);
            }
        }
    }
}