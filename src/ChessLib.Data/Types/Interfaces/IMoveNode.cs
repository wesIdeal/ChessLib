using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using System;
using System.Collections.Generic;

namespace ChessLib.Types.Interfaces
{
    public interface IMoveNode<T>
        where T : IMove
    {
        uint Depth { get; }
        T MoveData { get; }
        IMoveNode<T> Parent { get; set; }
        List<MoveTree<T>> Variations { get; }
        IMoveNode<T> Next { get; set; }
        IMoveNode<T> Previous { get; set; }
        IMoveNode<T> CutNext();
        IMoveNode<T> AddVariation(IMoveNode<T> node);
        int GetHashCode();
        string ToString();
        MoveTree<T> AddVariation();
    }
}