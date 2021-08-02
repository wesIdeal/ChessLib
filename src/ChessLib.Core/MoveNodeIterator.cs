using ChessLib.Core.Helpers;

namespace ChessLib.Core
{
    /// <summary>
    ///     Is a <see cref="MoveNode" /> that contains a <see cref="Board" />.
    /// </summary>
    public class MoveNodeIterator : MoveNode
    {
        /// <summary>
        ///     Constructs an iterator, using the values from the node and setting the board from the move.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moveNode"></param>
        public MoveNodeIterator(Board board, MoveNode moveNode) : base(moveNode)
        {
            BoardState = board;
        }


        /// <summary>
        /// Applies <paramref name="move"/> to <paramref name="board"/>, creating a new <see cref="MoveNodeIterator"/> as a subsequent move in a <see cref="Game"/>.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="move"></param>
        protected MoveNodeIterator(Board board, Move move) : base(move)
        {
            if (!move.IsNullMove)
            {
                var preMoveValue = board;
                var postMoveValue = board.ApplyMoveToBoard(move);
                var san = MoveToSan.Translate(move, preMoveValue, postMoveValue);
                San = san;
                BoardStateValue = BoardState = postMoveValue;

            }
            else
            {
                BoardStateValue = BoardState = board;
            }

        }

        public override string ToString()
        {
            return $"{San} -> {Fen}";
        }

        protected MoveNodeIterator(MoveNodeIterator board) : this(board.BoardState, Move.NullMove)
        {
            foreach (var gameContinuation in board.Continuations)
            {
                Continuations.Add(new MoveNode(gameContinuation));
            }
        }

        public new Board BoardState { get; }

        /// <summary>
        ///     Returns a bool indicating whether the current node is the beginning of the game.
        /// </summary>
        public bool IsInitialBoard => Move.IsNullMove;

        public string Fen => BoardState.CurrentFEN;

        public MoveNodeIterator AddContinuation(Move move)
        {
            var postMoveNode = new MoveNodeIterator(BoardState, move);
            AddContinuation(postMoveNode);
            return postMoveNode;
        }



        #region Navigation

        /// <summary>
        ///     Gets the next board, <see cref="MoveNode.Continuations" />'s first, and if null, returns the current iterator.
        /// </summary>
        public new MoveNodeIterator Next
        {
            get
            {
                MoveNodeIterator iterator = null;
                var next = base.Next;
                if (next != null)
                {
                    iterator = new MoveNodeIterator(BoardState, next);
                }

                return iterator;
            }
        }



        #endregion Navigation


    }
}