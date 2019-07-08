using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data
{
    public class MoveTree<T> : IEnumerable<T>, IMove
        where T : IMove
    {

        private IMoveNode<T> _last;
        private IMoveNode<T> _first;

        public IMoveNode<T> VariationParent { get; }
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

            return _last.AddVariation(node);
        }

        public IMoveNode<T> FirstMove => _first;
        public IMoveNode<T> LastMove => _last;


        public override string ToString()
        {
            var sb = new StringBuilder();
            IMoveNode<T> current = _first;
            while (current != null)
            {
                sb.AppendLine(current.MoveData.ToString());
                current = current.Next;
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
            var current = _last;
            while (current != null)
            {
                yield return current.MoveData;
                current = current.Previous ?? current.Parent;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
