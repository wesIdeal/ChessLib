using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Parse.PGN.Base
{
    public class PgnLexer
    {

        readonly char[] _whiteSpaceTokens =
        {
            ' ', '.', '<', '>',
            '\n', '\v', '\t', '\r'
        };

        public static (string tagSection, string moveSection) GetSectionsFromPGN(string pgn)
        {
            const string tagSectionKey = "tagSection";
            const string sectionSeperatingRegEx =
                "(?<" + tagSectionKey + ">[\\s]*[\\[[\\s\\S]*\\\"(\\s)*\\])(\\r\\n){2}";
            Regex regex = new Regex(sectionSeperatingRegEx);
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

        public Game<MoveStorage> ParseGame(in string game, PGNParserOptions options, out List<PgnParsingLog> parseLogs)
        {
            _pgnParser = new PgnParser();
            parseLogs = new List<PgnParsingLog>();
            var sections = GetSectionsFromPGN(game);
            ParseTagSection(sections.tagSection, parseLogs);
            ParseMoveSection(sections.moveSection, options, parseLogs);
            parseLogs.AddRange(_pgnParser.LogMessages);
            return _pgnParser.Game;
        }

        private void ParseTagSection(string tagSection, List<PgnParsingLog> parseLogs)
        {
            if (!string.IsNullOrWhiteSpace(tagSection))
            {
                _pgnParser.VisitTagPairSection(tagSection);
            }
            else
            {
                parseLogs.Add(new PgnParsingLog()
                { ErrorLevel = ErrorLevel.Warning, Message = "Warning: No tag section found for game." });
            }
        }

        private void ParseMoveSection(string moveSection, PGNParserOptions options, List<PgnParsingLog> parseLogs)
        {
            if (!string.IsNullOrWhiteSpace(moveSection))
            {
                _pgnParser.VisitMoveSection(moveSection, options);
            }
            else
            {
                parseLogs.Add(new PgnParsingLog()
                { ErrorLevel = ErrorLevel.Warning, Message = "Warning: No move section found for game." });
            }
        }


        private PgnParser _pgnParser;

    }
}