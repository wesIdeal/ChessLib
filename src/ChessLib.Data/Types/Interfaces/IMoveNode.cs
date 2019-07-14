using System.Collections.Generic;

namespace ChessLib.Data.Types.Interfaces
{
    public interface IMoveNode<T>
        where T : IMove
    {
        uint Depth { get; }
        T MoveData { get; }
        MoveNode<T> Parent { get; set; }
        List<MoveTree<T>> Variations { get; }
        MoveNode<T> Next { get; set; }
        MoveNode<T> Previous { get; set; }
        IMoveNode<T> CutNext();
        int GetHashCode();
        string ToString();
        MoveTree<T> AddVariation();
        /// <summary>
        /// Adds a tree to the list of variations
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        void AddVariation(MoveTree<T> tree);
    }
}