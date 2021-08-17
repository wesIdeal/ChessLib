using System;
using System.Collections.Generic;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core.Types.Interfaces
{
    public interface INode<T> : ICloneable
    {
        INode<T> Previous { get; }
        T Value { get; }
        List<INode<T>> Continuations { get; }
    }
}