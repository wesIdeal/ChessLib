using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data
{
    public class MoveNode<T> : IMoveNode<T>
        where T : IMove
    {
        public MoveNode<T> Parent { get; set; }
        public MoveNode<T> Previous { get; set; }
        public MoveNode<T> Next { get; set; }

        private MoveNode()
        {
            Variations = new List<MoveTree<T>>();
            Parent = null;
        }

        public MoveNode(T move) : this()
        {
            MoveData = move;
        }

        public MoveNode(T move, MoveNode<T> parent) : this(move)
        {
            MoveData = move;
            Parent = parent;
        }

        public MoveTree<T> AddVariation()
        {
            Variations.Add(new MoveTree<T>(this));
            return Variations.Last();
        }

        public void AddVariation(MoveTree<T> tree)
        {
            tree.VariationParent = this;
            if (tree.HeadMove != null)
            {
                tree.HeadMove.Parent = this;
            }
            Variations.Add(tree);
        }

        public List<MoveTree<T>> Variations { get; }
        public T MoveData { get; }
        public uint Depth
        {
            get
            {
                uint depth = 0;
                MoveNode<T> node = this;
                while (node.Parent != null)
                {
                    node = node.Parent;
                    depth++;
                }
                return depth;
            }
        }


        MoveNode<T> IMoveNode<T>.Next { get; set; }

        public override int GetHashCode()
        {
            return MoveData.GetHashCode();
        }

        public override string ToString()
        {
            var str = MoveData.ToString();
            foreach (var variation in Variations)
            {
                str += $"\r\n\t[{Depth}]{variation}";
            }
            return str;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }


        public IMoveNode<T> CutNext()
        {
            Next = null;
            return this;
        }


    }
}
