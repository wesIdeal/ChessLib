using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.PgnExport;

namespace ChessLib.Core.Types.GameTree.Traversal
{
    public  struct MovePair
    {
        private PgnMoveInformation?[] _movePairs;
        public PgnMoveInformation?[] MovePairs => _movePairs ??= new PgnMoveInformation?[2];
        internal const string EmptyNodeIndicator = "[empty]";
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            var white = WhiteNode.ToString() ?? EmptyNodeIndicator;
            var black = BlackNode.ToString() ?? EmptyNodeIndicator;
            return $"<{white} {black}>";
        }

        public bool IsEmpty => WhiteNode == null && BlackNode == null;
        public bool IsFull => WhiteNode != null && BlackNode != null;


        /// <summary>
        ///     Places nodes in the correct locations
        /// </summary>
        /// <param name="nodes">Nodes to convert to MovePair object</param>
        public MovePair(params PgnMoveInformation?[] nodes)
        {
            if (nodes?.Length > 2)
            {
                throw new ArgumentException("Cannot send more than 2 nodes to MovePair constructor.");
            }

            _movePairs = new PgnMoveInformation?[2];

            
            if (nodes != null)
            {
                foreach (var node in nodes.Where(x => x.HasValue))
                {
                    MovePairs[(int)node.Value.ColorMakingMove] = node;
                }
            }
        }

        public PgnMoveInformation? WhiteNode
        {
            get => MovePairs[(int)Color.White];
            set => SetNode(value, (int)Color.White);
        }

        public PgnMoveInformation? BlackNode
        {
            get => MovePairs[(int)Color.Black];
            set => SetNode(value, (int)Color.Black);
        }

        private void SetNode(PgnMoveInformation? value, int colorIndex)
        {
            MovePairs[colorIndex] = value;
        }
    }
}