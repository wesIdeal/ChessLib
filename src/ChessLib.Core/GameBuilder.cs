using System.Diagnostics;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.GameTree;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core
{
    public abstract class GameBuilder : GameEnumerator
    {
        internal static readonly SanToMove sanToMove = new SanToMove();

        /// <summary>
        ///     Applies a move to the current board.
        /// </summary>
        /// <param name="san">Simple Algebraic Notation of move.</param>
        /// <param name="moveApplicationStrategy">Strategy for how move should be applied</param>
        /// <remarks>
        ///     After adding move to <see cref="NodeBase{T}.Continuations" />, <see cref="GameEnumerator.Current" /> is
        ///     adjusted to reflect new board state.
        /// </remarks>
        public void ApplyMove(string san, MoveApplicationStrategy moveApplicationStrategy)
        {
            Debug.Assert(Current != null);
            var move = sanToMove.GetMoveFromSAN(Current.Board, san);
            ApplyMove(move, moveApplicationStrategy);
        }

        internal void ApplyMove(Move move, MoveApplicationStrategy moveApplicationStrategy)
        {
            var postMoveBoard = Current?.Board.ApplyMoveToBoard(move);
            var postMoveNode = Current?.AddNode(new PostMoveState(postMoveBoard, move, ""));
            Current = new BoardNode(postMoveBoard, postMoveNode);
        }
    }
}