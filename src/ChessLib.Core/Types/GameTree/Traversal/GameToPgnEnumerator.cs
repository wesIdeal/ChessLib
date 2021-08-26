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
    internal class PgnNode : IEnumerator<MovePair<MoveTreeNode<PostMoveState>>>
    {
        public PgnNode[] SiblingNodes { get; } = Array.Empty<PgnNode>();
        public MoveTreeNode<PostMoveState> Value { get; }
        public PgnNode Next { get; }

        public PgnNode(MoveTreeNode<PostMoveState> node)
        {
            Value = node;
            if (!node.IsFirstMoveOfVariation)
            {
                SiblingNodes = node.GetSiblingVariations().Select(x => new PgnNode(x)).ToArray();
            }
            Next = node.Continuations.Any() ? new PgnNode((MoveTreeNode<PostMoveState>)node.Continuations.First()) : null;
        }

        private IEnumerator<MovePair<MoveTreeNode<PostMoveState>>> enumerator;

        public bool MoveNext()
        {
            var flattenedMoves = Flattened();
            IEnumerable<MovePair<MoveTreeNode<PostMoveState>>> movePairs = GetMovePairs(flattenedMoves);
            enumerator ??= movePairs.GetEnumerator();

            var nextMoveExists = enumerator.MoveNext();
            if (!nextMoveExists)
            {
                Reset();
                return false;
            }



            return true;
        }

        public IEnumerable<MovePair<MoveTreeNode<PostMoveState>>> GetMovePairs(IEnumerable<MoveTreeNode<PostMoveState>> flattenedMoves)
        {
            var pair = new MovePair<MoveTreeNode<PostMoveState>>();
            int skip = 0;
            MoveTreeNode<PostMoveState>[] movePair;
            while ((movePair = flattenedMoves.Skip(skip).Take(2).ToArray()).Any())
            {
                foreach (var move in movePair)
                {
                    if (move.ColorMakingMove == Color.Black)
                    {
                        pair.BlackNode = move;
                        skip++;
                        yield return pair;
                        pair = new MovePair<MoveTreeNode<PostMoveState>>();
                    }
                    else
                    {
                        if (pair.WhiteNode != null)
                        {
                            skip++;
                            yield return pair;
                            pair = new MovePair<MoveTreeNode<PostMoveState>>(move, null);

                        }
                        else
                        {
                            pair.WhiteNode = move;
                        }
                    }

                    if (pair.BlackNode != null && pair.WhiteNode != null)
                    {
                        yield return pair;
                        pair = new MovePair<MoveTreeNode<PostMoveState>>();
                    }
                }
            }
        }

        public void Reset()
        {
            enumerator = null;
            Current = new MovePair<MoveTreeNode<PostMoveState>>(null, null);
        }

        public MovePair<MoveTreeNode<PostMoveState>> Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public IEnumerable<MoveTreeNode<PostMoveState>> Flattened()
        {
            if (Value.Value.MoveValue != Move.NullMove)
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
    internal sealed class GameToPgnEnumerator : IEnumerator<MovePair<PgnMoveInformation?>>
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

        private IEnumerator<MovePair<MoveTreeNode<PostMoveState>>> moveEnumerator;
        private PgnNode rootNode;



        public bool MoveNext()
        {
            if (rootNode == null)
            {
                rootNode = new PgnNode(Initial);
                var flattened = rootNode.Flattened();
                var paired = rootNode.GetMovePairs(flattened);
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

        public MovePair<PgnMoveInformation?> Current
        {
            get
            {
                if (moveEnumerator?.Current == null)
                {
                    throw new InvalidOperationException("Game to PGN enumeration has not started. Call MoveNext.");
                }

                return GetPgnMovePair((MovePair<MoveTreeNode<PostMoveState>>)moveEnumerator?.Current);
            }
        }

        private MovePair<PgnMoveInformation?> GetPgnMovePair(MovePair<MoveTreeNode<PostMoveState>> moveEnumeratorCurrent)
        {
            var whiteNode = moveEnumeratorCurrent.WhiteNode;
            var blackNode = moveEnumeratorCurrent.BlackNode;
            var white = whiteNode != null ? GetPgnMoveInformation(whiteNode) : (PgnMoveInformation?)null;
            var black = blackNode != null ? GetPgnMoveInformation(blackNode) : (PgnMoveInformation?)null;
            return new MovePair<PgnMoveInformation?>(white, black);
        }

        object IEnumerator.Current => Current;


        private PgnMoveInformation GetPgnMoveInformation(MoveTreeNode<PostMoveState> move)
        {
            if (move == null)
            {
                throw new ArgumentNullException(nameof(move));
            }

            string moveNumberWhiteSpace = string.Empty;

            var moveNumber = move.MoveNumber.ToString();
            var moveDecimal = move.ColorMakingMove == Color.White ? "." : "...";
            move.SetMoveDetailFlags();
            var pgnMove = new PgnMoveInformation(move.ColorMakingMove, move.Value.San, move.MoveFlags, moveNumber,
                moveDecimal, moveNumberWhiteSpace);
            return pgnMove;
        }

        public void Dispose()
        {
            Reset();
        }
    }

    public struct MovePair<T>
    {
        public MovePair(T whiteNode, T blackNode)
        {
            WhiteNode = whiteNode;
            BlackNode = blackNode;
        }

        public T WhiteNode { get; set; }
        public T BlackNode { get; set; }
    }
}