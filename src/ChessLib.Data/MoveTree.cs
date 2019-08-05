using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLib.Data.Annotations;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data
{
    public class MoveTree<T> : IEnumerable<T>
        where T : IMove
    {
        private MoveNode<T> _last;
        private MoveNode<T> _head;

        public MoveNode<T> VariationParent { get; internal set; }


        public MoveTree([ItemNotNull]MoveNode<T> parentMove)
        {
            VariationParent = parentMove;
            if (VariationParent == null)
            {
                _head = MoveNode<T>.MakeNullNode;
                _last = _head;
            }
        }

        public IMoveNode<T> AddMove(T move)
        {
            return InsertLast(move);
        }

        public IMoveNode<T> InsertFirst(T move)
        {
            var node = new MoveNode<T>(move);
            node.Next = _head;
            node.Previous = null;
            if (_head != null)
            {
                _head.Previous = node;
            }

            _head = node;
            return _head;
        }

        public IMoveNode<T> InsertLast(T move)
        {
            var moveNode = new MoveNode<T>(move);
            if (_head == null)
            {
                moveNode.Previous = null;
                _head = moveNode;
                _last = _head;
                return moveNode;
            }

            var lastNode = _last;
            lastNode.Next = moveNode;
            moveNode.Previous = lastNode;
            _last = moveNode;
            return moveNode;
        }

        private void GetLastNode(out IMoveNode<T> lastNode)
        {
            var temp = HeadMove;
            while (temp.Next != null)
            {
                temp = temp.Next;
            }
            lastNode = temp;
        }

        public MoveNode<T> HeadMove => _head;
        public MoveNode<T> LastMove => _last;


        public override string ToString()
        {
            var sb = new StringBuilder();
            IMoveNode<T> current = _head;
            while (current != null)
            {
                sb.AppendLine(current.MoveData.ToString());
                current = current.Next;
            }
            return sb.ToString();
        }

        public IEnumerable<MoveNode<T>> GetNodeEnumerator()
        {
            var node = _head.IsNullNode ? _head.Next : _head;
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
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
            while (current != null && !current.IsNullNode)
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
