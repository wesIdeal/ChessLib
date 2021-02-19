using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;
using System;
using System.Diagnostics;
using System.Linq;

namespace ChessLib.MagicBitboard
{
    abstract class MovingPiece : IMovingPiece
    {
        protected ulong[] moveMask;
        protected ulong[] attackMask;
        protected MovingPieceService MovingPieceSvc;
        protected MoveObstructionBoard[][] blockerBoards = new MoveObstructionBoard[64][];
        public readonly ulong[] MagicKey = new ulong[64];

        protected MovingPiece()
        {
            MovingPieceSvc = new MovingPieceService();
            Initialize();
        }

        private ulong[] MovesAndAttacks
        {
            get
            {
                Debug.Assert(moveMask != null && attackMask != null, "Move and Attack masks need initialization.");
                Debug.Assert(moveMask.Length == attackMask.Length, "Move and Attack masks should be of the same length.");
                Debug.Assert(moveMask.Length == 64, "Move and Attack mask should be of size 64.");
                return moveMask.Select((movesFromPosition, index) => movesFromPosition | attackMask[index])
                               .ToArray();
            }
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
        //            var occupancyPermutations = MovingPieceSvc.GetAllPermutations(attackBoard);
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

        /// <summary>
        /// Generates magic multiplier to retrieve moves for a given piece on a square
        /// </summary>
        /// <param name="blockerAndMoveBoards">The object containing occupancy and resultant moves</param>
        /// <param name="countOfSetBits">The number of set bits in the move mask. Used to determine the array size.</param>
        /// <param name="attackArray">The newly ordered array of attacks, based on the calculated magic number</param>
        /// <returns>The magic key which was found</returns>
        protected ulong GenerateMagicKey(MoveObstructionBoard[] blockerAndMoveBoards, int countOfSetBits, out ulong[] attackArray)
        {
            var maxMoves = 1 << countOfSetBits;
            ulong[] emptyArray = new ulong[maxMoves];
            attackArray = new ulong[maxMoves];
            Array.Copy(emptyArray, attackArray, maxMoves);

            var key = (ulong)0;
            var foundCollision = true;
            while (foundCollision)
            {
                key = GetRandomKey();
                foundCollision = false;
                attackArray = new ulong[maxMoves];
                foreach (var pattern in blockerAndMoveBoards)
                {
                    var hash = (pattern.Occupancy * key) >> (64 - countOfSetBits);
                    if (attackArray[hash] != 0 && attackArray[hash] != pattern.MoveBoard)
                    {
                        foundCollision = true;
                        break;
                    }

                    attackArray[hash] = pattern.MoveBoard;
                }
            }
            return key;
        }
        #region Random Number Helpers
        private readonly Random _random = new Random();

        public ulong NextRandom()
        {
            var leftPart = (ulong)_random.Next() << 32;
            var rightPart = (ulong)_random.Next();
            return leftPart | rightPart;
        }

        protected ulong GetRandomKey()
        {
            return NextRandom() & NextRandom() & NextRandom();
        }
        #endregion
        public abstract ulong GetPsuedoLegalMoves(ushort square, Color playerColor, ulong playerOccupancy, ulong opponentOccupancy);
        public abstract void Initialize();
    }
}