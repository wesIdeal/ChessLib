//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace ChessLib.Core.Types
//{
//    /// <summary>
//    ///     A class that holds a database containing a chess game.
//    /// </summary>
//    public class MoveTree : LinkedList<MoveNode>, IEquatable<MoveTree>
//    {
//        internal MoveTree(MoveTree moveTree)
//        {
//            CopyTree(moveTree);
//            var clonedParent = (GameMove) moveTree.VariationParentNode.Value.Clone();
//            VariationParentNode = new LinkedListNode<GameMove>(clonedParent);
//            GameComment = moveTree.GameComment;
//        }

//        /// <summary>
//        ///     Creates a new tree of moves under the parent <paramref name="parentVariation" />.
//        /// </summary>
//        /// <param name="parentVariation">The variation from which these moves begin.</param>
//        /// <param name="move">A move to apply as a first move to the <paramref name="parentVariation" />'s board.</param>
//        public MoveTree(LinkedListNode<GameMove> parentVariation)
//        {
//            VariationParentNode = parentVariation;
//        }

//        /// <summary>
//        ///     Creates a new MoveTree from a board.
//        /// </summary>
//        /// <remarks>This should be used for the start of the game (initial board)</remarks>
//        /// <param name="initialBoard">The board loaded into the property <see cref="InitialBoard" /></param>
//        public MoveTree(Board initialBoard)
//            : this(new LinkedListNode<GameMove>(new GameMove(initialBoard)))
//        {
//        }

//        public Board InitialBoard => VariationParentNode.Value.Board;


//        public bool HasGameComment => !string.IsNullOrEmpty(GameComment);
//        public string GameComment { get; set; }

//        //public MoveNode<T> VariationParent { get; internal set; }

//        /// <summary>
//        ///     The parent to the current variation
//        /// </summary>
//        public LinkedListNode<GameMove> VariationParentNode { get; }

//        public string StartingFen => VariationParentNode?.Value?.Board.CurrentFEN;


//        public bool Equals(MoveTree other)
//        {
//            return Equals(other, true);
//        }

//        public bool Equals(MoveTree other, bool checkVariations)
//        {
//            if (ReferenceEquals(null, other)) return false;
//            if (ReferenceEquals(this, other)) return true;
//            var gameCommentEq = GameComment == other.GameComment;
//            var variationParentNodeEq = VariationParentNode.Value.Equals(other.VariationParentNode.Value);
//            var startingFenEq = StartingFen == other.StartingFen;
//            var variationsEq = CheckVariationsEquality(VariationParentNode.Value.Variations,
//                other.VariationParentNode.Value.Variations);
//            var treeEqual = CheckEquality(other, checkVariations);
//            return gameCommentEq && variationParentNodeEq && startingFenEq && treeEqual && variationsEq;
//        }

//        private static bool CheckVariationsEquality(List<MoveTree> v1, List<MoveTree> v2)
//        {
//            var areEqual = true;
//            if (!(areEqual &= v1.Count == v2.Count))
//            {
//                areEqual = false;
//            }
//            else
//            {
//                foreach (var (variation1, variation2) in v1.Zip(v2, Tuple.Create))
//                {
//                    if (!(areEqual &= CheckEquality(variation1, variation2, true)))
//                    {
//                        break;
//                    }
//                }
//            }

//            return areEqual;
//        }

//        private void CopyTree(MoveTree moveTree)
//        {
//            foreach (var node in moveTree.AsQueryable())
//            {
//                AddLast(new GameMove(node));
//            }
//        }

//        /// <summary>
//        ///     Adds a move at the end of the tree.
//        /// </summary>
//        /// <param name="move"></param>
//        /// <returns>the added move's node</returns>
//        public LinkedListNode<GameMove> AddMove(GameMove move)
//        {
//            return AddLast(move);
//        }

//        public override string ToString()
//        {
//            var parent = VariationParentNode;
//            var depth = 0;
//            while (parent != null)
//            {
//                depth++;
//                parent = ((MoveTree) parent.List).VariationParentNode;
//            }

//            var rv = $"Variation Depth: {depth}{Environment.NewLine}{StartingFen}";
//            return rv;
//        }


//        public bool CheckEquality(MoveTree other, bool checkVariations)
//        {
//            return CheckEquality(this, other, checkVariations);
//        }

//        protected static bool CheckEquality(MoveTree tree1, MoveTree tree2, bool checkVariations)
//        {
//            var areEqual = true;

//            foreach (var (board1, board2) in tree1.Zip(tree2))
//            {
//                if (!(areEqual &= board1.Equals(board2)))
//                {
//                    break;
//                }

//                if (checkVariations)
//                {
//                    areEqual &= CheckVariationsEquality(board1.Variations, board2.Variations);
//                }
//            }

//            return areEqual;
//        }


//        public override bool Equals(object obj)
//        {
//            if (ReferenceEquals(null, obj)) return false;
//            if (ReferenceEquals(this, obj)) return true;
//            if (obj.GetType() != GetType()) return false;
//            return Equals((MoveTree) obj);
//        }

//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                var hashCode = GameComment != null ? GameComment.GetHashCode() : 0;
//                hashCode = (hashCode * 397) ^ (VariationParentNode != null ? VariationParentNode.GetHashCode() : 0);
//                hashCode = (hashCode * 397) ^ (StartingFen != null ? StartingFen.GetHashCode() : 0);
//                return hashCode;
//            }
//        }

//        public static bool operator ==(MoveTree left, MoveTree right)
//        {
//            return Equals(left, right);
//        }

//        public static bool operator !=(MoveTree left, MoveTree right)
//        {
//            return !Equals(left, right);
//        }
//    }
//}

