using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.Tree;
using EnumsNET;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Types")]

namespace ChessLib.Core.Types
{
    internal readonly struct MoveInformation
    {
        public MoveInformation(MoveTreeNode<PostMoveState> postMoveTreeNode)
        {
            PreMoveBoardState = (MoveTreeNode<PostMoveState>)postMoveTreeNode.Previous;
            PostMoveState = postMoveTreeNode;
        }


        public uint MoveNumber => (uint)(((BoardState)PostMoveState.Value.BoardState).FullMoveCounter -
                                         (ActiveColor == Color.Black ? 1 : 0));

        public bool IsInitialBoard => Move.IsNullMove;
        public Color ActiveColor => ((BoardState)PreMoveBoardState.Value.BoardState).ActivePlayer;

        public bool IsFirstMoveOfVariation()
        {
            var moveInfo = this;
            var any = !moveInfo.IsMainLine;
            return any;
        }

        public MoveTreeNode<PostMoveState> PostMoveState { get; }
        public MoveTreeNode<PostMoveState> PreMoveBoardState { get; }
        public Move Move => PostMoveState.Value.MoveValue;
        public Move ParentMove => PreMoveBoardState?.Value.MoveValue;
        public bool IsMainLine => PreMoveBoardState.Continuations.First().Value.MoveValue == Move;
        public string San => PostMoveState.Value.San;

        public override string ToString()
        {
            return San;
        }

        /// <summary>
        ///     Gets sibling continuations which are not the mainline
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MoveTreeNode<PostMoveState>> SiblingVariationNodes()
        {
            var moveInfo = this;
            return moveInfo.PreMoveBoardState?.Continuations
                .Skip(1)
                .Cast<MoveTreeNode<PostMoveState>>()
                .Where(x=>x.Value.MoveValue != moveInfo.Move);
        }

        public Move[] SiblingVariations => SiblingVariationNodes()
            .Select(x => (Move)x.Value.MoveValue).ToArray();

        public Move[] ParentVariations =>
            PreMoveBoardState?.Previous?.Continuations.Select(x => (Move)x.Value.MoveValue).ToArray();

        public string Comment => PostMoveState.Comment;
        public NumericAnnotation Annotation => PostMoveState.Annotation;
    }

    public class PgnFormatter
    {
        private string ResultMoveSeparator => options.ResultOnNewLine ? options.NewLine : " ";

        public PgnFormatter(PGNFormatterOptions options)
        {
            this.options = options;
        }

        private readonly PGNFormatterOptions options;

        public string BuildPgn(Game game)
        {
            var gameClone = (Game)game.Clone();
            var tagSection = BuildTags(gameClone.Tags);
            var treeRoot = gameClone.InitialNode;
            var moveSection = BuildMoveTree(treeRoot.Node);
            return tagSection + options.NewLine + moveSection + ResultMoveSeparator + game.Result + options.NewLine;
        }


        private string BuildMoveTree(MoveTreeNode<PostMoveState> rootNode)
        {
            //Get the next node's move recorded to text
            var stringBuilder = new PgnBuilder(options);
            BuildMoveTree((MoveTreeNode<PostMoveState>)rootNode.Continuations.FirstOrDefault(), ref stringBuilder, 0);
            return stringBuilder.ToString().Trim();
        }

        private void BuildMoveTree(MoveTreeNode<PostMoveState> rootNode, ref PgnBuilder stringBuilder,
            ContinuationType continuationType = ContinuationType.MainLine)
        {
            if (rootNode == null)
            {
                return;
            }

            var moveInfo = new MoveInformation(rootNode);
            var nextNode = rootNode.Continuations.FirstOrDefault();
            if (nextNode == null)
            {
                continuationType = (continuationType.RemoveFlags(ContinuationType.MainLine)) | ContinuationType.EndVariation;
            }
            stringBuilder.AppendMove(moveInfo.ToPgn(options), moveInfo.ActiveColor,
                continuationType);
            AppendCurrentSiblings(ref stringBuilder, moveInfo.SiblingVariationNodes().ToArray());
            if (nextNode != null)
            {
                BuildMoveTree((MoveTreeNode<PostMoveState>)nextNode, ref stringBuilder);
            }

            return;
        }

        private void AppendCurrentSiblings(ref PgnBuilder pgnBuilder,
            MoveTreeNode<PostMoveState>[] moveInfoSiblingVariations)
        {
            if (!moveInfoSiblingVariations.Any())
            {
                return;
            }

            foreach (var variation in moveInfoSiblingVariations)
            {
                BuildMoveTree(variation, ref pgnBuilder, ContinuationType.Variation);
            }
        }


        private string GetFormattedComment(string comment)
        {
            if (options.KeepComments && !string.IsNullOrEmpty(comment))
            {
                return $"{Environment.NewLine}    {{{comment.Trim()}}}{Environment.NewLine}";
            }

            return "";
        }

        private string GetFormattedPly(MoveInformation moveInfo)
        {
            var moveText = moveInfo.ToPgn(options);
            return moveText;
        }


        private string BuildTags(in Tags tags)
        {
            var sb = new StringBuilder();
            foreach (var requiredTag in tags.RequiredTags)
            {
                sb.Append($"[{requiredTag.Key} \"{requiredTag.Value}\"]{options.NewLine}");
            }

            foreach (var supplementalTag in tags.SupplementalTags)
            {
                sb.Append($"[{supplementalTag.Key} \"{supplementalTag.Value}\"]{options.NewLine}");
            }

            return sb.ToString();
        }

        [Flags]
        internal enum ContinuationType
        {
            MainLine,
            Variation,
            EndVariation
        }

        internal class PgnBuilder
        {
            private StringBuilder Accumulator { get; } = new StringBuilder();

            private StringBuilder StringBuilder { get; } = new StringBuilder();
            private Stack<int> IndentationStack { get; } = new Stack<int>();

            public PgnBuilder(PGNFormatterOptions formatterOptions)
            {
                options = formatterOptions;
                IndentationStack.Push(0);
            }

            private readonly PGNFormatterOptions options;


            public override string ToString()
            {
                var strReturnValue = StringBuilder + Accumulator.ToString();
                return strReturnValue;
            }

            public PgnBuilder AppendMove(string value, Color activeColor, ContinuationType nodeType)
            {
                string formattedValue = value;
                var postMoveString = options.GetPostMoveString(activeColor);
                var indentation = options.GetIndentation(IndentationStack.Peek());
                if (nodeType == ContinuationType.MainLine)
                {
                    formattedValue = $"{value}{postMoveString}";
                }
                else if (nodeType == (ContinuationType.Variation | ContinuationType.EndVariation))
                {
                    formattedValue = $"{indentation}({options.VariationPadding()}{value}{options.VariationPadding()})";
                }
                else if (nodeType == ContinuationType.Variation)
                {
                    formattedValue = $"{indentation}({options.VariationPadding()}{value}";
                    if (options.IndentVariations)
                    {
                        FlushAccumulator();
                        IndentationStack.Push(IndentationStack.Peek() + 1);
                    }
                }
                else if (nodeType == ContinuationType.EndVariation)
                {
                    formattedValue = $"{value}{options.VariationPadding()})";
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
                }

                AddMoveToAccumulator(formattedValue);
                if (nodeType == ContinuationType.EndVariation && options.IndentVariations)
                {
                    FlushAccumulator();
                    IndentationStack.Pop();
                }

                return this;
            }

            private void AddMoveToAccumulator(string formattedValue)
            {
                var accumulatorLength = Accumulator.Length;
                var valueLength = formattedValue.Length;
                var futureLength = accumulatorLength + valueLength;
                var isNewLine = accumulatorLength == 0;
                if (accumulatorLength + valueLength > 80)
                {
                    FlushAccumulator();
                }

                Accumulator.Append(formattedValue);
            }

            private void FlushAccumulator()
            {
                if (Accumulator.Length == 0)
                {
                    return;
                }

                StringBuilder.AppendLine(Accumulator.ToString());
                Accumulator.Clear();
            }
        }
    }
}