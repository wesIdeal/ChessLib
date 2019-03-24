using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Data
{
    public class MoveTree<T> : LinkedList<MoveNode<T>> where T : IEquatable<T>
    {
       
        public MoveNode<T> ParentMove { get; private set; }
        public MoveTree(MoveNode<T> parentMove)
        {
            ParentMove = parentMove;
        }
        public LinkedListNode<MoveNode<T>> Add(MoveNode<T> move)
        {
            move.Parent = ParentMove;
            return AddLast(move);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var mv in this)
            {
                sb.AppendLine(mv.ToString());
            }
            return sb.ToString();
        }

        public static implicit operator List<T>(MoveTree<T> v)
        {
            throw new NotImplementedException();
        }
    }
}
