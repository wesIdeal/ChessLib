using System;
using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.GameTree;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core
{
    public class MoveTree : List<PostMoveState>
    {
        public int NodeDepth { get; internal set; }

        public MoveTree(int depth)
        {
            NodeDepth = depth;
        }
    }

    public static class BoardNodeFactory
    {
        private readonly static MoveToSan moveToSan = new MoveToSan();
        public static BoardNode ApplyMoveToBoard(BoardNode board, Move moveToApply)
        {
            var postMoveBoard = board.Board.ApplyMoveToBoard(moveToApply);
            var san = moveToSan.Translate(moveToApply, board.Board, postMoveBoard);
            var postMoveState = new PostMoveState(postMoveBoard, moveToApply, san);//TODO: Get real san, here
            var node = new TreeNode<PostMoveState>(postMoveState, board.Node);
            return new BoardNode(postMoveBoard, node);
        }

        public static BoardNode ApplyExistingNode(Board currentBoard, INode<PostMoveState> nodeToApply)
        {
            var postMoveBoard = currentBoard.ApplyMoveToBoard(nodeToApply.Value.MoveValue);
            return new BoardNode(postMoveBoard, nodeToApply);
        }

        public static BoardNode UnapplyMoveFromBoard(BoardNode currentBoardNode)
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
    public readonly struct PostMoveState : IEquatable<PostMoveState>
    {
        public PostMoveState(Board initialBoard) : this(initialBoard, Move.NullMove, "")
        {
        }

        public PostMoveState(Board postMoveBoard, ushort moveValue, string san, string comment = "",
            NumericAnnotation annotation = null)
        {
            BoardState = postMoveBoard;
            Comment = comment;
            Annotation = annotation;
            MoveValue = moveValue;
            San = san;
            BoardStateHash = PolyglotHelpers.GetBoardStateHash(postMoveBoard);
        }

        public uint BoardState { get; }
        public ushort MoveValue { get; }
        public string San { get; }

        public ulong BoardStateHash { get; }
        public string Comment { get; }
        public NumericAnnotation Annotation { get; }

        public bool Equals(PostMoveState other)
        {
            return BoardState == other.BoardState && MoveValue == other.MoveValue && San == other.San &&
                   BoardStateHash == other.BoardStateHash && Comment == other.Comment &&
                   Equals(Annotation, other.Annotation);
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
                hashCode = (hashCode * 397) ^ (Comment != null ? Comment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Annotation != null ? Annotation.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(PostMoveState left, PostMoveState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PostMoveState left, PostMoveState right)
        {
            return !left.Equals(right);
        }
    }
}