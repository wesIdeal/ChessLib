using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;

namespace ChessLib.Core.Types
{
    /// <summary>
    /// A class that holds a database containing a chess game.
    /// </summary>

    public class MoveTree : LinkedList<BoardSnapshot>, IEquatable<MoveTree>
    {
        internal MoveTree(MoveTree moveTree)
        {
            CopyTree(moveTree);
        }

        public Board InitialBoard => VariationParentNode.Value.Board;
        private void CopyTree(MoveTree moveTree)
        {
            foreach (var node in moveTree.AsQueryable())
            {
                AddLast(new BoardSnapshot(node));
            }
        }

        /// <summary>
        /// Creates a new tree of moves under the parent <paramref name="parentVariation"/>.
        /// </summary>
        /// <param name="parentVariation">The variation from which these moves begin.</param>
        /// <param name="move">A move to apply as a first move to the <paramref name="parentVariation"/>'s board.</param>
        public MoveTree(LinkedListNode<BoardSnapshot> parentVariation)
        {
            VariationParentNode = parentVariation;
        }

        /// <summary>
        /// Creates a new MoveTree from a board.
        /// </summary>
        /// <remarks>This should be used for the start of the game (initial board)</remarks>
        /// <param name="initialBoard">The board loaded into the property <see cref="InitialBoard"/></param>
        public MoveTree(Board initialBoard) 
        :this(new LinkedListNode<BoardSnapshot>(new BoardSnapshot(initialBoard)))
        {
          
        }


        public bool HasGameComment => !string.IsNullOrEmpty(GameComment);
        public string GameComment { get; set; }

        //public MoveNode<T> VariationParent { get; internal set; }

        /// <summary>
        /// The parent to the current variation
        /// </summary>
        public LinkedListNode<BoardSnapshot> VariationParentNode { get; }

        public string StartingFen => VariationParentNode.Value.Board.CurrentFEN;

        /// <summary>
        ///     Adds a move at the end of the tree.
        /// </summary>
        /// <param name="move"></param>
        /// <returns>the added move's node</returns>
        public LinkedListNode<BoardSnapshot> AddMove(BoardSnapshot move)
        {
            return AddLast(move);
        }

        public override string ToString()
        {
            var parent = VariationParentNode;
            var depth = 0;
            while (parent != null)
            {
                depth++;
                parent = ((MoveTree)parent.List).VariationParentNode;
            }

            var rv = $"Variation Depth: {depth}{Environment.NewLine}{StartingFen}";
            return rv;
        }


        public bool Equals(MoveTree other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var gameCommentEq = GameComment == other.GameComment;
            var variationParentNodeEq = Equals(VariationParentNode, other.VariationParentNode);
            var startingFenEq = StartingFen == other.StartingFen;
            var treeEqual = CheckEquality(other);
            return gameCommentEq && variationParentNodeEq && startingFenEq && treeEqual;
        }

        private bool CheckEquality(MoveTree other)
        {
            var iterator = this.First;
            var otherIterator = other.First;
            var areEqual = true;
            while (iterator != null && otherIterator != null)
            {
                var valuesEqual = iterator.Value.Equals(otherIterator.Value);
                areEqual &= valuesEqual;
                iterator = iterator.Next;
                otherIterator = otherIterator.Next;
            }

            if (iterator == null ^ otherIterator == null)
            {
                areEqual = false;
            }

            return areEqual;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MoveTree)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (GameComment != null ? GameComment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (VariationParentNode != null ? VariationParentNode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (StartingFen != null ? StartingFen.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(MoveTree left, MoveTree right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MoveTree left, MoveTree right)
        {
            return !Equals(left, right);
        }
    }
}