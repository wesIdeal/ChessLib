using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Helpers;
using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.Magic.Init;

namespace ChessLib.Data.PieceMobility
{
    internal class PawnMovesInitializer : MoveInitializer
    {
        private Color _color;

        private PawnMovesInitializer()
        {
        }

        public PawnMovesInitializer(Color color)
        {
            _color = color;
            MoveDirectionFlags = color == Color.White ? MoveDirection.N : MoveDirection.S;
        }

        /// <summary>
        /// Gets the permutations of Occupancy/MoveValue boards from a given position
        /// </summary>
        /// <param name="pieceLocationIndex">The index of the piece</param>
        /// <param name="attackMask">The piece's associated attack mask from the position index</param>
        /// <param name="occupancyBoards">The associated occupancy boards</param>
        /// <returns>An array of blocker boards and corresponding moves based on blocker placement.</returns>
        public new IEnumerable<BlockerAndMoveBoards> GetAllPermutationsForAttackMask(int pieceLocationIndex, ulong attackMask, IEnumerable<ulong> occupancyBoards)
        {
            var boardCombos = new List<BlockerAndMoveBoards>();
            foreach (var board in occupancyBoards)
            {
                boardCombos.Add(new BlockerAndMoveBoards(board, CalculateMovesFromPosition(pieceLocationIndex, board)));
            }
            return boardCombos;
        }

        /// <summary>
        ///     Gets a board representing the squares a piece on a square can move to.
        /// </summary>
        /// <param name="positionIndex">The board index position of the piece</param>
        /// <param name="occupancyBoard">A bitboard representation of occupied squares</param>
        /// <param name="moveDirectionFlags">The directions in which the piece can move</param>
        /// <param name="attackArrayGen">
        ///     When true, excludes outer board edges (for attack masks). When false, provides all
        ///     possible moves.
        /// </param>
        /// <returns>A bitboard representation of legal moves from given position</returns>
        public ulong CalculateMovesFromPosition(int positionIndex, ulong occupancyBoard,
             bool attackArrayGen = false)
        {
            var moves = GetPawnMovesFromPosition(positionIndex, occupancyBoard);
            var attacks = GetPawnAttacksFromPosition(positionIndex, occupancyBoard);
            return moves | attacks;
        }

       
        private ulong GetPawnAttacksFromPosition(int positionIndex, ulong occupancyBoard)
        {
            var attacks = PieceAttackPatterns.Instance.PawnAttackMask[(int) _color][positionIndex];
            return attacks & occupancyBoard;
        }

        private ulong GetPawnMovesFromPosition(int positionIndex, ulong occupancyBoard)
        {
            ulong rv = 0;
            var potentialMoves = PieceAttackPatterns.Instance.PawnMoveMask[(int) _color][positionIndex];
            foreach (var potentialMoveSq in potentialMoves.GetSetBits())
            {
                var moveSqValue = potentialMoveSq.GetBoardValueOfIndex();
                var inBetween = BoardHelpers.InBetween(positionIndex, potentialMoveSq);
                var checkSquares = inBetween | potentialMoveSq.GetBoardValueOfIndex();
                if (checkSquares.GetSetBits().All(ib => (ib.GetBoardValueOfIndex() & occupancyBoard) == 0))
                {
                    rv |= moveSqValue;
                }
            }

            return rv;
        }
    }
}