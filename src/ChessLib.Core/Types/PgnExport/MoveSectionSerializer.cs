using System;
using ChessLib.Core.Types.Enums;
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
                Serialize(move, pgnWriter);
            }
        }

        public void Serialize(MoveInformation move, PgnWriter pgnWriter)
        {
            var pgnMove = GetPgnMoveInformation(move);
            pgnWriter.WriteMoveObject(pgnMove);
        }

        private PgnWriter.ContinuationType GetContinuationType(MoveInformation info)
        {
            var continuationType = PgnWriter.ContinuationType.NULL;

            if (info.IsMainLineContinuation)
            {
                continuationType |= PgnWriter.ContinuationType.MainLine;
            }
            else if (info.IsFirstMoveOfVariation())
            {
                continuationType |= PgnWriter.ContinuationType.Variation;
            }

            if (info.IsLastMove)
            {
                continuationType |= PgnWriter.ContinuationType.EndVariation;
            }

            return continuationType;
        }

        /// <summary>
        ///     Gets value to determine if a PGN move number indicator is necessary.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>
        ///     <value>true</value>
        ///     if the move number should be displayed, false if not.
        /// </returns>
        internal static bool IsMoveNumberNeeded(MoveInformation info)
        {
            if (info.ActiveColor == Color.White ||
                info.IsFirstMoveOfVariation())
            {
                return true;
            }

            if (info.ParentVariations == null)
            {
                return false;
            }

            

            return false;
        }

        private PgnWriter.PgnMoveInformation GetPgnMoveInformation(MoveInformation move)
        {
            if (move == null)
            {
                throw new ArgumentNullException(nameof(move), "Move being serialized cannot be null.");
            }

            string moveNumber = string.Empty, moveDecimal = string.Empty, moveNumberWhiteSpace = string.Empty;
            if (IsMoveNumberNeeded(move))
            {
                moveNumber = move.MoveNumber.ToString();
                moveDecimal = move.ActiveColor == Color.White ? "." : "...";
                moveNumberWhiteSpace = Options.SpaceAfterMoveNumber ? Options.WhitespaceSeparator.ToString() : "";
            }

            var continuationType = GetContinuationType(move);
            var pgnMove = new PgnWriter.PgnMoveInformation(move.ActiveColor, move.San, continuationType, moveNumber,
                moveDecimal, moveNumberWhiteSpace);
            return pgnMove;
        }
    }
}