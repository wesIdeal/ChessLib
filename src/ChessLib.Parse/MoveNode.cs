using System;
using System.Collections.Generic;

namespace ChessLib.Parse
{
    public class MoveNode<T> where T : IEquatable<T>
    {
        private MoveNode<T> _parent;
        private MoveNode()
        {
            Variations = new LinkedList<MoveTree<T>>();
        }
        public MoveNode(T move) : this(move, null) { }

        public MoveNode(T move, MoveNode<T> parent) : this()
        {
            Move = move;
            Parent = parent;
        }

        public MoveNode<T> Parent
        {
            get { return _parent; }

            set
            {

                _parent = value;
            }
        }
        public MoveTree<T> AddVariation()
        {
            return Variations.AddLast(new MoveTree<T>(this)).Value;
        }
        public LinkedList<MoveTree<T>> Variations { get; private set; }
        public T Move { get; set; }
        public uint Depth
        {
            get
            {
                uint depth = 0;
                var node = this;
                while (node.Parent != null)
                {
                    node = node.Parent;
                    depth++;
                }
                return depth;
            }
        }

        public string NAG { get; internal set; }

        public override int GetHashCode()
        {
            return Move.GetHashCode();
        }

        public override string ToString()
        {
            var str = Move.ToString();
            foreach (var variation in Variations)
            {
                str += $"\r\n\t[{Depth}]{variation.ToString()}";
            }
            return str;
        }
    }
}
