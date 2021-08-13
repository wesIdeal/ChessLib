using System;
using System.Collections.Generic;
using System.Text;
using EnumsNET;

namespace ChessLib.Core.Types
{
    public class PgnFormatter<TS> where TS : Move, IEquatable<TS>
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
        private Game _game;
        private string _initialFEN;

        public string BuildPgn(Game game)
        {
            _game = game;
            _initialFEN = game.Tags.FENStart;
            //var tagSection = BuildTags(_game.Tags);
            //var treeRoot = _game.InitialPosition;
            //var moveSection = BuildMoveTree(treeRoot, _initialFEN);
            //return tagSection + NewLine + moveSection + NewLine + game.Result + NewLine;
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


        //private string BuildMoveTree(in IEnumerable<PostMoveState> tree, string fen, uint indentLevel = 0)
        //{
        //    var sb = new StringBuilder();
        //    var game = new Game(fen);

        //    var bi = game.InitialPosition.Board;
        //    //if (tree.HasGameComment)
        //    //{
        //    //    sb.Append(IndentText(indentLevel) + GetFormattedComment(tree.GameComment));
        //    //}

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

        //private string GetFormattedPly(PostMoveState state)
        //{
        //    var boardState = state.BoardState;
        //    var move = state.Move;
        //    var strMoveNumber = GetFormattedMoveNumber(boardState, state.ParentNode);
        //    var strMoveText = move.SAN;
        //    var moveEndingCharacter = GetEndOfMoveWhitespace(boardState.ActivePlayer);
        //    var formattedComment = GetFormattedComment(state.PostMoveComment);
        //    var annotation = GetFormattedAnnotation(state);
        //    var moveText = $"{strMoveNumber}{strMoveText}{moveEndingCharacter}{annotation}{formattedComment}";

        //    return moveText;
        //}

        //private string GetFormattedAnnotation(PostMoveState postMove)
        //{
        //    if (postMove.Annotation != null)
        //    {
        //        if (_options.AnnotationStyle == AnnotationStyle.PGNSpec)
        //        {
        //            return $"{postMove.Annotation.ToNAGString()} ";
        //        }

        //        return $"{postMove.Annotation} ";
        //    }

        //    return "";
        //}

        //private string GetFormattedMoveNumber(BoardState bi, PostMoveState previousPostMove)
        //{
        //    if (ShouldWriteMoveNumber(bi.ActivePlayer, previousPostMove))
        //    {
        //        return FormatMoveNumber(bi.FullMoveCounter,
        //            UseEllipses(previousPostMove, bi.ActivePlayer));
        //    }

        //    return "";
        //}

        //private string GetFormattedComment(string comment)
        //{
        //    if (_options.KeepComments && !string.IsNullOrEmpty(comment))
        //    {
        //        return $"{Environment.NewLine}    {{{comment.Trim()}}}{Environment.NewLine}";
        //    }

        //    return "";
        //}

        //private string GetFormattedVariations(List<string> lstVariations, uint depth)
        //{
        //    if (_options.IndentVariations)
        //    {
        //        var formattedPgn = string.Join("", lstVariations.Select(v => $"{NewLine}( {v} )"));
        //        formattedPgn = formattedPgn.Replace(NewLine.ToString(), NewLine + IndentText(depth));
        //        return formattedPgn + NewLine;
        //    }

        //    return string.Join("", lstVariations.Select(v => $"( {v} ) "));
        //}

        //private static string IndentText(uint depth)
        //{
        //    return depth == 0 ? " " : new string(' ', (int)depth * 4);
        //}

        //private char GetEndOfMoveWhitespace(Color activeColor)
        //{
        //    return _options.ExportFormat ? ' ' : _options.NewlineEachMove && activeColor == Color.Black ? NewLine : ' ';
        //}

        //private static bool ShouldWriteMoveNumber(Color activeColor, PostMoveState previousPostMove)
        //{
        //    if (activeColor == Color.White)
        //    {
        //        return true;
        //    }

        //    if (previousPostMove == null)
        //    {
        //        return true;
        //    }

        //    if (!string.IsNullOrWhiteSpace(previousPostMove.PostMoveComment))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //private bool UseEllipses(PostMoveState previousPostMove, Color activeColor)
        //{
        //    return (previousPostMove == null || !string.IsNullOrWhiteSpace(previousPostMove.PostMoveComment)) &&
        //           activeColor == Color.Black;
        //}

        //private string FormatMoveNumber(uint moveNumber, bool threePeriods)
        //{
        //    var moveNumberText = moveNumber + (threePeriods ? "..." : ".");

        //    if (_options.SpaceAfterMoveNumber && !_options.ExportFormat)
        //    {
        //        moveNumberText += " ";
        //    }

        //    return moveNumberText;
        //}
    }
}