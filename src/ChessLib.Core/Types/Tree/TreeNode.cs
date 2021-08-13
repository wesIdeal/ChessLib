using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Tree
{
    public class TreeNode<T> : NodeBase<T>
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
    }
}