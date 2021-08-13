using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Tree
{
    public abstract class NodeBase<T> : INode<T>
    {
        /// <summary>
        ///     Constructs a NodeBase object from an existing NodeBase object
        /// </summary>
        /// <param name="value"></param>
        protected NodeBase(INode<T> value) : this(value.Value, value.Previous)
        {
            Continuations = value.Continuations;
        }

        protected NodeBase(T value, INode<T> parent) : this()
        {
            Value = value;
            Previous = parent;
        }

        private NodeBase()
        {
            Continuations = new List<INode<T>>();
        }

        public INode<T> Previous { get; }

        public INode<T> Next => Continuations.FirstOrDefault();

        public T Value { get; }
        public List<INode<T>> Continuations { get; }

        public INode<T> AddNode(T nodeValue)
        {
            var childNode = new TreeNode<T>(nodeValue, this);
            Continuations.Add(childNode);
            return childNode;
        }
    }
}