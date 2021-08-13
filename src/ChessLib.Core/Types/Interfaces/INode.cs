using System.Collections.Generic;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core.Types.Interfaces
{
    public interface INode<T>
    {
        INode<T> Previous { get; }
        INode<T> Next { get; }
        T Value { get; }
        List<INode<T>> Continuations { get; }
        INode<T> AddNode(T nodeValue);
        INode<T> AddNode(INode<T> nodeValue);
    }
}