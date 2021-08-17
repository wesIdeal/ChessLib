using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Tree;
using EnumsNET;

namespace ChessLib.Core.Types
{
    public class PgnFormatter
    {
        private const char NewLine = '\n';

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
            return tagSection + NewLine + moveSection + NewLine + game.Result + NewLine;
        }


        private string BuildMoveTree(MoveTreeNode<PostMoveState> treeRoot, int indentLevel)
        {
            var sb = new StringBuilder();
            // First, get SAN for current move
            if (treeRoot.Value.MoveValue != Move.NullMove)
            {
                var currentSan = GetFormattedPly(treeRoot);
                sb.Append(currentSan);
            }

            if (treeRoot.Continuations.Any())
            {
                var mainLine = treeRoot.Continuations.First();
                var mainLinePgn = BuildMoveTree((MoveTreeNode<PostMoveState>)mainLine, indentLevel);
                var variations = treeRoot.Continuations.Cast<MoveTreeNode<PostMoveState>>().Skip(1).ToArray();
                if (variations.Any())
                {
                    var variationTextTrees = new List<string>();
                    foreach (var variation in variations)
                    {
                        var variationText = BuildMoveTree(variation, indentLevel + 1);
                        variationTextTrees.Add(variationText);
                    }
                    sb.Append(GetFormattedVariations(variationTextTrees, indentLevel + 1));
                }

                sb.Append(mainLinePgn);
            }

            return sb.ToString().Trim();
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

        private string GetFormattedPly(MoveTreeNode<PostMoveState> currentState)
        {
            var boardState = (BoardState)currentState.Value.BoardState;
            var strMoveNumber = GetFormattedMoveNumber(boardState, (MoveTreeNode<PostMoveState>)currentState.Previous);
            var strMoveText = currentState.Value.San;
            var moveEndingCharacter = GetEndOfMoveWhitespace(boardState.ActivePlayer);
            var formattedComment = GetFormattedComment(currentState.Comment);
            var annotation = GetFormattedAnnotation(currentState);
            var moveText = $"{strMoveNumber}{strMoveText}{moveEndingCharacter}{annotation}{formattedComment}";

            return moveText;
        }

        private string GetFormattedVariations(List<string> lstVariations, int depth)
        {
            if (_options.IndentVariations)
            {
                var formattedPgn = string.Join("", lstVariations.Select(v => $"{NewLine}( {v} )"));
                formattedPgn = formattedPgn.Replace(NewLine.ToString(), NewLine + IndentText(depth));
                return formattedPgn + NewLine;
            }

            return string.Join("", lstVariations.Select(v => $"( {v} ) "));
        }

        private string GetFormattedMoveNumber(BoardState bi, MoveTreeNode<PostMoveState> previousMove)
        {
            if (ShouldWriteMoveNumber(previousMove))
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


        private static bool ShouldWriteMoveNumber(MoveTreeNode<PostMoveState> previousNode)
        {
            var moveColor = ((BoardState)previousNode.Value.BoardState).ActivePlayer;
            if ( moveColor == Color.White || //if the last move was White's move
                previousNode?.Previous == null || // or if this is the first move of the game
                !string.IsNullOrWhiteSpace(previousNode.Comment)) //or if the last thing written was a comment
            {
                return true;
            }

            return false;
        }


        private string GetFormattedAnnotation(MoveTreeNode<PostMoveState> postMove)
        {
            if (postMove.Annotation != null)
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

        private char GetEndOfMoveWhitespace(Color activeColor)
        {
            return _options.ExportFormat ? ' ' : _options.NewlineEachMove && activeColor == Color.Black ? NewLine : ' ';
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