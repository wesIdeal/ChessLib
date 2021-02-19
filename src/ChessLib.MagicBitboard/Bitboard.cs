using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.MovingPieces;
using System;
using System.Collections.Generic;

namespace ChessLib.MagicBitboard
{
    public sealed class Bitboard
    {
        private static List<string> lLock = new List<string>();
        private static Bitboard instance = null;
        private static IMovingPiece pawn;
        private static IMovingPiece knight;
        private static IMovingPiece bishop;
        private static IMovingPiece rook;
        private static IMovingPiece queen;
        private static IMovingPiece king;


        private Bitboard()
        {
            pawn = new Pawn();
        }

        public ulong GetMoves(ushort squareIndex, Data.Types.Enums.Color color, ulong playerOccupancy, ulong opponentOccupancy)
        {
            return pawn.GetPsuedoLegalMoves(squareIndex, color, playerOccupancy, opponentOccupancy);
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
    }
}
