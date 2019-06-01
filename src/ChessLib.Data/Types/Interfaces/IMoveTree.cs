using System.Collections.Generic;

namespace ChessLib.Types.Interfaces
{
    public interface IMoveTree<T> : ICollection<IMoveNode<T>>
    {
        string FENStart { get; set; }
        IMoveNode<T> ParentMove { get; }
        new LinkedListNode<IMoveNode<T>> Add(IMoveNode<T> move);
        string ToString();
    }
}
