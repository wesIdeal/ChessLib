using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types.Interfaces;

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