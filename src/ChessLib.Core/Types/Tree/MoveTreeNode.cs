using System;
using System.Linq;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Tree
{
    public class MoveTreeNode<T> : NodeBase<T>, ICloneable, IEquatable<MoveTreeNode<T>> where T : ICloneable, IEquatable<T>
    {
        public string Comment { get; internal set; } = string.Empty;
        public NumericAnnotation Annotation { get; } = new NumericAnnotation();

        public MoveTreeNode(T value, INode<T> parent) : base(value, parent)
        {
        }

        internal MoveTreeNode(MoveTreeNode<T> value, MoveTreeNode<T> parent) : base(value, parent)
        {
            Comment = value.Comment;
            Annotation = value.Annotation;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool Equals(MoveTreeNode<T> other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// Clones a <see cref="MoveTreeNode{T}"/> using the current <see cref="INode{T}.Previous"/> as the parent node.
        /// </summary>
        /// <returns>A new MoveTreeNode that is a copy of the current.</returns>
        public override object Clone()
        {
            return new MoveTreeNode<T>(this, (MoveTreeNode<T>)this.Previous);
        }

       




        /// <summary>
        ///     Adds a root node
        /// </summary>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        internal new MoveTreeNode<T> AddNode(T nodeValue)
        {
            return (MoveTreeNode<T>)base.AddNode(nodeValue);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MoveTreeNode<T>)obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(MoveTreeNode<T> left, MoveTreeNode<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MoveTreeNode<T> left, MoveTreeNode<T> right)
        {
            return !Equals(left, right);
        }
    }
}