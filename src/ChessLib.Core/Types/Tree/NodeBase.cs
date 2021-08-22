using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Tree
{
    public abstract class NodeBase<T> : INode<T>, IEquatable<NodeBase<T>>
        where T : IPostMoveState
    {
        /// <summary>
        /// Constructs a NodeBase object from an existing NodeBase object
        /// </summary>
        /// <param name="value"></param>
        protected NodeBase(INode<T> value) : this(value.Value, value.Previous)
        {
            value.Continuations.ForEach(c => { Continuations.Add((INode<T>)value.Clone()); });
        }

        /// <summary>
        /// Creates a NodeBase from a value and a parent node.
        /// </summary>
        /// <param name="value">Value of the data contained in node</param>
        /// <param name="parent">Parent / Previous node</param>
        protected NodeBase(T value, INode<T> parent) : this()
        {
            Value = (T)value.Clone();
            Previous = parent;
        }

        protected NodeBase(NodeBase<T> nodeBase, NodeBase<T> nodeBaseParent) : this(nodeBase.Value, nodeBaseParent)
        {
            foreach (var continuationNode in nodeBase.Continuations)
            {
                var clone = (INode<T>)continuationNode.Clone();
                Continuations.Add(clone);
            }
        }

        private NodeBase()
        {
            Continuations = new List<INode<T>>();
        }

        public abstract object Clone();


        public bool Equals(NodeBase<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var previousEqual = Equals(Previous, other.Previous);
            var valuesEqual = EqualityComparer<T>.Default.Equals(Value, other.Value);
            var continuationsEqual = Continuations.Zip(other.Continuations, (t, oth) => t.Equals(oth)).All(x=>x == true);
            return previousEqual && valuesEqual &&
                   continuationsEqual;
        }


        public INode<T> Previous { get; }

        /// <summary>
        ///     The value / data contained within the node.
        /// </summary>
        /// <remarks>Must implement ICloneable to ensure a deep copy can be made.</remarks>
        public T Value { get; }

        public List<INode<T>> Continuations { get; }


        public virtual INode<T> AddNode(T nodeValue)
        {
            var childNode = new MoveTreeNode<T>(nodeValue, this);
            return AddNode(childNode);
        }

        public virtual INode<T> AddNode(INode<T> nodeValue)
        {
            Continuations.Add(nodeValue);
            return nodeValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NodeBase<T>)obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Previous != null ? Previous.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Value);
                hashCode = (hashCode * 397) ^ (Continuations != null ? Continuations.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(NodeBase<T> left, NodeBase<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NodeBase<T> left, NodeBase<T> right)
        {
            return !Equals(left, right);
        }
    }
}