using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.Tree
{
    public class MoveTreeNode<T> : NodeBase<T>, IEquatable<MoveTreeNode<T>> where T : IPostMoveState
    {
        public string Comment { get; internal set; } = string.Empty;
        public NumericAnnotation Annotation { get; } = new NumericAnnotation();

        public bool IsFirstMoveOfAContinuation => Previous?.Continuations.Skip(1).ToList().Any(x=>x.Value.Equals(Value)) ?? false;

        public bool IsVariation => VariationDepth != 0;
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


        public MoveTreeNode(T value, INode<T> parent) : base(value, parent)
        {
        }

        internal MoveTreeNode(MoveTreeNode<T> value, MoveTreeNode<T> parent) : base(value, parent)
        {
            Comment = value.Comment;
            Annotation = value.Annotation;
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
            var node = new MoveTreeNode<T>(this, (MoveTreeNode<T>)Previous);
            return node;
        }

        #region MoveInformation

        public bool IsFirstMoveOfVariation =>
            IsVariation && IsFirstMoveOfAContinuation;

        /// <summary>
        ///     Is the current node the main continuation from parent.
        /// </summary>
        public bool IsMainLineContinuation

        {
            get
            {
                var parentMainLine = Previous?
                    .Continuations
                    .FirstOrDefault()?
                    .Value.MoveValue == Value.MoveValue;
                return parentMainLine;
            }
        }


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

        public bool IsFirstMoveInGame => Previous?.Value.MoveValue == Move.NullMove &&
                                         Previous?.Continuations[0].Value.MoveValue == Value.MoveValue;

        public virtual bool IsLastMoveOfContinuation => Continuations.FirstOrDefault() == null;

        public virtual bool IsLastMoveOfGame => IsLastMoveOfContinuation && VariationDepth == 0;

        public bool IsInitialMoveOfContinuation => Previous?.Value.MoveValue == Move.NullMove ||
                                                   Previous?.Continuations[0].Value.MoveValue != Value.MoveValue;
        

        #endregion MoveInformation
    }
}