using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ChessLib.MagicBitboard.MovingPieces
{
    internal class Pawn : MovingPiece
    {
        protected new ulong[][] moveMask;
        protected new ulong[][] attackMask;
        public Pawn()
        {
        }

        public override ulong GetPsuedoLegalMoves(ushort square, Color playerColor, ulong playerOccupancy, ulong opponentOccupancy)
        {
            var occupancy = playerOccupancy | opponentOccupancy;
            var empty = ~(occupancy);
            var attacks = attackMask[(int)playerColor][square] & opponentOccupancy;
            var moves = GetMovesFromOccupancy(square, playerColor, occupancy);
            var result = attacks | moves;
            return result;
        }

        private bool IsOpeningMove(ushort squareIndex, Color color)
        {
            var boardValue = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            var rankMask = color == Color.Black ? BoardConstants.Rank7 : BoardConstants.Rank2;
            return (boardValue & rankMask) != 0;
        }


        private ulong GetMovesFromOccupancy(ushort squareIndex, Color color, ulong totalOccupancy)
        {

            var empty = ~(totalOccupancy);
            var openMoves = moveMask[(int)color][squareIndex];
            var availableMoves = empty & openMoves;

            var dpRankMask = color == Color.Black ? BoardConstants.Rank5 : BoardConstants.Rank4;
            //if starting position and the only available move is to relative rank 4
            if (IsPawnInStartingPosition(squareIndex, color) && (dpRankMask & availableMoves) == availableMoves)
            {
                availableMoves = 0;
            }
            return availableMoves;
        }

        private static bool IsPawnInStartingPosition(ushort squareIndex, Color color)
        {
            var bv = MovingPieceService.GetBoardValueOfIndex(squareIndex);
            var secondRankMask = MovingPieceService.GetPawnStartRankMask(color);
            return (bv & secondRankMask) != 0;
        }

        public override void Initialize()
        {
            moveMask = new ulong[2][];
            moveMask[0] = new ulong[64];
            moveMask[1] = new ulong[64];

            attackMask = new ulong[2][];
            attackMask[0] = new ulong[64];
            attackMask[1] = new ulong[64];


            for (ushort square = 8; square < 56; square++)
            {
                ulong squareValue = MovingPieceService.GetBoardValueOfIndex(square);
                moveMask[(int)Color.White][square] = MovingPieceService.ShiftN(squareValue) | ((squareValue & BoardConstants.Rank2) << 16);
                attackMask[(int)Color.White][square] = MovingPieceService.ShiftNW(squareValue) | MovingPieceService.ShiftNE(squareValue);
            }
            for (ushort square = 55; square >= 8; square--)
            {
                ulong squareValue = MovingPieceService.GetBoardValueOfIndex(square);
                moveMask[(int)Color.Black][square] = MovingPieceService.ShiftS(squareValue) | ((squareValue & BoardConstants.Rank7) >> 16);
                attackMask[(int)Color.Black][square] = MovingPieceService.ShiftSW(squareValue) | MovingPieceService.ShiftSE(squareValue);
            }
        }



    }
}
