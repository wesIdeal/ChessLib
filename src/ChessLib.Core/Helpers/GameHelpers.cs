using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.GameTree;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core.Helpers
{
    public static class GameHelpers
    {
        /// <summary>
        /// Returns the complete main line of the game, not jumping to variation branches.
        /// </summary>
        /// <param name="game">The game to get the main line from</param>
        /// <returns>Returns a collection of <see cref="PostMoveState"/> objects containing board and move information for each move of the game.</returns>
        public static IEnumerable<INode<PostMoveState>> MainLine(this Game game)
        {
            var mainLine = game.InitialNode.Node.Continuations.MainLine();
            return mainLine;
        }

        public static Game ApplyMove(this Game game, params string[] sanMoves)
        {
            foreach (var move in sanMoves)
            {
                game.ApplyMove(move, MoveApplicationStrategy.ContinueMainLine);
            }

            return game;
        }

        /// <summary>
        ///     Applies a move to the current board.
        /// </summary>
        /// <param name="san">Simple Algebraic Notation of move.</param>
        /// <param name="moveApplicationStrategy">Strategy for how move should be applied</param>
        /// <remarks>
        ///     After adding move to <see cref="NodeBase{T}.Continuations" />, <see cref="GameEnumerator.Current" /> is
        ///     adjusted to reflect new board state.
        /// </remarks>
        public static Game ApplyMove(this Game game, string san, MoveApplicationStrategy moveApplicationStrategy)
        {
            Debug.Assert(game?.Current != null);
            var move = sanToMove.GetMoveFromSAN(game.Current.Board, san);
            game.ApplyMove(move, moveApplicationStrategy);
            return game;
        }
        internal static readonly SanToMove sanToMove = new SanToMove();

        public static Game ApplyMove(this Game game, Move move, MoveApplicationStrategy moveApplicationStrategy)
        {
            Debug.Assert(game?.Current != null);
            var postMoveBoardNode = BoardNodeFactory.ApplyMoveToBoard(game.Current, move);
            game.Current.Node.AddNode(postMoveBoardNode.Node);
            game.Current = postMoveBoardNode;
            return game;
        }

        private static IEnumerable<INode<PostMoveState>> MainLine(this IEnumerable<INode<PostMoveState>> continuations,
            List<INode<PostMoveState>> aggregator = null)
        {
            aggregator ??= new List<INode<PostMoveState>>();
            aggregator.ToList();
            var nextMove = continuations.FirstOrDefault();
            if (nextMove == null)
            {
                return aggregator;
            }

            aggregator.Add(nextMove);
            return nextMove.Continuations.MainLine(aggregator);
        }
    }
}