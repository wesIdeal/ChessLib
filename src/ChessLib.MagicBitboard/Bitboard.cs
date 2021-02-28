using System.Collections.Generic;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.MovingPieces;

namespace ChessLib.MagicBitboard
{
    public sealed class Bitboard
    {
        private static readonly List<string> lLock = new List<string>();
        private static Bitboard instance;
        private static IMovingPiece _pawn;
        private static IMovingPiece knight;
        private static Bishop _bishop;
        private static IMovingPiece rook;
        private static IMovingPiece queen;
        private static IMovingPiece king;

        private Bitboard()
        {
            _pawn = new Pawn();
            _bishop = new Bishop();
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


        public void ShowBlockBoardsForBishop(ushort idx)
        {
            _bishop.ShowBlockersFromSquare(idx);
        }

        public ulong GetMoves(ushort squareIndex, Color color, ulong playerOccupancy, ulong opponentOccupancy)
        {
            return _pawn.GetPsuedoLegalMoves(squareIndex, color, playerOccupancy, opponentOccupancy);
        }
    }
}