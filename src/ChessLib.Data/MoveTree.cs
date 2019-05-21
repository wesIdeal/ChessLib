using ChessLib.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Data
{
    public class MoveTree<T> : LinkedList<IMoveNode<T>>, IMoveTree<T>
    {
        public string FENStart { get; set; }
        public IMoveNode<T> ParentMove { get; }
        public MoveTree(IMoveNode<T> parentMove)
        {
            ParentMove = parentMove;
        }
        public LinkedListNode<IMoveNode<T>> Add(IMoveNode<T> move)
        {
            move.Parent = ParentMove;
            return AddLast(move);
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
    }
}
