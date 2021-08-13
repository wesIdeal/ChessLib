using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core.Types.GameTree
{
    public class BoardNode : TreeNode<PostMoveState>, IEquatable<BoardNode>
    {
        public Board Board { get; protected set; }
        public string Fen => Board.Fen;
        public List<INode<PostMoveState>> Variations => Continuations.Skip(1).ToList();

        public BoardNode(Board initialBoard) : base(new PostMoveState(initialBoard), null)
        {
            Board = (Board) initialBoard.Clone();
        }

        /// <summary>
        ///     Create a BoardNode, which consists of the board along with the postmove state.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="postMoveStateNode"></param>
        public BoardNode(Board board, INode<PostMoveState> postMoveStateNode) : base(postMoveStateNode)
        {
            Board = (Board) board.Clone();
        }

        public bool Equals(BoardNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Board, other.Board) && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BoardNode) obj);
        }

        public override int GetHashCode()
        {
            return Board != null ? Board.GetHashCode() : 0;
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