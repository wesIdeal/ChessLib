using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Boards;
using EnumsNET;

namespace ChessLib.Data
{
    public class PGNFormatter<TS> where TS : MoveExt, IEquatable<TS>
    {
        private Game<TS> _game;
        private string _initialFEN;
        private PGNFormatterOptions _options;
        private const char NewLine = '\n';
        private List<string> _tagsToKeep = new List<string>();
        public PGNFormatter(PGNFormatterOptions options)
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

        public string BuildPGN(Game<TS> game)
        {
            _game = game;
            _initialFEN = game.TagSection.FENStart;
            var tagSection = BuildTags(_game.TagSection);
            var tree = _game.MainMoveTree as MoveTree;
            var moveSection = BuildMoveTree(tree, _initialFEN);
            return tagSection + NewLine + moveSection + NewLine;
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

        private bool IsTagUnknown(KeyValuePair<string, string> tag) => 
            tag.Value.StartsWith("?") || (tag.Key == "Result" && tag.Value == "*");

        private string BuildTags(in Tags tags)
        {
            StringBuilder sb = new StringBuilder();
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
            StringBuilder sb = new StringBuilder();
            var game = new Game<MoveStorage>(fen);
            game.BeginGameInitialization();
            var bi = game.Board;
            MoveDisplayService displayService = new MoveDisplayService();
            var iterations = 0;
            foreach (var node in tree)
            {
                if (node.IsNullMove)
                {
                    continue;
                }
                displayService.Initialize(bi);
                string strMoveNumber = "";
                if (ShouldWriteMoveNumber(iterations, bi.ActivePlayer))
                {
                    strMoveNumber = FormatMoveNumber(bi.FullmoveCounter,
                         UseEllipses(iterations, bi.ActivePlayer));
                }

                string strMoveText = displayService.MoveToSAN(node);
                var moveEndingCharacter = FullMoveEndingCharacter(bi.ActivePlayer);
                var moveText = $"{strMoveNumber}{strMoveText}{moveEndingCharacter}";
                sb.Append(moveText);
                if (node.Variations.Any())
                {
                    var lstVariations = new List<string>();
                    foreach (var variation in node.Variations)
                    {
                        lstVariations.Add(BuildMoveTree(variation, bi.ToFEN(), indentLevel++));
                    }

                    sb.Append(GetFormattedVariations(lstVariations, indentLevel++));
                    iterations = 0;
                }


                else
                {
                    iterations++;
                }
                game.ApplyMove(node);

            }
            game.EndGameInitialization();
            var strPgn = sb.ToString().Trim();
            return strPgn;
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

        private string IndentText(uint depth) => depth == 0 ? " " : new string(' ', (int)depth * 4);

        private char FullMoveEndingCharacter(Color activeColor)
        {

            return _options.ExportFormat ? ' ' : (_options.NewlineEachMove && activeColor == Color.Black) ? NewLine : ' ';
        }

        private bool ShouldWriteMoveNumber(int iterations, Color activeColor) =>
            UseEllipses(iterations, activeColor) || activeColor == Color.White;
        private bool UseEllipses(int iterations, Color activeColor) => iterations == 0 && activeColor == Color.Black;
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
