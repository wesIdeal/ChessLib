using System.Collections.Generic;
using System.Text.RegularExpressions;
using ChessLib.Core;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Parse.PGN.Base
{
    public class PgnLexer
    {
        private readonly List<PgnParsingLog> _parsingLogs = new List<PgnParsingLog>();


        private PgnVisitor _pgnVisitor;

        public static (string tagSection, string moveSection) GetSectionsFromPgn(string pgn)
        {
            const string tagSectionKey = "tagSection";
            const string sectionSeparatingRegEx =
                "(?<" + tagSectionKey + ">[\\s]*[\\[[\\s\\S]*\\\"(\\s)*\\])(\\r\\n){2}";
            var regex = new Regex(sectionSeparatingRegEx);
            var matches = regex.Match(pgn);
            var tagGroup = matches.Groups[tagSectionKey];
            if (tagGroup.Success)
            {
                var tagSection = matches.Groups[tagSectionKey]?.Value;
                var moveSection = pgn.Substring(tagGroup.Length).Trim();
                return (tagSection, moveSection);
            }

            return ("", "");
        }

        public Game ParseGame(in string game, PGNParserOptions options)
        {
            _pgnVisitor = new PgnVisitor();
            var sections = GetSectionsFromPgn(game);
            var tags = ParseTagSection(sections.tagSection);
            _pgnVisitor.Game = new Game(tags.FENStart, tags);
            ParseMoveSection(sections.moveSection.Replace("\r\n", " "), options);
            AddParsingLogsToGame();
            return _pgnVisitor.Game;
        }

        private void AddParsingLogsToGame()
        {
            foreach (var log in _parsingLogs)
            {
                _pgnVisitor.Game.AddParsingLogItem(log);
            }
        }

        private Tags ParseTagSection(string tagSection)
        {
            if (!string.IsNullOrWhiteSpace(tagSection))
            {
                return _pgnVisitor.VisitTagPairSection(tagSection);
            }

            _parsingLogs.Add(new PgnParsingLog(ParsingErrorLevel.Warning, "Warning: No tag section found for game.",
                tagSection));
            return null;
        }

        private void ParseMoveSection(string moveSection, PGNParserOptions options)
        {
            if (!string.IsNullOrWhiteSpace(moveSection))
            {
                _pgnVisitor.VisitMoveSection(moveSection, options);
            }
            else
            {
                _parsingLogs.Add(new PgnParsingLog(ParsingErrorLevel.Warning,
                    "Warning: No move section found for game.",
                    moveSection));
            }
        }
    }
}