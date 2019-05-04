using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Data
{
    public class MoveTree<T> : LinkedList<MoveNode<T>>
    {
        public string FENStart { get; set; }
        public MoveNode<T> ParentMove { get; }
        public MoveTree(MoveNode<T> parentMove)
        {
            ParentMove = parentMove;
        }
        public LinkedListNode<MoveNode<T>> Add(MoveNode<T> move)
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
