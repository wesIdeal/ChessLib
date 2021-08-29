using System;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
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

        public bool Equals(PostMoveState other)
        {
            return BoardState == other.BoardState && MoveValue == other.MoveValue && San == other.San &&
                   BoardStateHash == other.BoardStateHash;
        }

       


        public object Clone()
        {
            return new PostMoveState(BoardState, MoveValue, BoardStateHash, San);
        }
    }
}