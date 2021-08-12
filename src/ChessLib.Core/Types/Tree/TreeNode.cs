using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Tree
{
    public class TreeNode<T> : NodeBase<T>
    {
        public TreeNode(T value, INode<T> parent) : base(value, parent)
        {
        }
    }
}