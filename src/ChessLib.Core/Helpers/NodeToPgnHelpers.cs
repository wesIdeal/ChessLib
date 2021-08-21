using System.Linq;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core.Helpers
{
    public static class NodeToPgnHelpers
    {
        /// <summary>
        ///     Gets PGN-ready SAN for building PGN game
        /// </summary>
        /// <param name="node"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ToPgn(this MoveTreeNode<PostMoveState> node, PGNFormatterOptions options)
        {
            var info = new MoveInformation(node);
            return info.ToPgn(options);
        }

        internal static string ToPgn(this MoveInformation info, PGNFormatterOptions options)
        {
            if (info.IsInitialBoard)
            {
                return "";
            }

            var moveNumber = "";
            if (IsPgnMoveNeeded(info))
            {
                moveNumber = info.GetPgnMoveNumberRepresentation();
                moveNumber += GetSeparator(options);
            }

            var moveText = info.San;
            return $"{moveNumber}{moveText}";
        }

        /// <summary>
        ///     Gets full PGN representation, regardless of need.
        /// </summary>
        /// <param name="previousNode"></param>
        /// <param name="options"></param>
        /// <returns>Full PGN entry of SAN for node.</returns>
        public static string GetPgnRepresentation(this MoveTreeNode<PostMoveState> previousNode,
            PGNFormatterOptions options)
        {
            var info = new MoveInformation(previousNode);
            var moveNumber = previousNode.GetPgnMoveNumberRepresentation();
            var decimalPoint = GetDecimal(info);
            var separator = GetSeparator(options);
            var moveText = info.San;
            return $"{moveNumber}{decimalPoint}{separator}{moveText}";
        }

        private static string GetSeparator(PGNFormatterOptions options)
        {
            var separator = options.SpaceAfterMoveNumber ? " " : string.Empty;
            return separator;
        }


        private static string GetPgnMoveNumberRepresentation(this MoveInformation info)
        {
            var moveNumber = info.MoveNumber.ToString() + GetDecimal(info);
            return moveNumber;
        }

        /// <summary>
        ///     Gets the PGN move number, alone.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>No decimal separators are included.</remarks>
        public static string GetPgnMoveNumberRepresentation(this MoveTreeNode<PostMoveState> node)
        {
            var info = new MoveInformation(node);
            return info.GetPgnMoveNumberRepresentation();
        }

        private static string GetDecimal(MoveInformation info)
        {
            return info.ActiveColor == Color.Black ? "..." : ".";
        }

        /// <summary>
        ///     Gets value to determine if a PGN move number indicator is necessary.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        ///     <value>true</value>
        ///     if the move number should be displayed, false if not.
        /// </returns>
        public static bool IsPgnMoveNeeded(this MoveTreeNode<PostMoveState> node)
        {
            return IsPgnMoveNeeded(new MoveInformation(node));
        }

        /// <summary>
        ///     Gets value to determine if a PGN move number indicator is necessary.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>
        ///     <value>true</value>
        ///     if the move number should be displayed, false if not.
        /// </returns>
        private static bool IsPgnMoveNeeded(this MoveInformation info)
        {
            if (info.Move.IsNullMove)
            {
                return false;
            }

            if (info.ActiveColor == Color.White)
            {
                return true;
            }

            if (info.IsFirstMoveOfVariation())
            {
                return true;
            }

            if (info.ParentVariations != null)
            {
                if (info.ParentVariations.Length > 1)
                {
                    var parentMoveValue = info.ParentMove;
                    //and this node is the main line of the last
                    if (info.ParentVariations.First() == parentMoveValue.MoveValue)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}