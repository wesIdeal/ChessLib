using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Tree;
using EnumsNET;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Types")]
namespace ChessLib.Core.Types
{
    public class PgnFormatter
    {
        internal static readonly string LineFeed = $"{0x0a}";

        internal string NewLine =>
            _options.ExportFormat ? LineFeed : Environment.NewLine;

        private string ResultMoveSeparator => _options.ExportFormat || !_options.ResultOnNewLine ? " " : NewLine;

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
            return tagSection + NewLine + moveSection + ResultMoveSeparator + game.Result + NewLine;
        }


        private string BuildMoveTree(MoveTreeNode<PostMoveState> rootNode, int indentLevel)
        {
            //Get the next node's move recorded to text
            var stringBuilder = new StringBuilder();
            BuildMoveTree(rootNode, ref stringBuilder, 0);
            return stringBuilder.ToString().Trim();
        }

        private void BuildMoveTree(MoveTreeNode<PostMoveState> rootNode, ref StringBuilder stringBuilder,
            int indentLevel)
        {
            //Get the next node's move recorded to text
            var mainContinuation = rootNode.Continuations.FirstOrDefault();
            var variationsText = GetVariationText(rootNode, indentLevel + 1);
            if (mainContinuation != null)
            {
                var move = mainContinuation.Value;
                var moveText = GetFormattedPly((MoveTreeNode<PostMoveState>)mainContinuation, false) + " ";
                stringBuilder.Append(moveText).Append(variationsText);
                BuildMoveTree((MoveTreeNode<PostMoveState>)mainContinuation, ref stringBuilder, indentLevel);
            }
            else
            {
                stringBuilder.Append(variationsText);
            }
        }

        private string GetVariationText(MoveTreeNode<PostMoveState> rootNode, int indent)
        {
            var continuations = rootNode.Continuations.Skip(1).ToArray();

            if (!continuations.Any())
            {
                return string.Empty;
            }

            var variationsText = new List<string>(continuations.Count());
            foreach (var continuation in continuations)
            {
                var variationText = GetFormattedPly((MoveTreeNode<PostMoveState>)continuation, true) + " ";
                var sb = new StringBuilder(variationText);
                BuildMoveTree((MoveTreeNode<PostMoveState>)continuation, ref sb, indent);
                variationsText.Add(sb.ToString().Trim());
            }

            var rv = GetFormattedVariations(variationsText, indent);

            return rv;
        }


        //private string BuildMoveTree(in IEnumerable<PostMoveState> tree, string fen, uint indentLevel = 0)
        //{
        //    var sb = new StringBuilder();
        //    var game = new Game(fen);
        //    var currentNode = tree.First();

        //    while (currentNode != null)
        //    {
        //        var previousMove = currentNode;
        //        var move = currentNode;


        //        var plySequence = GetFormattedPly(currentNode);
        //        sb.Append(plySequence);
        //        if (move.Variations.Any())
        //        {
        //            var lstVariations = new List<string>();
        //            foreach (var variation in move.Variations)
        //            {
        //                lstVariations.Add(BuildMoveTree(variation.MainLine, bi.Fen, indentLevel++));
        //            }

        //            sb.Append(GetFormattedVariations(lstVariations, indentLevel++));
        //        }

        //        currentNode = currentNode.Next;
        //    }

        //    var strPgn = sb.ToString().Trim();
        //    return strPgn;
        //}

        private string GetFormattedComment(string comment)
        {
            if (_options.KeepComments && !string.IsNullOrEmpty(comment))
            {
                return $"{Environment.NewLine}    {{{comment.Trim()}}}{Environment.NewLine}";
            }

            return "";
        }

        private string GetFormattedPly(MoveTreeNode<PostMoveState> currentState, bool isContinuationVariation)
        {
            var boardState = (BoardState)currentState.Value.BoardState;
            var strMoveNumber = GetFormattedMoveNumber(boardState, (MoveTreeNode<PostMoveState>)currentState.Previous, isContinuationVariation);
            var strMoveText = currentState.Value.San;
            var moveEndingCharacter = GetEndOfMoveWhitespace(boardState.ActivePlayer);
            var formattedComment = GetFormattedComment(currentState.Comment);
            var annotation = GetFormattedAnnotation(currentState);
            var moveText = $"{strMoveNumber}{strMoveText}{moveEndingCharacter}{annotation}{formattedComment}".Trim();

            return moveText;
        }

        private string GetFormattedVariations(List<string> lstVariations, int depth)
        {
            if (_options.IndentVariations)
            {
                var formattedPgn = string.Join("", lstVariations.Select(v => $"{NewLine}( {v} )"));
                formattedPgn = formattedPgn.Replace(NewLine, NewLine + IndentText(depth));
                return formattedPgn + NewLine;
            }

            return string.Join("", lstVariations.Select(v => $"( {v.Trim()} )"));
        }

        private string GetFormattedMoveNumber(BoardState bi, MoveTreeNode<PostMoveState> previousMove, bool isContinuationVariation)
        {
            if (ShouldWriteMoveNumber(previousMove, isContinuationVariation))
            {
                var shouldUseEllipses = UseEllipses(previousMove, bi.ActivePlayer);
                return FormatMoveNumber(bi.FullMoveCounter,
                    shouldUseEllipses);
            }

            return "";
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
                    sb.Append($"[{requiredTag.Key} \"{requiredTag.Value}\"]{NewLine}");
                }
            }

            foreach (var supplementalTag in tags.SupplementalTags)
            {
                if (KeepTag(supplementalTag))
                {
                    sb.Append($"[{supplementalTag.Key} \"{supplementalTag.Value}\"]{NewLine}");
                }
            }

            return sb.ToString();
        }


        private static bool ShouldWriteMoveNumber(MoveTreeNode<PostMoveState> previousNode, bool isVariationContinuation)
        {
            var moveColor = ((BoardState)previousNode.Value.BoardState).ActivePlayer;
            if ((moveColor == Color.White || //if the last move was White's move
                 previousNode?.Previous == null || // or if this is the first move of the game
                 !string.IsNullOrWhiteSpace(previousNode.Comment)) || isVariationContinuation) //or if the last thing written was a comment
            {
                return true;
            }

            return false;
        }


        private string GetFormattedAnnotation(MoveTreeNode<PostMoveState> postMove)
        {
            if (postMove.Annotation != null && postMove.Annotation.Any())
            {
                if (_options.AnnotationStyle == AnnotationStyle.PGNSpec)
                {
                    return $"{postMove.Annotation.ToNAGString()} ";
                }

                return $"{postMove.Annotation} ";
            }

            return "";
        }


        private static string IndentText(int depth)
        {
            return depth == 0 ? " " : new string(' ', depth * 4);
        }

        private string GetEndOfMoveWhitespace(Color activeColor)
        {
            return _options.ExportFormat ? " " : _options.NewlineEachMove && activeColor == Color.Black ? NewLine : " ";
        }

        private bool UseEllipses(MoveTreeNode<PostMoveState> previousMove, Color activeColor)
        {
            return (previousMove == null || !string.IsNullOrWhiteSpace(previousMove.Comment)) &&
                   activeColor == Color.Black;
        }

        private string FormatMoveNumber(uint moveNumber, bool threePeriods)
        {
            var moveNumberText = moveNumber + (threePeriods ? "..." : ".");

            if (_options.SpaceAfterMoveNumber && !_options.ExportFormat)
            {
                moveNumberText += " ";
            }

            return moveNumberText;
        }
    }
}