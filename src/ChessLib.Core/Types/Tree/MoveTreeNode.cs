using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Types.PgnExport;

namespace ChessLib.Core.Types.Tree
{
    public class MoveTreeNode<T> : NodeBase<T>, IEquatable<MoveTreeNode<T>> where T : IPostMoveState
    {
        public string Comment { get; internal set; } = string.Empty;
        public NumericAnnotation Annotation { get; } = new NumericAnnotation();

        public bool IsFirstMoveOfAContinuation => (MoveFlags & GameMoveFlags.InitialMove) != 0;

        public MoveTreeNode<T> Next => (MoveTreeNode<T>)Continuations.FirstOrDefault();

        /// <summary>
        ///     Gets the full whole-move count of this move.
        /// </summary>
        /// <remarks>
        ///     If the last move was from <see cref="Color.Black" />, then the move is actually the board state move minus
        ///     one.
        /// </remarks>
        public uint MoveNumber =>
            (uint)(((BoardState)Value.BoardState).FullMoveCounter - (ColorMakingMove == Color.Black ? 1 : 0));

        /// <summary>
        ///     <see cref="Color" /> of side making the <see cref="Move" /> that resulted in <see cref="BoardState" />.
        /// </summary>
        public Color ColorMakingMove => ((BoardState)Value.BoardState).ActivePlayer.Toggle();

        public GameMoveFlags MoveFlags { get; private set; } = GameMoveFlags.NullMove;

        public IEnumerable<MoveTreeNode<T>> ParentContinuations =>
            Previous?.Previous?.Continuations.Cast<MoveTreeNode<T>>();

        public MoveTreeNode(T value, INode<T> parent) : base(value, parent)
        {
            SetMoveDetailFlags();
        }

        internal MoveTreeNode(MoveTreeNode<T> value, MoveTreeNode<T> parent) : base(value, parent)
        {
            Comment = value.Comment;
            Annotation = value.Annotation;
            SetMoveDetailFlags();
        }

        public bool Equals(MoveTreeNode<T> other)
        {
            return base.Equals(other);
        }

        /// <summary>
        ///     Gets sibling continuations which are not the mainline
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MoveTreeNode<T>> GetSiblingVariations()
        {
            var siblings = Previous?.Continuations
                .Skip(1)
                .Where(x => x.Value.MoveValue != Value.MoveValue);
            return siblings?.Cast<MoveTreeNode<T>>() ?? new List<MoveTreeNode<T>>();
        }

        public void SetMoveDetailFlags()
        {
            if (Value.MoveValue == Move.NullMove)
            {
                MoveFlags = GameMoveFlags.NullMove;
                return;
            }

            SetMainlineContinuationFlag();

            SetInitialMoveFlag();

            SetLastMoveFlag();

            SetVariationMarker();
        }

        private void SetVariationMarker()
        {
            if (VariationDepth != 0)
            {
                MoveFlags |= GameMoveFlags.Variation;
            }
        }

        private void SetLastMoveFlag()
        {
            if (IsLastMoveOfContinuation)
            {
                MoveFlags |= GameMoveFlags.LastMoveOfContinuation;
            }
            else
            {
                MoveFlags &= ~(GameMoveFlags.LastMoveOfContinuation);
            }
        }


        private void SetInitialMoveFlag()
        {
            if (Previous == null)
            {
                return;
            }

            var initial = Previous.Value.MoveValue == Move.NullMove;

            var continuations = Previous.Continuations.ToArray();
            if (continuations.Any() && continuations.First().Value.MoveValue != Value.MoveValue)
            {
                initial = true;
            }

            if (initial)
            {
                MoveFlags |= GameMoveFlags.InitialMove;
            }
        }

        private void SetMainlineContinuationFlag()
        {
            var parentMainLine = Previous?
                .Continuations
                .FirstOrDefault()?
                .Value.MoveValue == Value.MoveValue;
            if (parentMainLine)
            {
                MoveFlags |= GameMoveFlags.MainLine;
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        ///     Clones a <see cref="MoveTreeNode{T}" /> using the current <see cref="INode{T}.Previous" /> as the parent node.
        /// </summary>
        /// <returns>A new MoveTreeNode that is a copy of the current.</returns>
        public override object Clone()
        {
            return new MoveTreeNode<T>(this, (MoveTreeNode<T>)Previous);
        }

        #region MoveInformation

        public bool IsFirstMoveOfVariation =>
            (MoveFlags & GameMoveFlags.BeginVariation) == GameMoveFlags.BeginVariation;

        /// <summary>
        ///     Is the current node the main continuation from parent.
        /// </summary>
        public bool IsMainLineContinuation => (MoveFlags & GameMoveFlags.Variation) != GameMoveFlags.Variation;


        public int VariationDepth
        {
            get
            {
                var nodeIterator = (INode<PostMoveState>)this;
                var moveIterator = Value.MoveValue;
                var count = 0;

                while ((nodeIterator = nodeIterator.Previous) != null)
                {
                    var continuations = nodeIterator.Continuations?.FirstOrDefault();
                    if (continuations != null && continuations.Value.MoveValue != moveIterator)
                    {
                        count++;
                    }

                    moveIterator = nodeIterator.Value.MoveValue;
                }

                return count;
            }
        }

        public bool IsFirstMoveInGame =>
            (MoveFlags & GameMoveFlags.InitialMove) != 0 && IsMainLineContinuation;

        public virtual bool IsLastMoveOfContinuation => Continuations.FirstOrDefault() == null;

        public virtual bool IsLastMoveOfGame => IsLastMoveOfContinuation && VariationDepth == 0;

        #endregion MoveInformation
    }
}