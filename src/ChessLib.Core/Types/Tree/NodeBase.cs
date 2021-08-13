using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Tree
{
    public abstract class NodeBase<T> : INode<T>, IEquatable<NodeBase<T>>
    {
        /// <summary>
        ///     Constructs a NodeBase object from an existing NodeBase object
        /// </summary>
        /// <param name="value"></param>
        protected NodeBase(INode<T> value) : this(value.Value, (INode<T>)value.Previous)
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


        public INode<T> Previous { get; private set; }
        public INode<T> Next => Continuations.FirstOrDefault();

        public T Value { get; }
        public List<INode<T>> Continuations { get; }


        public virtual INode<T> AddNode(T nodeValue)
        {
            var childNode = new MoveTreeNode<T>(nodeValue, this);
            return AddNode(childNode);
        }

        public INode<T> AddNode(INode<T> nodeValue)
        {
            Continuations.Add(nodeValue);
            return nodeValue;
        }

        public bool Equals(NodeBase<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Previous, other.Previous) && EqualityComparer<T>.Default.Equals(Value, other.Value) && Equals(Continuations, other.Continuations);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NodeBase<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Previous != null ? Previous.GetHashCode() : 0);
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