using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.PgnExport;
using ChessLib.Core.Types.Tree;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Types.GameTree.Traversal")]

namespace ChessLib.Core.Types.GameTree.Traversal
{
    /// <summary>
    ///     "Flattens" the game move tree structure in PGN order.
    /// </summary>
    internal class PgnNode
    {
        public PgnNode[] SiblingNodes { get; } = Array.Empty<PgnNode>();
        public PgnMoveInformation Value { get; }
        public PgnNode Next { get; }

        public PgnNode(MoveTreeNode<PostMoveState> treeNode)
        {
            var node = (MoveTreeNode<PostMoveState>)treeNode.Clone();
            Value = new PgnMoveInformation(node.ColorMakingMove, node.Value.San,
                node.MoveNumber.ToString(), node.IsFirstMoveInGame, node.IsLastMoveOfContinuation, node.VariationDepth, node.IsFirstMoveOfVariation);
            if (!node.IsFirstMoveOfVariation)
            {
                SiblingNodes = node.GetSiblingVariations().Select(x => new PgnNode(x)).ToArray();
            }

            Next = node.Continuations.Any()
                ? new PgnNode((MoveTreeNode<PostMoveState>)node.Continuations.First())
                : null;
        }

        public IEnumerable<MovePair> GetMovePairs()
        {
            var pair = new MovePair(null);
            var flattenedTreeNodes = FlattenToPgnOrder();
            var nodeQueue = new Queue<PgnMoveInformation>(flattenedTreeNodes);


            while (nodeQueue.TryDequeue(out var node))
            {
                if (node.ColorMakingMove == Color.White)
                {
                    if (!pair.IsEmpty)
                    {
                        yield return pair;
                    }

                    pair = new MovePair(node);
                }
                else
                {

                    pair.BlackNode = node;
                    yield return pair;
                    pair = new MovePair(null);
                }

                if (node.IsLastMove)
                {
                    yield return pair;
                    pair = new MovePair(null);
                }
            }
        }

        public IEnumerable<PgnMoveInformation> FlattenToPgnOrder()
        {
            if (!string.IsNullOrWhiteSpace(Value.MoveSan))
            {
                yield return Value;
            }

            foreach (var siblingNode in SiblingNodes.Concat(new[] { Next }).Where(x => x != null))
            {
                foreach (var siblingInfo in siblingNode.FlattenToPgnOrder())
                {
                    yield return siblingInfo;
                }
            }
        }
    }

    /// <summary>
    ///     Enumerates an Initial Node's continuations in PGN order.
    /// </summary>
    internal sealed class GameToPgnEnumerator : PgnNode, IEnumerator<MovePair>
    {
        public MoveTreeNode<PostMoveState> Initial { get; }

        public GameToPgnEnumerator(MoveTreeNode<PostMoveState> initial) : base(initial)
        {
            var startingNode = (MoveTreeNode<PostMoveState>)initial.Clone();
            if (startingNode.Value.MoveValue == Move.NullMove && startingNode.Continuations.Any())
            {
                startingNode = startingNode.Next;
            }

            Initial = (MoveTreeNode<PostMoveState>)startingNode.Clone();

            Reset();
        }


        public GameToPgnEnumerator(Game game) : this(game.InitialNode.Node)
        {
        }

        private IEnumerator<MovePair> moveEnumerator;
        private PgnNode rootNode;


        public bool MoveNext()
        {
            if (rootNode == null)
            {
                rootNode = new PgnNode(Initial);
                var paired = rootNode.GetMovePairs();
                moveEnumerator = paired.GetEnumerator();
            }

            var rv = moveEnumerator?.MoveNext() ?? false;
            if (rv == false)
            {
                Reset();
                return false;
            }

            return true;
        }

        public void Reset()
        {
            rootNode = null;
            moveEnumerator = null;
        }

        public MovePair Current
        {
            get
            {
                if (moveEnumerator?.Current == null)
                {
                    throw new InvalidOperationException("Game to PGN enumeration has not started. Call MoveNext.");
                }

                return moveEnumerator.Current;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Reset();
        }
    }
}