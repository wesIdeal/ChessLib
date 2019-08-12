using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Boards;

namespace ChessLib.Data
{
    public class PGNFormatter<TS> where TS : MoveExt, IEquatable<TS>
    {
        private Game<TS> _game;
        private string _initialFEN;
        private PGNFormatterOptions _options;
        private const char NewLine = '\n';
        public PGNFormatter(PGNFormatterOptions options)
        {
            _options = options;
        }

        public string BuildPGN(Game<TS> game)
        {
            _game = game;
            _initialFEN = game.TagSection.FENStart;
            var tagSection = BuildTags(_game.TagSection);
            var tree = _game.MoveTree as MoveTree<TS>;
            var moveSection = BuildMoveTree(tree, _initialFEN);
            return tagSection + NewLine + moveSection + NewLine;
        }

        private string BuildTags(in Tags tags)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var requiredTag in tags.RequiredTags)
            {
                sb.Append($"[{requiredTag.Key} \"{requiredTag.Value}\"]{NewLine}");
            }
            foreach (var requiredTag in tags.SupplementalTags)
            {
                sb.Append($"[{requiredTag.Key} \"{requiredTag.Value}\"]{NewLine}");
            }

            return sb.ToString();
        }



        private string BuildMoveTree(in MoveTree<TS> tree, string fen, uint indentLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            var bi = new BoardInfo(fen);

            MoveDisplayService displayService = new MoveDisplayService();
            var iterations = 0;
            foreach (var node in tree.GetNodeEnumerator())
            {
                displayService.Initialize(bi);
                string strMoveNumber = "";
                if (ShouldWriteMoveNumber(iterations, bi.ActivePlayer))
                {
                    strMoveNumber = FormatMoveNumber(bi.FullmoveCounter,
                         UseEllipses(iterations, bi.ActivePlayer));
                }

                string strMoveText = displayService.MoveToSAN(node.MoveData);
                var moveEndingCharacter = FullMoveEndingCharacter(bi.ActivePlayer);
                var moveText = $"{strMoveNumber}{strMoveText}{moveEndingCharacter}";
                sb.Append(moveText);
                if (node.Variations.Any())
                {
                    var lstVariations = new List<string>();
                    foreach (var variation in node.Variations)
                    {
                        lstVariations.Add(BuildMoveTree(variation, bi.ToFEN()));
                    }

                    sb.Append(GetFormattedVariations(lstVariations, node.Depth + 1));
                    iterations = 0;
                }


                else
                {
                    iterations++;
                }
                bi.ApplyValidatedMove(node.MoveData);

            }

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
