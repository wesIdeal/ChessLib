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
        where T : MoveExt
    {
        private MoveNode<T> _last;
        private MoveNode<T> _head;

        public MoveNode<T> VariationParent { get; internal set; }


        public MoveTree([ItemNotNull]MoveNode<T> parentMove)
        {
            VariationParent = parentMove;
            if (VariationParent == null)
            {

                _head = MoveNode<T>.GetNullNode();
                _last = _head;
            }
        }

        /// <summary>
        /// Adds a move at the end of the tree.
        /// </summary>
        /// <param name="move"></param>
        /// <returns>the added move's node</returns>
        public MoveNode<T> AddMove(T move)
        {
            return InsertLast(move);
        }

        public MoveNode<T> AddVariation(MoveNode<T> variationOfNode, T move)
        {
            variationOfNode.Variations.Add(new MoveTree<T>(variationOfNode));
            return variationOfNode.Variations.Last().AddMove(move);
        }

        public MoveNode<T> InsertFirst(T move)
        {
            var currentFirst = _head;
            var node = new MoveNode<T>(move, currentFirst.ParentTreeMove);
            _head.ParentTreeMove = null;
            node.Next = _head;
            node.Previous = null;
            if (_head != null)
            {
                _head.Previous = node;
            }

            _head = node;
            return _head;
        }

        /// <summary>
        /// Adds a move at the end of the tree.
        /// </summary>
        /// <param name="move"></param>
        /// <returns>the added move's node</returns>
        public MoveNode<T> InsertLast(T move)
        {
            if (_last == null)
            {
                _head = new MoveNode<T>(move, VariationParent);
                _head.ParentTreeMove = VariationParent;
                _head.Previous = null;
                _last = _head;

            }
            else
            {
                var nextMove = _last.AddNextMove(move, VariationParent);
                _last = nextMove;
            }
            return _last;
        }

        private void GetLastNode(out MoveNode<T> lastNode)
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
            MoveNode<T> current = _head;
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

        public static MoveNode<T> FindVariationParentNode(in MoveNode<T> fromNode)
        {
            var currentNode = fromNode;
            while (currentNode.Previous != null && !currentNode.IsNullNode)
            {
                currentNode = currentNode.Previous;
            }
            var rvNode = currentNode.ParentTreeMove;
            if (rvNode == null) { return null; }
            if (rvNode.Previous.IsNullNode) return null;
            return rvNode?.Previous;
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
                current = current.Previous ?? current.ParentTreeMove;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
