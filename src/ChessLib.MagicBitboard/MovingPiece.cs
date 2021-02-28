using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;

namespace ChessLib.MagicBitboard
{
    internal abstract class MovingPiece : IMovingPiece
    {
        public readonly ulong[] MagicKey = new ulong[64];
        protected ulong[] AttackMask;
        protected ulong[] MoveMask;
        protected MovingPieceService MovingPieceSvc;

        protected IEnumerable<ushort> AllSquares => Enumerable.Range(0, 64).Select(x=>(ushort)x);

        protected MovingPiece()
        {
            MovingPieceSvc = new MovingPieceService();
        }

        private ulong[] MovesAndAttacks
        {
            get
            {
                Debug.Assert(MoveMask != null && AttackMask != null, "Move and Attack masks need initialization.");
                Debug.Assert(MoveMask.Length == AttackMask.Length,
                    "Move and Attack masks should be of the same length.");
                Debug.Assert(MoveMask.Length == 64, "Move and Attack mask should be of size 64.");
                return MoveMask.Select((movesFromPosition, index) => movesFromPosition | AttackMask[index])
                    .ToArray();
            }
        }

        public abstract ulong GetPsuedoLegalMoves(ushort square, Color playerColor, ulong playerOccupancy,
            ulong opponentOccupancy);

        public abstract void Initialize();

        public ulong GetMovesFromSquare(ushort squareIndex)
        {
            return MoveMask[squareIndex];
        }

        //public ulong[][] MovesFromLocation = new ulong[64][]; private void InitializeMagicBitBoard()
        //{
        //    var movesAndAttacks = MovesAndAttacks;

        //    for (ushort index = 8; index < 56; index++)
        //    {
        //        var attackBoard = movesAndAttacks[index];
        //        ulong[] attackArray = new ulong[0];

        //        if (attackBoard != 0)
        //        {
        //            var setBitCount = 12;
        //            attackMask[index] = movesAndAttacks[index];
        //            var occupancyPermutations = MovingPieceSvc.GetAllBlockerPermutationsFromMoveMask(attackBoard);
        //            var permutations =
        //                MovingPieceSvc.GetAllPermutationsForAttackMask(index, moveMask[index], attackMask[index], occupancyPermutations)
        //                .ToArray();
        //            blockerBoards[index] = permutations;
        //            MagicKey[index] = GenerateMagicKey(blockerBoards[index], setBitCount,
        //                out attackArray);
        //        }
        //        MovesFromLocation[index] = attackArray;
        //    }
        //    //);

        //}


    }
}