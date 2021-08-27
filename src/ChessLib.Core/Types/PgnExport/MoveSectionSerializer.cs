using ChessLib.Core.Types.GameTree.Traversal;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core.Types.PgnExport
{
    internal class MoveSectionSerializer : PgnSerializer
    {
        public MoveSectionSerializer(PGNFormatterOptions options) : base(options)
        {
            
        }

        /// <summary>
        ///     Writes move section in PGN form.
        /// </summary>
        /// <param name="node">The starting node.</param>
        /// <param name="pgnWriter">The writer used to write the information.</param>
        public void Serialize(MoveTreeNode<PostMoveState> node, PgnWriter pgnWriter)
        {
            var enumerator = new GameToPgnEnumerator(node);

            while (enumerator.MoveNext())
            {
                var move = enumerator.Current;
                pgnWriter.WriteMove(move);
            }
        }
    }
}