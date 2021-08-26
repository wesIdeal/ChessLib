using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Types.Tree;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Types")]

namespace ChessLib.Core.Types
{
    public class MoveInformation : IEquatable<MoveInformation>, ICloneable
    {
        /// <summary>
        ///     Gets the full whole-move count of this move.
        /// </summary>
        public uint MoveNumber => (uint)(((BoardState)PostMoveState.Value.BoardState).FullMoveCounter -
                                         (ActiveColor == Color.Black ? 1 : 0));

        

        

        /// <summary>
        ///     Get the active player's color for this move.
        /// </summary>
        public Color ActiveColor => ((BoardState)PreMoveBoardState.Value.BoardState).ActivePlayer;

        public MoveTreeNode<PostMoveState> PostMoveState { get; }
        public MoveTreeNode<PostMoveState> PreMoveBoardState { get; }
        public Move Move => PostMoveState.Value.MoveValue;
        public Move ParentMove => PreMoveBoardState?.Value.MoveValue;

        

        public string San => PostMoveState.Value.San;

        public virtual IEnumerable<MoveTreeNode<PostMoveState>> CurrentContinuations =>
            PostMoveState.Continuations.Cast<MoveTreeNode<PostMoveState>>();

        /// <summary>
        ///     Gets the next main line continuation, or
        ///     <value>null</value>
        ///     if none exist.
        /// </summary>
        public MoveTreeNode<PostMoveState> NextMove => CurrentContinuations.FirstOrDefault();

        

        

        /// <summary>
        ///     Get the parent move's variations. Useful to know if a PGN move number should be written
        /// </summary>
        public Move[] ParentVariations
        {
            get
            {
                return PreMoveBoardState?
                           .Previous?
                           .Previous?
                           .Continuations
                           .Select(x => (Move)x.Value.MoveValue)
                           .ToArray() ??
                       Array.Empty<Move>();
            }
        }

        public string Comment => PostMoveState.Comment;
        public NumericAnnotation Annotation => PostMoveState.Annotation;

        public bool IsFirstMove =>
            ParentMove.IsNullMove && PreMoveBoardState.Continuations.First().Value.MoveValue == Move;

        public MoveInformation(MoveTreeNode<PostMoveState> postMoveTreeNode)
        {
            PreMoveBoardState = (MoveTreeNode<PostMoveState>)postMoveTreeNode.Previous;
            PostMoveState = postMoveTreeNode;
        }

        protected MoveInformation()
        {
            //for unit test mocking
        }

     
        public object Clone()
        {
            return new MoveInformation((MoveTreeNode<PostMoveState>)PostMoveState.Clone());
        }

        public bool Equals(MoveInformation other)
        {
            return other != null &&
                   PostMoveState.Equals(other.PostMoveState);
        }

        

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(San) ? Move.ToString() : San;
        }


        
    }
}