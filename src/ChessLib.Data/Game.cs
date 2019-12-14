using System;
using System.Collections.Generic;
using System.Threading;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Data
{
    public class Game<TMove> : MoveTraversalService
        where TMove : MoveExt, IEquatable<TMove>
    {
        public Game() : base(FENHelpers.FENInitial)
        {

            TagSection = new Tags(OnFenChanged);
            TagSection.SetFen(FENHelpers.FENInitial);

        }

        public Game(Tags tags) : base(tags.FENStart)
        {
            TagSection = tags;
            InitialFen = tags.FENStart;
            TagSection.OnFenChanged += OnFenChanged;
        }

        public Game(string fen) : base(fen)
        {
            TagSection = new Tags(OnFenChanged);
            TagSection.SetFen(fen);
        }

        public Tags TagSection { get; set; }
        public string Result { get; set; }

        private void OnFenChanged(string fen)
        {
            InitialFen = fen;
        }

        public Game<MoveStorage> SplitFromCurrentPosition(bool copyVariations = false)
        {
            return SplitFromMove(CurrentMoveNode);
        }

        public Game<MoveStorage> SplitFromMove(LinkedListNode<MoveStorage> move, bool copyVariations = false)
        {
            var currentFen = CurrentFEN;
            var moveStack = new Stack<MoveStorage>();
            LinkedListNode<MoveStorage> currentMove = move;
            moveStack.Push(currentMove.Value);
            while (!(currentMove = GetPreviousNode(currentMove)).Value.IsNullMove)
            {
                moveStack.Push(currentMove.Value);
            }
            var g = new Game<MoveStorage>(this.InitialFen);

            while (moveStack.Count > 0)
            {
                var moveStorage = moveStack.Pop();
                g.ApplyValidatedMove(moveStorage);
            }
            g.GoToInitialState();
            return g;
        }

        public bool IsEqualTo(Game<MoveStorage> otherGame, bool includeVariations = false)
        {
            if (otherGame.PlyCount != PlyCount)
            {
                return false;
            }

            return ParseTreesForEquality(this.MainMoveTree.First, otherGame.MainMoveTree.First, includeVariations);

        }

        private bool ParseTreesForEquality(LinkedListNode<MoveStorage> gNode, LinkedListNode<MoveStorage> otherNode, bool includeVariations)
        {
            var areEqual = true;
            if (gNode.Value.Move != otherNode.Value.Move)
            {
                return false;
            }
            else
            {
                LinkedListNode<MoveStorage> moveNode = gNode.Next;
                LinkedListNode<MoveStorage> otherMoveNode = otherNode.Next;
                while ((moveNode != null))
                {
                    if (otherMoveNode == null)
                    {
                        areEqual = false;
                    }
                    else
                    {
                        areEqual &= moveNode.Value.Move == otherMoveNode.Value.Move;
                    }

                    if (!areEqual)
                    {
                        break;
                    }

                    if (includeVariations)
                    {
                        if (moveNode.Value.Variations.Count != otherMoveNode.Value.Variations.Count)
                        {
                            areEqual = false;
                        }
                        else
                        {
                            for (var variationIndex = 0; variationIndex < moveNode.Value.Variations.Count; variationIndex++)
                            {
                                var variation = moveNode.Value.Variations[variationIndex];
                                var otherVariation = otherMoveNode.Value.Variations[variationIndex];
                                areEqual &= ParseTreesForEquality(variation.First, otherVariation.First,
                                    true);
                            }
                        }
                    }
                    moveNode = moveNode.Next;
                    otherMoveNode = otherMoveNode.Next;
                }
            }
            return areEqual;
        }

        public void SetFEN(string fen)
        {
            OnFenChanged(fen);
        }
    }
}