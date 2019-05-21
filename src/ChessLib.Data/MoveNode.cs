using ChessLib.Types.Interfaces;
using System;
using System.Collections.Generic;

namespace ChessLib.Data
{
    public class MoveNode<T> : IMoveNode<T>
    {
        private MoveNode()
        {
            Variations = new LinkedList<IMoveTree<T>>();
        }
        public MoveNode(T move) : this(move, null) { }

        public MoveNode(T move, MoveNode<T> parent) : this()
        {
            Move = move;
            Parent = parent;
        }

        public IMoveNode<T> Parent { get; set; }

        public IMoveTree<T> AddVariation()
        {
            return Variations.AddLast(new MoveTree<T>(this)).Value;
        }
        public LinkedList<IMoveTree<T>> Variations { get; }
        public T Move { get; }
        public uint Depth
        {
            get
            {
                uint depth = 0;
                IMoveNode<T> node = this;
                while (node.Parent != null)
                {
                    node = node.Parent;
                    depth++;
                }
                return depth;
            }
        }



        public override int GetHashCode()
        {
            return Move.GetHashCode();
        }

        public override string ToString()
        {
            var str = Move.ToString();
            foreach (var variation in Variations)
            {
                str += $"\r\n\t[{Depth}]{variation}";
            }
            return str;
        }
    }
}
