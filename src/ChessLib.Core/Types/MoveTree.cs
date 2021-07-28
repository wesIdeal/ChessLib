using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLib.Core.Types
{
    public class MoveTree : LinkedList<BoardSnapshot>
    {
        internal MoveTree(MoveTree moveTree)
        {
            this.Clear();
            CopyTree(moveTree);
        }

        private void CopyTree(MoveTree moveTree)
        {
            foreach (var node in moveTree.AsQueryable())
            {
                this.AddLast(new BoardSnapshot(node));

            }
        }


        public MoveTree(LinkedListNode<BoardSnapshot> parentVariation, string fen)
        {
            StartingFEN = fen;
            VariationParentNode = parentVariation;
            if (VariationParentNode == null)
            {
                AddFirst(new LinkedListNode<BoardSnapshot>(new BoardSnapshot()));
            }
        }

        public bool HasGameComment => !string.IsNullOrEmpty(GameComment);
        public string GameComment { get; set; }

        //public MoveNode<T> VariationParent { get; internal set; }
        public LinkedListNode<BoardSnapshot> VariationParentNode { get; }
        public string StartingFEN { get; set; }

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
                parent = ((MoveTree) parent.List).VariationParentNode;
            }

            var rv = $"Variation Depth: {depth}{Environment.NewLine}{StartingFEN}";
            return rv;
        }
    }
}