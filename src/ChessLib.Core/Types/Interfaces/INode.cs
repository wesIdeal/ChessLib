using System.Collections.Generic;

namespace ChessLib.Core.Types.Interfaces
{
    public interface INode<T>
    {
        INode<T> Previous { get; }
        INode<T> Next { get; }
        T Value { get; }
        List<INode<T>> Continuations { get; }
        INode<T> AddNode(T nodeValue);
    }
}