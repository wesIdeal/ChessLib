using System;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Tree
{
    public class MoveTreeNode<T> : NodeBase<T>, IEquatable<MoveTreeNode<T>>
    {
        public string Comment { get; internal set; } = string.Empty;
        public NumericAnnotation Annotation { get; } = new NumericAnnotation();

        public MoveTreeNode(T value, INode<T> parent) : base(value, parent)
        {
        }

        public bool Equals(MoveTreeNode<T> other)
        {
            return base.Equals(other);
        }

        public new MoveTreeNode<T> AddNode(T nodeValue)
        {
            return (MoveTreeNode<T>) base.AddNode(nodeValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MoveTreeNode<T>) obj);
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