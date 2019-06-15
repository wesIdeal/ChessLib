using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLib.Data
{
    public class MoveTree<T> : IEnumerable<T>, IMove
        where T : IMove
    {

        private IMoveNode<T> _last;
        private IMoveNode<T> _first;

        public IMoveNode<T> VariationParent { get; private set; }
        public MoveTree(MoveNode<T> parentMove)
        {
            VariationParent = parentMove;
        }

        public IMoveNode<T> AddMove(T move)
        {
            var node = new MoveNode<T>(move);
            return AddMove(node);
        }

        public IMoveNode<T> AddMove(IMoveNode<T> node)
        {
            node.Next = null;
            if (_first == null)
            {
                _first = _last = node;
                node.Previous = null;
            }
            else
            {
                node.Previous = _last;
                _last = node;
            }
            return _last;
        }

        public IMoveNode<T> AddVariation(IMoveNode<T> node)
        {
            if (_last == null)
            {
                return AddMove(node);
            }
            else
            {
                return _last.AddVariation(node);
            }
        }

        public IMoveNode<T> FirstMove => _first;
        public IMoveNode<T> LastMove => _last;


        public override string ToString()
        {
            var sb = new StringBuilder();
            IMoveNode<T> curr = _first;
            while (curr != null)
            {
                sb.AppendLine(curr.MoveData.ToString());
                curr = curr.Next;
            }
            return sb.ToString();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetItemsInReverse().Reverse().GetEnumerator();
        }

        public IEnumerator<T> GetReverseEnumerator()
        {
            return GetItemsInReverse().GetEnumerator();
        }

        public IEnumerable<T> GetItemsInReverse()
        {
            var curr = _last;
            while (curr != null)
            {
                yield return curr.MoveData;
                curr = curr.Previous ?? curr.Parent;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
