using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.PgnExport;
using ChessLib.Core.Types.Tree;
using EnumsNET;

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

        public bool IsInitialBoard => Move.IsNullMove;

        /// <summary>
        ///     Get the active player's color for this move.
        /// </summary>
        public Color ActiveColor => ((BoardState)PreMoveBoardState.Value.BoardState).ActivePlayer;

        public MoveTreeNode<PostMoveState> PostMoveState { get; }
        public MoveTreeNode<PostMoveState> PreMoveBoardState { get; }
        public Move Move => PostMoveState.Value.MoveValue;
        public Move ParentMove => PreMoveBoardState?.Value.MoveValue;

        /// <summary>
        ///     Is the current node the main continuation from parent.
        /// </summary>
        public bool IsMainLineContinuation =>
            PreMoveBoardState?.Continuations.FirstOrDefault()?.Value.MoveValue == Move;

        public string San => PostMoveState.Value.San;

        public virtual IEnumerable<MoveTreeNode<PostMoveState>> CurrentContinuations =>
            PostMoveState.Continuations.Cast<MoveTreeNode<PostMoveState>>();

        /// <summary>
        ///     Gets the next main line continuation, or
        ///     <value>null</value>
        ///     if none exist.
        /// </summary>
        public MoveTreeNode<PostMoveState> NextMove => CurrentContinuations.FirstOrDefault();

        public virtual bool IsLastMove => NextMove == null;

        public Move[] SiblingVariations => SiblingVariationNodes()
            .Select(x => (Move)x.Value.MoveValue).ToArray();

        public Move[] ParentVariations =>
            PreMoveBoardState?.Previous?.Continuations.Select(x => (Move)x.Value.MoveValue).ToArray();

        public string Comment => PostMoveState.Comment;
        public NumericAnnotation Annotation => PostMoveState.Annotation;

        public MoveInformation PreviousMoveInformation
            => PreMoveBoardState != null ? new MoveInformation(PreMoveBoardState) : null;

        /// <summary>
        ///     Provided <see cref="NextMove" /> isn't null, it creates an information object for the next node. Otherwise, null.
        /// </summary>
        public MoveInformation NextMoveInformation => NextMove != null ? new MoveInformation(NextMove) : null;

        public MoveInformation(MoveTreeNode<PostMoveState> postMoveTreeNode)
        {
            PreMoveBoardState = (MoveTreeNode<PostMoveState>)postMoveTreeNode.Previous;
            PostMoveState = postMoveTreeNode;
            bufferLength = 0;
        }

        protected MoveInformation()
        {
            //for unit test mocking
        }

        private int bufferLength;

        public object Clone()
        {
            return new MoveInformation((MoveTreeNode<PostMoveState>)PostMoveState.Clone());
        }

        public bool Equals(MoveInformation other)
        {
            return other != null &&
                   PostMoveState.Equals(other.PostMoveState);
        }

        public bool IsFirstMoveOfVariation()
        {
            var moveInfo = this;
            var any = !moveInfo.IsMainLineContinuation;
            return any;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(San) ? Move.ToString() : San;
        }

        /// <summary>
        ///     Gets sibling continuations which are not the mainline
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<MoveTreeNode<PostMoveState>> SiblingVariationNodes()
        {
            var moveInfo = this;
            if (IsFirstMoveOfVariation())
            {
                return new List<MoveTreeNode<PostMoveState>>();
            }

            return moveInfo.PreMoveBoardState?.Continuations
                       .Skip(1)
                       .Cast<MoveTreeNode<PostMoveState>>()
                   ?? new List<MoveTreeNode<PostMoveState>>();
        }
    }

    public class PgnFormatter
    {
        private string ResultMoveSeparator => options.ResultOnNewLine ? options.NewLineIndicator : " ";

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
            return tagSection + options.NewLineIndicator + moveSection + ResultMoveSeparator + game.Result +
                   options.NewLineIndicator;
        }


        private string BuildMoveTree(MoveTreeNode<PostMoveState> rootNode)
        {
            //Get the next node's move recorded to text
            var stringBuilder = new PgnBuilder(options);
            BuildMoveTree((MoveTreeNode<PostMoveState>)rootNode.Continuations.FirstOrDefault(), ref stringBuilder, 0);
            return stringBuilder.ToString().Trim();
        }

        private void BuildMoveTree(MoveTreeNode<PostMoveState> rootNode, ref PgnBuilder stringBuilder,
            PgnWriter.ContinuationType continuationType = PgnWriter.ContinuationType.MainLine)
        {
            if (rootNode == null)
            {
                return;
            }

            var moveInfo = new MoveInformation(rootNode);
            var nextNode = rootNode.Continuations.FirstOrDefault();
            if (nextNode == null)
            {
                continuationType = continuationType.RemoveFlags(PgnWriter.ContinuationType.MainLine) |
                                   PgnWriter.ContinuationType.EndVariation;
            }

            //stringBuilder.AppendMove(PgnMove.ToPgn(moveInfo, options), moveInfo.IsLastMove);

            AppendCurrentSiblings(ref stringBuilder, moveInfo.SiblingVariationNodes().ToArray());
            if (nextNode != null)
            {
                BuildMoveTree((MoveTreeNode<PostMoveState>)nextNode, ref stringBuilder);
            }
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
                BuildMoveTree(variation, ref pgnBuilder, PgnWriter.ContinuationType.Variation);
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


        private string BuildTags(in Tags tags)
        {
            var sb = new StringBuilder();
            foreach (var requiredTag in tags.RequiredTags)
            {
                sb.Append($"[{requiredTag.Key} \"{requiredTag.Value}\"]{options.NewLineIndicator}");
            }

            foreach (var supplementalTag in tags.SupplementalTags)
            {
                sb.Append($"[{supplementalTag.Key} \"{supplementalTag.Value}\"]{options.NewLineIndicator}");
            }

            return sb.ToString();
        }

       

        internal class PgnBuilder
        {
            private StringBuilder Accumulator { get; } = new StringBuilder();

            private StringBuilder StringBuilder { get; } = new StringBuilder();
            private Stack<int> IndentationStack { get; } = new Stack<int>();

            public int? CurrentDepth => IndentationStack.Any() ? IndentationStack.Peek() : (int?)null;

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

            public PgnBuilder AppendMoveAsVariation(string value)
            {
                IndentationStack.Push(CurrentDepth.Value + 1);
                if (options.IndentVariations)
                {
                    FlushAccumulator();
                }

                return AppendMove("(" + options.GetVariationPadding() + value, false);
            }

            public PgnBuilder AppendMoveAsEndOfLine(string value)
            {
                var lineEndingIndication = CurrentDepth > 0 ? ")" : "";
                var moveValue = AppendMove(value + options.GetVariationPadding() + ")", true);
                IndentationStack.Pop();
                return moveValue;
            }

            public PgnBuilder AppendMove(string value, bool endOfLine)
            {
                if (endOfLine)
                {
                    Accumulator.Append(value);
                    FlushAccumulator();
                    return this;
                }

                if (Accumulator.Length + value.Length > options.MaxCharsPerLine)
                {
                    FlushAccumulator();
                }

                Accumulator.Append(value);
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

                var accumulatorValue = Accumulator.ToString().TrimEnd() + options.NewLineIndicator;
                StringBuilder.Append(accumulatorValue);
                var indentation = options.GetIndentation(CurrentDepth.Value);
                Accumulator.Clear().Append(indentation);
            }
        }
    }
}