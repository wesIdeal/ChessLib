using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Core.IO;
using ChessLib.Core.Types.Enums;
using EnumsNET;

namespace ChessLib.Core.Types
{
    public class PgnFormatter<TS> where TS : Move, IEquatable<TS>
    {
        private const char NewLine = '\n';
        private readonly PGNFormatterOptions _options;
        private readonly List<string> _tagsToKeep = new List<string>();
        private Game _game;
        private string _initialFEN;

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

        public string BuildPgn(Game game)
        {
            _game = game;
            _initialFEN = game.TagSection.FENStart;
            var tagSection = BuildTags(_game.TagSection);
            var tree = _game.MainMoveTree;
            var moveSection = BuildMoveTree(tree, _initialFEN);
            return tagSection + NewLine + moveSection + NewLine + game.Result + NewLine;
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


        private string BuildMoveTree(in MoveTree tree, string fen, uint indentLevel = 0)
        {
            var sb = new StringBuilder();
            var game = new Game(fen);
            game.BeginGameInitialization();
            var bi = game.Board;
            if (tree.HasGameComment)
            {
                sb.Append(IndentText(indentLevel) + GetFormattedComment(tree.GameComment));
            }

            var currentNode = tree.First;
            if (currentNode.Value.IsNullMove)
            {
                currentNode = currentNode.Next;
            }

            while (currentNode != null)
            {
                var previousMove = currentNode.Previous?.Value;
                var move = currentNode.Value;
                if (move.IsNullMove)
                {
                    currentNode = currentNode.Next;
                    continue;
                }

                var plySequence = GetFormattedPly(game.Board, previousMove, move);
                sb.Append(plySequence);
                if (move.Variations.Any())
                {
                    var lstVariations = new List<string>();
                    foreach (var variation in move.Variations)
                    {
                        lstVariations.Add(BuildMoveTree(variation, bi.CurrentFEN, indentLevel++));
                    }

                    sb.Append(GetFormattedVariations(lstVariations, indentLevel++));
                }

                game.ApplyMove(move);
                currentNode = currentNode.Next;
            }

            game.EndGameInitialization();
            var strPgn = sb.ToString().Trim();
            return strPgn;
        }

        private string GetFormattedPly(Board bi, BoardSnapshot previousMove,
            BoardSnapshot move)
        {
            var displaySvc = new MoveDisplayService(bi);
            var strMoveNumber = GetFormattedMoveNumber(bi, previousMove);
            var strMoveText = displaySvc.MoveToSAN(move);
            var moveEndingCharacter = GetEndOfMoveWhitespace(bi.ActivePlayer);
            var formattedComment = GetFormattedComment(move.Comment);
            var annotation = GetFormattedAnnotation(move);
            var moveText = $"{strMoveNumber}{strMoveText}{moveEndingCharacter}{annotation}{formattedComment}";

            return moveText;
        }

        private string GetFormattedAnnotation(BoardSnapshot move)
        {
            if (move.Annotation != null)
            {
                if (_options.AnnotationStyle == AnnotationStyle.PGNSpec)
                {
                    return $"{move.Annotation.ToNAGString()} ";
                }

                return $"{move.Annotation} ";
            }

            return "";
        }

        private string GetFormattedMoveNumber(Board bi, BoardSnapshot previousMove)
        {
            if (ShouldWriteMoveNumber(bi.ActivePlayer, previousMove))
            {
                return FormatMoveNumber(bi.FullMoveCounter,
                    UseEllipses(previousMove, bi.ActivePlayer));
            }

            return "";
        }

        private string GetFormattedComment(string comment)
        {
            if (_options.KeepComments && !string.IsNullOrEmpty(comment))
            {
                return $"{Environment.NewLine}    {{{comment.Trim()}}}{Environment.NewLine}";
            }

            return "";
        }

        private string GetFormattedVariations(List<string> lstVariations, uint depth)
        {
            if (_options.IndentVariations)
            {
                var formattedPgn = string.Join("", lstVariations.Select(v => $"{NewLine}( {v} )"));
                formattedPgn = formattedPgn.Replace(NewLine.ToString(), NewLine + IndentText(depth));
                return formattedPgn + NewLine;
            }

            return string.Join("", lstVariations.Select(v => $"( {v} ) "));
        }

        private static string IndentText(uint depth)
        {
            return depth == 0 ? " " : new string(' ', (int) depth * 4);
        }

        private char GetEndOfMoveWhitespace(Color activeColor)
        {
            return _options.ExportFormat ? ' ' : _options.NewlineEachMove && activeColor == Color.Black ? NewLine : ' ';
        }

        private static bool ShouldWriteMoveNumber(Color activeColor, BoardSnapshot previousMove)
        {
            if (activeColor == Color.White)
            {
                return true;
            }

            if (previousMove == null)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(previousMove.Comment))
            {
                return true;
            }

            return false;
        }

        private bool UseEllipses(BoardSnapshot previousMove, Color activeColor)
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