using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ChessLib.Core.Helpers;

namespace ChessLib.Core.Types.GameTree
{
    /// <summary>
    ///     Defines items necessary to traverse game and implements <see cref="IEnumerator{T}" />'s MoveNext() and Dispose().
    /// </summary>
    /// <remarks>Dispose is empty and unnecessary.</remarks>
    public abstract class GameEnumerator : IEnumerator<BoardNode>
    {
        protected const int MainLineIndex = 0;

        public bool MoveNext()
        {
            return MoveNext(MainLineIndex);
        }

        public abstract void Reset();

        public BoardNode Current { get; protected set; }

        object IEnumerator.Current => Current;

        /// <summary>
        ///     Empty dispose method.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
        }


        /// <summary>
        ///     Will move to the next continuation of index
        ///     <param name="continuationIndex"></param>
        ///     , if available.
        /// </summary>
        /// <param name="continuationIndex">
        ///     Index of the games branch to traverse. 0 is used to traverse the main line from the
        ///     current board.
        /// </param>
        /// <returns>True if successful, False if no continuation was found at the given index.</returns>
        public virtual bool MoveNext(int continuationIndex)
        {
            Debug.Assert(Current != null);
            if (continuationIndex < Current.Continuations.Count)
            {
                var continuation = Current.Continuations[continuationIndex];
                var postMoveBoard = Current.Board.ApplyMoveToBoard(continuation.Value.MoveValue);
                Current = new BoardNode(postMoveBoard, continuation);
                return true;
            }

            return false;
        }

        public virtual bool MoveNext(ushort move)
        {
            var index = Current.Continuations.FindIndex(0, x => x.Value.MoveValue == move);
            return index >= 0 && MoveNext(index);
        }

        /// <summary>
        ///     Will move to the previous board state, if available.
        /// </summary>
        /// <returns>True if successful, False if <see cref="Current" /> is the game's initial board.</returns>
        /// <remarks>When successful, <see cref="Current" /> contains post move information including board..</remarks>
        public bool MovePrevious()
        {
            Debug.Assert(Current != null);
            if (Current.Previous == null)
            {
                return false;
            }

            var previousBoard = Current.Board.UnapplyMoveFromBoard((BoardState) Current.Previous.Value.BoardState,
                Current.Value.MoveValue);
            var previousBoardNode = new BoardNode(previousBoard, Current.Previous);
            Current = previousBoardNode;
            return true;
        }
    }
}