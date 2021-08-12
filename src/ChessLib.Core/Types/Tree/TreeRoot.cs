namespace ChessLib.Core.Types.Tree
{
    public class TreeRoot<T> : NodeBase<T>
    {
        public TreeRoot(T value) : base(value, null)
        {
            if (value == null)
            {
                throw new TreeException(TreeErrorType.StartingNodeCannotBeNull);
            }
        }
    }

}