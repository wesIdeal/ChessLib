using System;
using System.Collections.Generic;
using ChessLib.Core;

namespace ChessLib.Data
{
    public class MoveTree : LinkedList<MoveStorage>
    {
        /// <summary>
        ///     Adds a move at the end of the tree.
        /// </summary>
        /// <param name="move"></param>
        /// <returns>the added move's node</returns>
        public LinkedListNode<MoveStorage> AddMove(MoveStorage move)
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


        public MoveTree(LinkedListNode<MoveStorage> parentVariation, string fen)
        {
            StartingFEN = fen;
            VariationParentNode = parentVariation;
            if (VariationParentNode == null)
            {
                AddFirst(new LinkedListNode<MoveStorage>(new MoveStorage()));
            }
        }

        public bool HasGameComment => !string.IsNullOrEmpty(GameComment);
        public string GameComment { get; set; }

        //public MoveNode<T> VariationParent { get; internal set; }
        public LinkedListNode<MoveStorage> VariationParentNode { get; }
        public string StartingFEN { get; set; }
    }
}