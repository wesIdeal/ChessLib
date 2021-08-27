using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core.Types.GameTree
{
    public class BoardNode : ICloneable, IEquatable<BoardNode>
    {
        public Board Board { get; protected set; }
        public MoveTreeNode<PostMoveState> Node { get; }
        public string Fen => Board.Fen;

        /// <summary>
        ///     Gets the variations from this board state.
        /// </summary>
        public List<INode<PostMoveState>> Variations => Node.Continuations.Skip(1).ToList();

        public BoardNode(Board rootNodeBoard)
        {
            Board = (Board)rootNodeBoard.Clone();
            Node = new MoveTreeNode<PostMoveState>(new PostMoveState(Board, Move.NullMove, ""), null);
        }

        /// <summary>
        ///     Create a BoardNode, which consists of the board along with the postmove state.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="postMoveStateNode"></param>
        public BoardNode(Board board, INode<PostMoveState> postMoveStateNode)
        {
            Board = (Board)board.Clone();
            Node = (MoveTreeNode<PostMoveState>)postMoveStateNode;
        }

        public BoardNode(BoardNode boardNode)
        {
            Node = (MoveTreeNode<PostMoveState>)boardNode.Node.Clone();
            Board = (Board)boardNode.Board.Clone();
        }

        public object Clone()
        {
            return new BoardNode(this);
        }

        public bool Equals(BoardNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Board, other.Board) && Node.Equals(other.Node);
        }

        public override string ToString()
        {
            return Node.Value.MoveValue == Move.NullMove ? "Initial" : Node.Value.San;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BoardNode)obj);
        }

        public override int GetHashCode()
        {
            return (int)Node.Value.BoardStateHash;
        }

        public static bool operator ==(BoardNode left, BoardNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BoardNode left, BoardNode right)
        {
            return !Equals(left, right);
        }
    }
}