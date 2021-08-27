using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.MagicBitboard.MovingPieces
{
    internal sealed class Pawn : MovingPiece
    {
        public Pawn()
        {
            Initialize();
        }

        internal new ulong[][] AttackMask;
        internal new ulong[][] MoveMask;

        public ulong GetAttacksFromSquare(ushort squareIndex, Color color)
        {
            return AttackMask[(int)color][squareIndex];
        }

        public ulong GetMovesFromSquare(ushort squareIndex, Color color)
        {
            return MoveMask[(int)color][squareIndex];
        }

        public override ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy)
        {
            var attacks = AttackMask[(int)playerColor][square] & occupancy;
            var moves = GetMovesFromOccupancy(square, playerColor, occupancy);
            var result = attacks | moves;
            return result;
        }


        private ulong GetMovesFromOccupancy(ushort squareIndex, Color color, ulong totalOccupancy)
        {
            var empty = ~totalOccupancy;
            var openMoves = MoveMask[(int)color][squareIndex];
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

        public void Initialize()
        {
            InitializeMasks();
            InitializeWhitePawnMovesAndAttacks();
            InitializeBlackPawnMovesAndAttacks();
        }

        private void InitializeBlackPawnMovesAndAttacks()
        {
            foreach (var square in BoardConstants.AllSquares)
            {
                var squareValue = MovingPieceService.GetBoardValueOfIndex(square);
                MoveMask[(int)Color.Black][square] =
                    MovingPieceService.ShiftS(squareValue) | ((squareValue & BoardConstants.Rank7) >> 16);
                AttackMask[(int)Color.Black][square] =
                    MovingPieceService.ShiftSW(squareValue) | MovingPieceService.ShiftSE(squareValue);
            }
        }

        private void InitializeWhitePawnMovesAndAttacks()
        {
            for (ushort square = 0; square < 64; square++)
            {
                var squareValue = MovingPieceService.GetBoardValueOfIndex(square);
                MoveMask[(int)Color.White][square] =
                    MovingPieceService.ShiftN(squareValue) | ((squareValue & BoardConstants.Rank2) << 16);
                AttackMask[(int)Color.White][square] =
                    MovingPieceService.ShiftNW(squareValue) | MovingPieceService.ShiftNE(squareValue);
            }
        }

        private void InitializeMasks()
        {
            MoveMask = new ulong[2][];
            MoveMask[0] = new ulong[64];
            MoveMask[1] = new ulong[64];

            AttackMask = new ulong[2][];
            AttackMask[0] = new ulong[64];
            AttackMask[1] = new ulong[64];
        }
    }
}