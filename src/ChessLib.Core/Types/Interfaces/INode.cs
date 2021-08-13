using System.Collections.Generic;

namespace ChessLib.Core.Types.Interfaces
{
    public interface INode<T>
    {
        INode<T> Previous { get; }
        T Value { get; }
        List<INode<T>> Continuations { get; }
    }
}