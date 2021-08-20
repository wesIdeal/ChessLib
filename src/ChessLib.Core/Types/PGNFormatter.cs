using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.Tree;
using EnumsNET;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Types")]

namespace ChessLib.Core.Types
{
    internal struct MoveInformation
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
            MoveInformation moveInfo = this;
            var any = !moveInfo.IsMainLine;
            return any;
        }

        public MoveTreeNode<PostMoveState> PostMoveState { get; }
        public MoveTreeNode<PostMoveState> PreMoveBoardState { get; }
        public Move Move => PostMoveState.Value.MoveValue;
        public Move ParentMove => PreMoveBoardState?.Value.MoveValue;
        public bool IsMainLine => SiblingVariations.First().MoveValue == Move;
        public string San => PostMoveState.Value.San;

        public override string ToString()
        {
            return San;
        }

        public Move[] SiblingVariations =>
            PreMoveBoardState.Continuations.Select(x => (Move)x.Value.MoveValue).ToArray();

        public Move[] ParentVariations =>
            PreMoveBoardState?.Previous?.Continuations.Select(x => (Move)x.Value.MoveValue).ToArray();

        public string Comment => PostMoveState.Comment;
        public NumericAnnotation Annotation => PostMoveState.Annotation;
    }

    public class PgnFormatter
    {
        internal static readonly string LineFeed = $"{0x0a}";


        private string ResultMoveSeparator => _options.ResultOnNewLine ? _options.NewLine : " ";

        public PgnFormatter(PGNFormatterOptions options)
        {
            if (!options.KeepAllTags)
            {
                foreach (var tag in options.TagsToKeep.GetFlags())
                {
                    _tagsToKeep.Add(tag.AsString());
                }

                _tagsToKeep.AddRange(options.OtherTagsToKeep);
            }

            _options = options;
        }

        private readonly PGNFormatterOptions _options;
        private readonly List<string> _tagsToKeep = new List<string>();

        public string BuildPgn(Game game)
        {
            var gameClone = (Game)game.Clone();
            var tagSection = BuildTags(gameClone.Tags);
            var treeRoot = gameClone.InitialNode;
            var moveSection = BuildMoveTree(treeRoot.Node, 0);
            return tagSection + _options.NewLine + moveSection + ResultMoveSeparator + game.Result + _options.NewLine;
        }


        private string BuildMoveTree(MoveTreeNode<PostMoveState> rootNode, int indentLevel)
        {
            //Get the next node's move recorded to text
            var stringBuilder = new PgnBuilder();
            BuildMoveTree(rootNode, ref stringBuilder, 0);
            return stringBuilder.ToString().Trim();
        }

        private void BuildMoveTree(MoveTreeNode<PostMoveState> rootNode, ref PgnBuilder stringBuilder,
            int indentLevel)
        {
            //Get the next node's move recorded to text
            var mainContinuation = rootNode.Continuations.FirstOrDefault();
            var variationsList = GetVariationText(rootNode, indentLevel + 1);
            var variationsText = GetFormattedVariations(variationsList, indentLevel + 1);
            if (mainContinuation != null)
            {
                var moveInfo = new MoveInformation((MoveTreeNode<PostMoveState>)mainContinuation);
                var moveText = GetFormattedPly(moveInfo) + " ";
                stringBuilder.Append(moveText).Append(variationsText.Trim());
                BuildMoveTree((MoveTreeNode<PostMoveState>)mainContinuation, ref stringBuilder, indentLevel);
            }
            else
            {
                stringBuilder.Append(variationsText);
            }
        }

        private List<string> GetVariationText(MoveTreeNode<PostMoveState> rootNode, int indent)
        {
            var continuations = rootNode.Continuations.Skip(1).ToArray();

            if (!continuations.Any())
            {
                return new List<string>();
            }

            var variationsText = new List<string>(continuations.Count());
            foreach (var node in continuations)
            {
                var continuation = (MoveTreeNode<PostMoveState>)node;
                var moveInfo = new MoveInformation(continuation);
                var variationText = GetFormattedPly(moveInfo) + " ";
                var sb = new PgnBuilder(variationText);
                BuildMoveTree(continuation, ref sb, indent);
                variationsText.Add(sb.ToString().Trim());
            }

            return variationsText;
        }


        private string GetFormattedComment(string comment)
        {
            if (_options.KeepComments && !string.IsNullOrEmpty(comment))
            {
                return $"{Environment.NewLine}    {{{comment.Trim()}}}{Environment.NewLine}";
            }

            return "";
        }

        private string GetFormattedPly(MoveInformation moveInfo)
        {
            var moveText = moveInfo.ToPgn(_options);
            return moveText;
        }

        private string GetFormattedVariations(IReadOnlyCollection<string> lstVariations, int depth)
        {
            if (_options.IndentVariations && lstVariations.Any())
            {
                var formattedPgn = string.Join("", lstVariations.Select(v => $"{_options.NewLine}( {v} )"));
                formattedPgn = formattedPgn.Replace(_options.NewLine, _options.NewLine + IndentText(depth));
                return formattedPgn + _options.NewLine;
            }

            return string.Join("", lstVariations.Select(v => $"( {v.Trim()} )"));
        }

        

        private bool KeepTag(in KeyValuePair<string, string> tag)
        {
            if (_options.KeepAllTags)
            {
                return true;
            }

            if (!_options.KeepTagsWithUnknownValues && IsTagUnknown(tag))
            {
                return false;
            }

            return _tagsToKeep.Contains(tag.Key);
        }

        private bool IsTagUnknown(KeyValuePair<string, string> tag)
        {
            return tag.Value.StartsWith("?") || tag.Key == "Result" && tag.Value == "*";
        }

        private string BuildTags(in Tags tags)
        {
            var sb = new StringBuilder();
            foreach (var requiredTag in tags.RequiredTags)
            {
                if (KeepTag(requiredTag))
                {
                    sb.Append($"[{requiredTag.Key} \"{requiredTag.Value}\"]{_options.NewLine}");
                }
            }

            foreach (var supplementalTag in tags.SupplementalTags)
            {
                if (KeepTag(supplementalTag))
                {
                    sb.Append($"[{supplementalTag.Key} \"{supplementalTag.Value}\"]{_options.NewLine}");
                }
            }

            return sb.ToString();
        }


        private static string IndentText(int depth)
        {
            return depth == 0 ? " " : new string(' ', depth * 4);
        }

        private class PgnBuilder
        {
            private StringBuilder Accumulator { get; }

            private StringBuilder StringBuilder { get; }

            public PgnBuilder()
            {
                StringBuilder = new StringBuilder();
                Accumulator = new StringBuilder();
            }

            public PgnBuilder(string variationText) : this()
            {
                Append(variationText);
            }

            public override string ToString()
            {
                var strReturnValue = StringBuilder + Accumulator.ToString();
                return strReturnValue;
            }

            public PgnBuilder Append(string value)
            {
                var valueLength = value.Length;
                var accumulatorLength = Accumulator.Length;
                if (valueLength + accumulatorLength > 80)
                {
                    StringBuilder.Append(Accumulator);
                    Accumulator.Clear();
                }

                Accumulator.Append(value);
                return this;
            }
        }
    }
}