using System.Collections.Generic;

namespace ChessLib.Types.Interfaces
{
    public interface IMoveNode<T>
    {
        uint Depth { get; }
        T Move { get; }
        IMoveNode<T> Parent { get; set; }
        LinkedList<IMoveTree<T>> Variations { get; }
        IMoveTree<T> AddVariation();
        int GetHashCode();
        string ToString();
    }
}