using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using System;

namespace ChessLib.Data
{
    public enum MoveApplicationStrategy
    {
        ContinueMainLine,
        Variation,
        NewMainLine,
        ReplaceMove
    }
    public interface IMoveTraversalService
    {
        string CurrentFEN { get; }
        /// <summary>
        /// Get Next Moves available from current move
        /// </summary>
        /// <returns>An array of possible stored moves. When a variation is encountered, the first item in the array is the main line, relative to the tree.</returns>
        MoveStorage[] GetNextMoves();

        event EventHandler<MoveMadeEventArgs> MoveMade;

        MoveNode<MoveStorage> ExitVariation();

        MoveNode<MoveStorage> ApplyMove(MoveExt move, MoveApplicationStrategy moveApplicationStrategy = MoveApplicationStrategy.ContinueMainLine);

        MoveNode<MoveStorage> ApplySANMove(string moveText, MoveApplicationStrategy moveApplicationStrategy = MoveApplicationStrategy.ContinueMainLine);

        /// <summary>
        /// Traverse the tree to the next move.
        /// </summary>
        /// <param name="move">The move that should be set to current.</param>
        /// <returns>Board state for the resulting board. null if the end of the tree was reached.
        /// <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>
        IBoard TraverseForward(MoveStorage move);

        /// <summary>
        /// Traverse the tree to the next move on the main line
        /// </summary>
        /// <returns>Board state for the resulting board. null if the end of the tree was reached.</returns>
        IBoard TraverseForward();


        /// <summary>
        /// Traverse the tree to the previous move.
        /// </summary>
        /// <returns>Board state for the resulting board.</returns>
        /// <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>
        IBoard TraverseBackward();

        void GoToInitialState();
        void GoToLastMove();
    }
}
