using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums.NAG;

namespace ChessLib.Core
{

    /// <summary>
    ///     A move that was applied
    /// </summary>
    public class MoveNode : IEquatable<MoveNode>
    {
        public bool Equals(MoveNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var boardStateEq = BoardStateValue == other.BoardStateValue && MoveValue == other.MoveValue;
            var moveEq = Move.Equals(other.Move);
            if (Continuations.Count != other.Continuations.Count)
            {
                return false;
            }

            var continuationZip = Continuations.Zip(other.Continuations, (c1, c2) => new
            {
                continuation1 = c1,
                continuation2 = c2,
                areEqual = c1.Equals(c2)
            });
            var continuationsEq = continuationZip.All(x => x.areEqual);
            return boardStateEq && moveEq && continuationsEq;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MoveNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)BoardStateValue * 397) ^ MoveValue.GetHashCode();
            }
        }

        public static bool operator ==(MoveNode left, MoveNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MoveNode left, MoveNode right)
        {
            return !Equals(left, right);
        }

        private MoveNode()
        {
            Continuations = new List<MoveNode>();
        }

        protected MoveNode(Move move) : this()
        {
            MoveValue = move.MoveValue;
        }

        /// <summary>
        /// Constructs a <see cref="MoveNode"/>
        /// </summary>
        /// <param name="board">Board to which <paramref name="move"/> will be applied</param>
        /// <param name="move">Applied to <paramref name="board"/> </param>
        public MoveNode(Board board, Move move) : this(move)
        {

            BoardStateHash = PolyglotHelpers.GetBoardStateHash(board);
            BoardStateValue = BoardState.BoardStateStorage;

        }


        public override string ToString()
        {
            return $"{Move} -> {BoardStateValue}";
        }
        public MoveNode(MoveNode moveNode) : this()
        {
            San = moveNode.San;
            BoardStateValue = moveNode.BoardStateValue;
            MoveValue = moveNode.MoveValue;
            Annotation = moveNode.Annotation;
            BoardStateHash = moveNode.BoardStateHash;
            PostMoveComment = moveNode.PostMoveComment;
            PreMoveComment = moveNode.PreMoveComment;
            Previous = moveNode.Previous;
            moveNode.Continuations.ForEach(x => CopyNodeTree(x, Continuations));
        }

        protected uint BoardStateValue { get; set; }
        protected ushort MoveValue { get; }

        public BoardState BoardState => new BoardState(BoardStateValue);
        protected static readonly MoveToSan MoveToSan = new MoveToSan();
        public Move Move => new Move(MoveValue);

        public string San { get; protected set; }

        public string PreMoveComment { get; set; }

        public string PostMoveComment { get; set; }

        public NumericAnnotation Annotation { get; set; }

        public ulong BoardStateHash { get; }

        public List<MoveNode> Continuations { get; private set; }

        private void CopyNodeTree(MoveNode orignalNode, List<MoveNode> continuations)
        {
            foreach (var originalContinuation in orignalNode.Continuations)
            {
                continuations.Add(new MoveNode(orignalNode));
            }
        }


        private bool IsEqualToNode(MoveNode mn)
        {
            return BoardStateValue == mn.BoardStateValue && MoveValue == mn.MoveValue;
        }


        public MoveNode FindExistingContinuation(MoveNode mn)
        {
            return Continuations.SingleOrDefault(x => IsEqualToNode(mn));
        }

        #region Navigation

        public virtual MoveNode Next => Continuations.FirstOrDefault();
        public virtual IEnumerable<MoveNode> Variations => Continuations.Skip(1);
        public IEnumerable<MoveNode> MainLine
        {
            get
            {
                var previousNode = this;
                MoveNode node = (MoveNode)this;
                while ((node = node.Next) != null)
                {
                    yield return node;
                }
            }
        }
        #endregion Navigation

        public virtual MoveNode Previous { get; private set; }
        public MoveNode AddContinuation(MoveNode moveNode)
        {
            var existingEquivalentNode = FindExistingContinuation(moveNode);
            if (existingEquivalentNode != null)
            {
                return existingEquivalentNode;
            }
            moveNode.Previous = this;
            Continuations.Add(moveNode);
            return Continuations.Last();
        }
    }
}