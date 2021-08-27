using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        public MoveTreeNode<PostMoveState> Value { get; }
        public PgnNode Next { get; }
        public int VariationDepth { get; }

        public PgnNode(MoveTreeNode<PostMoveState> treeNode)
        {
            Debug.Assert(treeNode != null);

            var node = (MoveTreeNode<PostMoveState>)treeNode.Clone();
            VariationDepth = treeNode.VariationDepth;
            Value = node;
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
            var nodeQueue = new Queue<MoveTreeNode<PostMoveState>>(flattenedTreeNodes);
            var lastLevel = 0;
            MoveTreeNode<PostMoveState> previous = null;
            while (nodeQueue.TryDequeue(out var moveNode))
            {
                var currentDepth = moveNode.VariationDepth;
                var hasNextNode = nodeQueue.TryPeek(out var nextNodeInQueue);
                var depthDifferencePrevious = GetPreviousDepthDifference(moveNode, previous, lastLevel);
                var depthDifferenceNext = GetNextDepthDifference(moveNode, nextNodeInQueue, currentDepth);

                var node = new PgnMoveInformation(moveNode.ColorMakingMove, moveNode.Value.San, moveNode.MoveNumber,
                    moveNode.IsFirstMoveInGame, !hasNextNode, depthDifferencePrevious, depthDifferenceNext,
                    moveNode.Comment, moveNode.Annotation);
                lastLevel = currentDepth;

                previous = moveNode;

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

                if ((node.VariationDepthFromNext != 0 || node.VariationDepthFromPrevious != 0 || node.IsLastMove) &&
                    !pair.IsEmpty)
                {
                    yield return pair;
                    pair = new MovePair(null);
                }
            }
        }

        private static int GetPreviousDepthDifference(MoveTreeNode<PostMoveState> currentNode,
            MoveTreeNode<PostMoveState> previousNode, int currentDepth)
        {
            if (previousNode == null)
            {
                return 0;
            }

            var depthDiff = currentNode.VariationDepth - currentDepth;
            if (depthDiff == 0 &&
                previousNode.ColorMakingMove == currentNode.ColorMakingMove &&
                currentNode.IsFirstMoveOfVariation)
            {
                return 1;
            }

            return depthDiff;
        }

        private static int GetNextDepthDifference(MoveTreeNode<PostMoveState> currentNode,
            MoveTreeNode<PostMoveState> nextNodeInQueue, int currentDepth)
        {
            if (nextNodeInQueue == null)
            {
                return -currentDepth;
            }

            var diff = nextNodeInQueue.VariationDepth - currentDepth;
            if (diff == 0 &&
                nextNodeInQueue.ColorMakingMove == currentNode.ColorMakingMove &&
                currentNode.IsLastMoveOfContinuation)
            {
                return -1;
            }

            return diff;
        }

        public IEnumerable<MoveTreeNode<PostMoveState>> FlattenToPgnOrder()
        {
            if (!string.IsNullOrWhiteSpace(Value.Value.San))
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