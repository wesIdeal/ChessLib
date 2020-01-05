using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Parse.PGN.Base
{
    public class PgnParser
    {
        protected const char TokenVariationStart = '(';
        protected const char TokenVariationEnd = ')';
        protected const char TokenCommentStart = '{';
        protected const char TokenCommentEnd = '}';
        private bool _foundGame;
        private bool _nextMoveIsVariation;
        private int _plyCount;
        public Game<MoveStorage> Game;
        public List<PgnParsingLog> LogMessages = new List<PgnParsingLog>();

        public PgnParser()
        {
            Game = new Game<MoveStorage>();
        }

        public void AddTagPair(string key, string value)
        {
            Game.TagSection.Add(key, value);
        }

        public void VisitMoveSection(string moveSection, PGNParserOptions options)
        {
            const string moveNumbersRegEx = "[\\d]+[.]+";
            var regEx = new Regex(moveNumbersRegEx);
            var movesOnly = regEx.Replace(moveSection, " ");
            var reader = new StringReader(movesOnly);
            int nReadChar;
            while ((nReadChar = reader.Peek()) != -1)
            {
                var readChar = (char) nReadChar;
                if (char.IsLetter(readChar))
                {
                    if (!VisitSanMove(reader, options))
                    {
                        break;
                    }
                }
                else if (readChar == TokenVariationStart)
                {
                    VisitVariationStart();
                    reader.Read();
                }
                else if (readChar == TokenVariationEnd)
                {
                    VisitVariationEnd();
                    reader.Read();
                }
                else if (readChar == TokenCommentStart)
                {
                    VisitComment(reader);
                }
                else
                {
                    reader.Read();
                }
            }
        }

        public bool VisitResult(char c)
        {
            if (c == '*')
            {
                Game.GameResult = GameResult.None;
            }

            return false;
        }

        /// <summary>
        ///     Parses a tag pair
        /// </summary>
        /// <param name="tagSection">PGN Section containing tag key/value pairs</param>
        /// <returns></returns>
        /// <remarks>
        ///     From http://www.saremba.de/chessgml/standards/pgn/pgn-complete.htm#c8.1
        ///     A tag pair is composed of four consecutive tokens: a left bracket token, a symbol token, a string token,
        ///     and a right bracket token. The symbol token is the tag name and the string token is the tag value
        ///     associated with the tag name. (There is a standard set of tag names and semantics described below.)
        ///     The same tag name should not appear more than once in a tag pair section.
        /// </remarks>
        public bool VisitTagPair(in string tagSection)
        {
            const string tagKey = "tagKey", tagValue = "tagValue";
            const string tagPairRegEx =
                "\\[[\\s]*(?<" + tagKey + ">[A-Za-z_0-9]+)[\\s]*\\\"(?<" + tagValue + ">[\\s\\S]*)\\\"[\\s]*[\\s]*\\]";
            var regex = new Regex(tagPairRegEx);
            var matches = regex.Match(tagSection);
            var key = matches.Groups[tagKey];
            var value = matches.Groups[tagValue];

            if (matches.Groups[tagKey].Success && matches.Groups[tagValue].Success)
            {
                AddTagPair(key.Value, value.Value);
            }
            else
            {
                LogMessages.Add(new PgnParsingLog
                {
                    ErrorLevel = ErrorLevel.Warning,
                    Message =
                        $"Tag Section: Could not parse tag pair for the following line{Environment.NewLine}{tagSection}"
                });
            }

            return true;
        }

        public bool VisitTagPairSection(string tagPairs)
        {
            const string tagPairSplitterRegEx = @"\[.*\]";
            var regex = new Regex(tagPairSplitterRegEx);
            var matches = regex.Matches(tagPairs);
            for (var i = 0; i < matches.Count; i++)
            {
                VisitTagPair(matches[i].Value);
            }

            return true;
        }

        public bool VisitVariationEnd()
        {
            _nextMoveIsVariation = false;
            Game.ExitVariation();
            return true;
        }

        public bool VisitVariationStart()
        {
            _nextMoveIsVariation = true;
            return true;
        }

        internal static string ReadUntil(in StringReader reader, in char c)
        {
            var buffer = "";
            char readChar;
            while ((readChar = (char) reader.Read()) != -1 && readChar != c)
            {
                buffer += readChar;
            }

            return buffer;
        }

        /// <summary>
        ///     Validates ply-count limit for parsing
        /// </summary>
        /// <param name="options"></param>
        /// <returns>true if count hasn't been exceeded</returns>
        private bool ValidatePlyCountLimit(in PGNParserOptions options)
        {
            return !(_plyCount >= options.MaximumPlyPerGame);
        }

        private bool ValidatePositionFilter(PGNParserOptions options, MoveStorage move)
        {
            if (!_foundGame)
            {
                if (move.BoardStateHash == options.BoardStateSearchHash)
                {
                    _foundGame = true;
                }
                else if (_plyCount >= options.FenPlyMoveLimit)
                {
                    return false;
                }
            }

            return true;
        }

        private void VisitComment(StringReader reader)
        {
            var comment = ReadUntil(reader, TokenCommentEnd);
            comment = comment.Trim(TokenCommentStart, TokenCommentEnd).Trim();
            Game.AddComment(comment);
        }

        private bool VisitSanMove(in StringReader reader, PGNParserOptions options)
        {
            var buffer = string.Empty;
            char c;
            while ((c = (char) reader.Read()) != -1)
            {
                if (char.IsWhiteSpace(c))
                {
                    break;
                }

                buffer += c;
            }

            var strategy = _nextMoveIsVariation
                ? MoveApplicationStrategy.Variation
                : MoveApplicationStrategy.ContinueMainLine;
            _nextMoveIsVariation = false;
            var move = Game.ApplySanMove(buffer, strategy);
            _plyCount = Game.PlyCount;
            if (move != null && options.FilteringApplied)
            {
                if (options.UseFenFilter && !ValidatePositionFilter(options, move.Value))
                {
                    Game = null;
                    return false;
                }
            }


            if (options.LimitPlyCount && _plyCount >= options.MaximumPlyPerGame)
            {
                return false;
            }

            return true;
        }
    }
}