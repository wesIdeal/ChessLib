using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ChessLib.Core.Types.Tree;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Types.GameTree.Traversal")]

namespace ChessLib.Core.Types.GameTree.Traversal
{
    /// <summary>
    ///     "Flattens" the game move tree structure in PGN order.
    /// </summary>
    internal class PgnNode : IEnumerator<MoveInformation>
    {
        public PgnNode[] SiblingNodes { get; }
        public MoveInformation Value { get; }
        public PgnNode Next { get; }

        public PgnNode(MoveTreeNode<PostMoveState> node)
        {
            Value = new MoveInformation(node);
            var siblingCollection = new List<PgnNode>();
            foreach (var sibling in Value.SiblingVariationNodes())
            {
                siblingCollection.Add(new PgnNode(sibling));
            }

            SiblingNodes = siblingCollection.ToArray();
            if (node.Continuations.Any())
            {
                Next = new PgnNode((MoveTreeNode<PostMoveState>)node.Continuations.First());
            }
            else
            {
                Next = null;
            }
        }

        private IEnumerator<MoveInformation> enumerator;

        public bool MoveNext()
        {
            if (enumerator == null)
            {
                enumerator = Flattened().GetEnumerator();
            }

            var nextMoveExists = enumerator.MoveNext();
            if (!nextMoveExists)
            {
                enumerator = null;
                Current = null;
                return false;
            }

            return true;
        }

        public void Reset()
        {
            enumerator = null;
            Current = null;
        }

        public MoveInformation Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MoveInformation> Flattened()
        {
            if (!Value.Move.IsNullMove)
            {
                yield return Value;
            }

            foreach (var siblingNode in SiblingNodes.Concat(new[] { Next }).Where(x => x != null))
            {
                foreach (var siblingInfo in siblingNode.Flattened())
                {
                    yield return siblingInfo;
                }
            }
        }
    }

    /// <summary>
    ///     Enumerates an Initial Node's continuations in PGN order.
    /// </summary>
    internal sealed class GameToPgnEnumerator : IEnumerator<MoveInformation>
    {
        public MoveTreeNode<PostMoveState> Initial { get; }

        public GameToPgnEnumerator(MoveTreeNode<PostMoveState> initial)
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

        private MoveInformation current;
        private IEnumerator<MoveInformation> moveEnumerator;
        private PgnNode rootNode;

        public bool MoveNext()
        {
            if (rootNode == null)
            {
                rootNode = new PgnNode(Initial);
                moveEnumerator = rootNode.Flattened().GetEnumerator();
            }

            var rv = moveEnumerator?.MoveNext() ?? false;
            if (rv == false)
            {
                Reset();
                return false;
            }

            current = moveEnumerator.Current;
            return true;
        }

        public void Reset()
        {
            rootNode = null;
            current = null;
            moveEnumerator = null;
        }

        public MoveInformation Current
        {
            get
            {
                if (moveEnumerator?.Current == null)
                {
                    throw new InvalidOperationException("Game to PGN enumeration has not started. Call MoveNext.");
                }

                return moveEnumerator?.Current;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Reset();
        }
    }
}