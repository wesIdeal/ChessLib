using System;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Tree
{
    public class TreeNode<T> : NodeBase<T>, IEquatable<TreeNode<T>>
    {
        /// <summary>
        ///     Constructs a TreeNode from a NodeBase object, using it's values to initialize the node.
        /// </summary>
        /// <param name="value"></param>
        public TreeNode(INode<T> value) : base(value)
        {
        }

        public TreeNode(T value, INode<T> parent) : base(value, parent)
        {
        }

        public bool Equals(TreeNode<T> other)
        {
            return base.Equals(other);
        }

        public new TreeNode<T> AddNode(T nodeValue)
        {
            return (TreeNode<T>) base.AddNode(nodeValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TreeNode<T>) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(TreeNode<T> left, TreeNode<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TreeNode<T> left, TreeNode<T> right)
        {
            return !Equals(left, right);
        }
    }
}