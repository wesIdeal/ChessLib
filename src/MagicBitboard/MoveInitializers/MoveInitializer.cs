using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagicBitboard
{
    public abstract class MoveInitializer : IMoveInitializer
    {

        private readonly MoveDirection _moveDirectionFlags;

        protected MoveInitializer(MoveDirection moveDirectionFlags)
        {
            _moveDirectionFlags = moveDirectionFlags;
        }

        #region Random Number Helpers
        private Random _random = new Random();

        public ulong NextRandom()
        {
            var leftPart = (ulong)_random.Next() << 32;
            var rightPart = (ulong)_random.Next();

            return leftPart | rightPart;
        }

        private ulong GetRandomKey()
        {
            return NextRandom() & NextRandom() & NextRandom();
        }
        #endregion

        /// <summary>
        /// Gets the permutations of Occupancy/Move boards from a given position
        /// </summary>
        /// <param name="pieceLocationIndex">The index of the piece</param>
        /// <param name="attackMask">The piece's associated attack mask from the position index</param>
        /// <param name="occupancyBoards">The associated occupancy boards</param>
        /// <returns></returns>
        public IEnumerable<BlockerAndMoveBoards> GetAllPermutationsForAttackMask(int pieceLocationIndex, ulong attackMask, IEnumerable<ulong> occupancyBoards)
        {
            var boardCombos = new List<BlockerAndMoveBoards>();
            var dtStart = DateTime.Now;
            var totalBoards = occupancyBoards.Count();
            foreach (var board in occupancyBoards)
            {
                //Debug.Write(string.Format("\r{0,4} | {1,-4}", count++, totalBoards));
                boardCombos.Add(new BlockerAndMoveBoards(board, CalculateMovesFromPosition(pieceLocationIndex, board)));
            }
            return boardCombos;
        }

        /// <summary>
        /// Generates magic multiplier to retrieve moves for a given piece on a square
        /// </summary>
        /// <param name="blockerAndMoveBoards">The object containing occupancy and resultant moves</param>
        /// <param name="countOfSetBits">The number of set bits in the move mask. Used to determine the array size.</param>
        /// <param name="attackArray">The newly ordered array of attacks, based on the calculated magic number</param>
        /// <returns>The magic key which was found</returns>
        public ulong GenerateMagicKey(BlockerAndMoveBoards[] blockerAndMoveBoards, int countOfSetBits, out ulong[] attackArray)
        {
            var maxMoves = 1 << countOfSetBits;
            ulong[] emptyArray = new ulong[maxMoves];
            attackArray = new ulong[maxMoves];
            Array.Copy(emptyArray, attackArray, maxMoves);

            var key = (ulong)0;
            var fail = true;
            var dtStart = DateTime.Now;
            var count = 1;
            while (fail)
            {
                key = GetRandomKey();
                fail = false;

                //Array.Clear(attackArray, 0, maxMoves);
                attackArray = new ulong[maxMoves];
                foreach (var pattern in blockerAndMoveBoards)
                {
                    var hash = (pattern.Occupancy * key) >> (64 - countOfSetBits);
                    if (attackArray[hash] != 0 && attackArray[hash] != pattern.MoveBoard)
                    {
                        fail = true;
                        count++;
                        break;
                    }

                    attackArray[hash] = pattern.MoveBoard;
                }
            }
            var totalMs = DateTime.Now.Subtract(dtStart).TotalMilliseconds;

            Console.WriteLine($"Finished MagicNumber calcs in {totalMs} ms.");
            return key;
        }

        /// <summary>
        /// Gets a board representing the squares a piece on a square can move to.
        /// </summary>
        /// <param name="positionIndex">The board index position of the piece</param>
        /// <param name="occupancyBoard">A bitboard representation of occupied squares</param>
        /// <param name="moveDirectionFlags">The directions in which the piece can move</param>
        /// <param name="attackArrayGen">When true, excludes outer board edges.</param>
        /// <returns>A bitboard representation of legal moves from given position</returns>
        public static ulong CalculateMovesFromPosition(int positionIndex, ulong occupancyBoard, MoveDirection moveDirectionFlags, bool attackArrayGen = false)
        {
            var rv = (ulong)0;
            const ulong AllSquares = ulong.MaxValue;
            var startingValue = (ulong)1 << positionIndex;
            var positionalValue = startingValue;

            //N
            if (moveDirectionFlags.HasFlag(MoveDirection.N))
            {

                while ((positionalValue = positionalValue.ShiftN()) != 0)
                {
                    rv |= positionalValue & (attackArrayGen ? ~BoardHelpers.RankMasks[7] : AllSquares);
                    if ((occupancyBoard & positionalValue) == positionalValue) break;
                }


            }

            //E
            if (moveDirectionFlags.HasFlag(MoveDirection.E))
            {
                positionalValue = startingValue;
                while ((positionalValue = positionalValue.ShiftE()) != 0)
                {
                    rv |= positionalValue & (attackArrayGen ? ~BoardHelpers.FileMasks[7] : AllSquares);
                    if ((occupancyBoard & positionalValue) == positionalValue) break;
                }
            }

            //S
            if (moveDirectionFlags.HasFlag(MoveDirection.S))
            {
                positionalValue = startingValue;
                while ((positionalValue = positionalValue.ShiftS()) != 0)
                {
                    rv |= positionalValue & (attackArrayGen ? ~BoardHelpers.RankMasks[0] : AllSquares);
                    if ((occupancyBoard & positionalValue) == positionalValue) break;
                }
            }

            //W
            if (moveDirectionFlags.HasFlag(MoveDirection.W))
            {
                positionalValue = startingValue;
                while ((positionalValue = positionalValue.ShiftW()) != 0)
                {
                    rv |= positionalValue & (attackArrayGen ? ~BoardHelpers.FileMasks[0] : AllSquares);
                    if ((occupancyBoard & positionalValue) == positionalValue) break;
                }
            }

            //NE
            if (moveDirectionFlags.HasFlag(MoveDirection.NE))
            {
                positionalValue = startingValue;
                while ((positionalValue = positionalValue.ShiftNE()) != 0)
                {
                    rv |= positionalValue & (attackArrayGen ? (~BoardHelpers.FileMasks[7] & ~BoardHelpers.RankMasks[7]) : AllSquares);
                    if ((occupancyBoard & positionalValue) == positionalValue) break;
                }
            }

            //NW
            if (moveDirectionFlags.HasFlag(MoveDirection.NW))
            {
                positionalValue = startingValue;
                while ((positionalValue = positionalValue.ShiftNW()) != 0)
                {
                    rv |= positionalValue & (attackArrayGen ? (~BoardHelpers.FileMasks[0] & ~BoardHelpers.RankMasks[7]) : AllSquares);
                    if ((occupancyBoard & positionalValue) == positionalValue) break;
                }
            }

            //SE
            if (moveDirectionFlags.HasFlag(MoveDirection.SE))
            {
                positionalValue = startingValue;
                while ((positionalValue = positionalValue.ShiftSE()) != 0)
                {
                    rv |= positionalValue & (attackArrayGen ? (~BoardHelpers.FileMasks[7] & ~BoardHelpers.RankMasks[0]) : AllSquares);
                    if ((occupancyBoard & positionalValue) == positionalValue) break;
                }
            }

            //SW
            if (moveDirectionFlags.HasFlag(MoveDirection.SW))
            {
                positionalValue = startingValue;
                while ((positionalValue = positionalValue.ShiftSW()) != 0)
                {
                    rv |= positionalValue & (attackArrayGen ? (~BoardHelpers.FileMasks[0] & ~BoardHelpers.RankMasks[0]) : AllSquares);
                    if ((occupancyBoard & positionalValue) == positionalValue) break;
                }
            }
            return rv;
        }

        /// <summary>
        /// Gets the permutations of blockers for a given attack mask. 
        /// </summary>
        /// <param name="mask">The relevant attack mask</param>
        /// <returns>All relevant occupancy boards for the given mask</returns>
        public static IEnumerable<ulong> GetAllPermutations(ulong mask)
        {
            var setBitIndices = BitHelpers.GetSetBits(mask);
            return GetAllPermutations(setBitIndices, 0, 0).Distinct();
        }

        private static IEnumerable<ulong> GetAllPermutations(int[] SetBits, int Index, ulong Value)
        {
            BitHelpers.SetBit(ref Value, SetBits[Index]);
            yield return Value;
            int index = Index + 1;
            if (index < SetBits.Count())
            {
                using (IEnumerator<ulong> occupancyPermutations = GetAllPermutations(SetBits, index, Value).GetEnumerator())
                {
                    while (occupancyPermutations.MoveNext())
                    {
                        yield return occupancyPermutations.Current;
                    }
                }
            }
            BitHelpers.ClearBit(ref Value, SetBits[Index]);
            yield return Value;
            if (index < SetBits.Count())
            {
                using (IEnumerator<ulong> occupancyPermutations = GetAllPermutations(SetBits, index, Value).GetEnumerator())
                {
                    while (occupancyPermutations.MoveNext())
                    {
                        yield return occupancyPermutations.Current;
                    }
                }
            }

        }

        /// <summary>
        /// Method used to call the static method with member directions.
        /// </summary>
        /// <param name="positionIndex">The board index position of the piece</param>
        /// <param name="occupancyBoard">A bitboard representation of occupied squares</param>
        /// <returns>A bitboard representation of legal moves from given position</returns>        
        private ulong CalculateMovesFromPosition(int positionIndex, ulong occupancyBoard) => CalculateMovesFromPosition(positionIndex, occupancyBoard, _moveDirectionFlags);

    }
}