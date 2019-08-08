using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
namespace ChessLib.Data
{
    public interface IMoveTraversalService
    {
        /// <summary>
        /// Get Next Moves available from current move
        /// </summary>
        /// <returns>An array of possible stored moves. When a variation is encountered, the first item in the array is the main line, relative to the tree.</returns>
        MoveStorage[] GetNextMoves();
        /// <summary>
        /// Traverse the tree to the next move.
        /// </summary>
        /// <param name="move">The move that should be set to current.</param>
        /// <returns>Board state for the resulting board.</returns>
        /// <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>
        IBoard TraverseForward(MoveStorage move);

        /// <summary>
        /// Traverse the tree to the next move.
        /// </summary>
        /// <param name="move">The move that should be set to current.</param>
        /// <param name="indexesUpdated">An array of board indexes for squares affected by move. Handy for a UI to call an update for these squares.</param>
        /// <returns>Board state for the resulting board.</returns>
        /// <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>
        IBoard TraverseForward(MoveStorage move, out ushort[] indexesUpdated);

        /// <summary>
        /// Traverse the tree to the previous move.
        /// </summary>
        /// <returns>Board state for the resulting board.</returns>
        /// <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>
        IBoard TraverseBackward();

        /// <summary>
        /// Traverse the tree to the previous move.
        /// </summary>
        /// <param name="indexesUpdated">An array of board indexes for squares affected by move. Handy for a UI to call an update for these squares.</param>
        /// <returns>Board state for the resulting board.</returns>
        /// <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>
        IBoard TraverseBackward(out ushort[] indexesUpdated);
    }
    public class MoveTraversalService
    {

    }
}
