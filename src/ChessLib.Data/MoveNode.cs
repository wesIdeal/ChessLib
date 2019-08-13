using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data
{
    //public class MoveNode : LinkedListNode<MoveStorage>
    //{
    //    protected MoveTree<T> TreeOwner { get; set; }
    //    public MoveNode<T> ParentTreeMove { get; set; }
    //    public MoveNode<T> Previous { get; set; }
    //    public MoveNode<T> Next { get; set; }
    //    private bool _isNullNode = false;
    //    private readonly ushort _id;
    //    private MoveNode()
    //    {
    //        Variations = new List<MoveTree<T>>();
    //        ParentTreeMove = null;
    //        var random = new Random(DateTime.Now.Millisecond);
    //        _id = (ushort)random.Next(ushort.MaxValue);
    //    }

    //    public static MoveNode<T> GetNullNode()
    //    {
    //        var nodeData = (T)Activator.CreateInstance(typeof(T));
    //        return new MoveNode<T>(nodeData, null);
    //    }

    //    public MoveNode(T move, MoveNode<T> parentTreeMove) : this()
    //    {
    //        MoveData = move;
    //        ParentTreeMove = parentTreeMove;
    //    }

    //    public bool IsNullNode => MoveData.IsNullMove;


    //    public List<MoveTree<T>> Variations { get; }
    //    public T MoveData { get; }
    //    public uint Depth
    //    {
    //        get
    //        {
    //            uint depth = 0;
    //            MoveNode<T> node = this;
    //            while (node.ParentTreeMove != null)
    //            {
    //                node = node.ParentTreeMove;
    //                depth++;
    //            }
    //            return depth;
    //        }
    //    }

    //    public override int GetHashCode() => MoveData.GetHashCode();

    //    public override string ToString()
    //    {
    //        var str = $"Move {MoveData.ToString()}";
    //        if(ParentTreeMove != null)
    //        {
    //            str += $" from variation of move {ParentTreeMove}";
    //        }
    //        return str;
    //    }

    //    public object Clone()
    //    {
    //        throw new NotImplementedException();
    //    }


    //    public MoveNode<T> CutNext()
    //    {
    //        Next = null;
    //        return this;
    //    }

    //    public MoveNode<T> AddAsReplacement(T moveStorage)
    //    {
    //        return AddNextMove(moveStorage, ParentTreeMove);
    //    }

    //    public MoveNode<T> AddNextMove(T moveStorage, MoveNode<T> variationParentNode)
    //    {
    //        var node = new MoveNode<T>(moveStorage, variationParentNode);
    //        node.Previous = this;
    //        this.Next = node;
    //        return this.Next;
    //    }

    //    public MoveNode<T> AddAsVariation(T move)
    //    {
    //        var node = new MoveNode<T>(move, this);
    //        Variations.Add(new MoveTree<T>(this));
    //        Variations.Last().AddMove(move);
    //        return node;
    //    }

    //    public bool Equals(MoveNode<T> other)
    //    {
    //        if (other == null) return false;
    //        var idsEqual = _id == other._id;
    //        //try
    //        //{
    //        //    var parentsEqual = (ParentTreeMove == null && other.ParentTreeMove == null);
    //        //    if (!parentsEqual && ParentTreeMove != null)
    //        //    {
    //        //       parentsEqual = ParentTreeMove.Equals(other.ParentTreeMove);
    //        //    }
    //        //    var dataEqual = MoveData.Equals(other.MoveData);
    //        //    return idsEqual && parentsEqual && dataEqual;
    //        //}
    //        //catch (Exception e)
    //        //{
    //        //    Debug.WriteLine(e);
    //        //    throw;
    //        //}
    //        return idsEqual;
    //    }
    //}
}
