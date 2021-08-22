using System;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.GameTree;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core
{
    public static class BoardNodeFactory
    {
        private static readonly MoveToSan moveToSan = new MoveToSan();

        public static BoardNode ApplyMoveToBoard(BoardNode board, Move moveToApply)
        {
            var postMoveBoard = board.Board.ApplyMoveToBoard(moveToApply);
            var san = moveToSan.Translate(moveToApply, board.Board, postMoveBoard);
            var postMoveState = new PostMoveState(postMoveBoard, moveToApply, san);
            var node = new MoveTreeNode<PostMoveState>(postMoveState, board.Node);
            return new BoardNode(postMoveBoard, node);
        }

        public static BoardNode ApplyExistingNode(Board currentBoard, INode<PostMoveState> nodeToApply)
        {
            var postMoveBoard = currentBoard.ApplyMoveToBoard(nodeToApply.Value.MoveValue);
            return new BoardNode(postMoveBoard, nodeToApply);
        }

        public static BoardNode UnApplyMoveFromBoard(BoardNode currentBoardNode)
        {
            var previousState = currentBoardNode.Node.Previous;
            var unDoneBoard = currentBoardNode.Board.UnapplyMoveFromBoard((BoardState)previousState.Value.BoardState,
                currentBoardNode.Node.Value.MoveValue);
            return new BoardNode(unDoneBoard, previousState);
        }
    }

    /// <summary>
    ///     A move that was applied to a board, resulting in <see cref="BoardState" />
    /// </summary>
    public readonly struct PostMoveState : IPostMoveState, IEquatable<PostMoveState>
    {
        public PostMoveState(Board postMoveBoard, ushort moveValue, string san)
        {
            BoardState = postMoveBoard;
            MoveValue = moveValue;
            San = san;
            BoardStateHash = PolyglotHelpers.GetBoardStateHash(postMoveBoard);
        }

        private PostMoveState(uint postMoveBoardState, ushort moveValue, ulong boardStateHash, string san)
        {
            BoardState = postMoveBoardState;
            MoveValue = moveValue;
            BoardStateHash = boardStateHash;
            San = san;
        }

        public uint BoardState { get; }
        public ushort MoveValue { get; }
        public string San { get; }
        public ulong BoardStateHash { get; }


        /// <summary>
        ///     <see cref="Color" /> of side making the <see cref="Move" /> that resulted in <see cref="BoardState" />.
        /// </summary>
        public Color ColorMakingMove => ((BoardState)BoardState).ActivePlayer.Toggle();

        /// <summary>
        ///     Gets the full whole-move count of this move.
        /// </summary>
        /// <remarks>
        ///     If the last move was from <see cref="Color.Black" />, then the move is actually the board state move minus
        ///     one.
        /// </remarks>
        public uint MoveNumber => (uint)(((BoardState)(BoardState)).FullMoveCounter - (ColorMakingMove == Color.Black ? 1 : 0));

        public bool Equals(PostMoveState other)
        {
            return BoardState == other.BoardState && MoveValue == other.MoveValue && San == other.San &&
                   BoardStateHash == other.BoardStateHash;
        }

        public bool Equals(IPostMoveState other)
        {
            if (other == null)
            {
                return false;
            }

            return BoardState == other.BoardState && MoveValue == other.MoveValue && San == other.San;
        }

        public override bool Equals(object obj)
        {
            return obj is PostMoveState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)BoardState;
                hashCode = (hashCode * 397) ^ MoveValue.GetHashCode();
                hashCode = (hashCode * 397) ^ (San != null ? San.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ BoardStateHash.GetHashCode();
                return hashCode;
            }
        }

        public object Clone()
        {
            return new PostMoveState(BoardState, MoveValue, BoardStateHash, San);
        }

        public static bool operator ==(PostMoveState left, PostMoveState right)
        {
            return left.Equals(right);
        }

        public override string ToString()
        {
            return MoveValue == Move.NullMove ? "[Initial]" : San;
        }

        public static bool operator !=(PostMoveState left, PostMoveState right)
        {
            return !left.Equals(right);
        }
    }
}