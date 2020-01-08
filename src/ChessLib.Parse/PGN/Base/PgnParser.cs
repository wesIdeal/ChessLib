using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.MoveRepresentation.NAG;
using EnumsNET;

namespace ChessLib.Parse.PGN.Base
{
    public class PgnParser
    {
        protected const char TokenVariationStart = '(';
        protected const char TokenVariationEnd = ')';
        protected const char TokenCommentStart = '{';
        protected const char TokenCommentEnd = '}';
        private bool _foundGame;

        private char[] _nagStartSymbols;
        private bool _nextMoveIsVariation;
        private int _plyCount;
        public Game<MoveStorage> Game;
        public List<PgnParsingLog> LogMessages = new List<PgnParsingLog>();
        protected (string, MoveNAG)[] MoveNags;
        protected (string, NonStandardNAG)[] NonStandardNags;
        protected (string, PositionalNAG)[] PositionalNags;
        protected (string, TimeTroubleNAG)[] TimeTroubleNags;
        private int _variationDepth;

        public PgnParser()
        {
            Game = new Game<MoveStorage>();
            InitNagInfo();
        }

        public void AddTagPair(string key, string value)
        {
            Game.TagSection.Add(key, value);
        }

        public void VisitMoveSection(string moveSection, PGNParserOptions options)
        {
            const string moveNumbersRegEx = "([\\d]+[.]+)|[(\\r\\n)|(\\n)]";
            var regEx = new Regex(moveNumbersRegEx);
            var movesOnly = regEx.Replace(moveSection, " ");
            var reader = new StringReader(movesOnly);
            int nNextCharacter;
            string parsedThusFar = "";
            while ((nNextCharacter = reader.Peek()) != -1)
            {
                var nextChar = (char)nNextCharacter;
                parsedThusFar += nextChar;
                if (char.IsLetter(nextChar))
                {
                    if (!VisitSanMove(reader, options))
                    {
                        break;
                    }
                }
                else if (nextChar == TokenVariationStart)
                {
                    VisitVariationStart(options);
                    reader.Read();
                }
                else if (nextChar == TokenVariationEnd)
                {
                    VisitVariationEnd(options);
                    reader.Read();
                }
                else if (nextChar == TokenCommentStart)
                {
                    VisitComment(reader);
                }
                else if (IsPossiblyNag(nextChar))
                {
                    VisitNAGSymbol(reader);
                }
                else if (nextChar == '$')
                {
                    VisitNumericNag(reader);
                }
                else
                {
                    reader.Read();
                }
            }
        }

        private void VisitNumericNag(StringReader reader)
        {
            var nagBuffer = ReadUntil(reader, ' ');
            nagBuffer = nagBuffer.TrimStart('$');
            Game.ApplyNAG(Convert.ToInt32(nagBuffer));
        }

        private void VisitNAGSymbol(StringReader reader)
        {
            var nagBuffer = ReadUntil(reader, ' ');
            var nags = GetNAGSybolMatches(nagBuffer);
            if (nags.Any())
            {
                if (nags.Length == 1)
                {
                    Game.ApplyNAG(nags[0]);
                }
                else
                {
                    LogMessages.Add(new PgnParsingLog
                    {
                        ErrorLevel = ErrorLevel.Warning,
                        Message = $"Found multiple symbol matches for {nagBuffer}"
                    });
                }
            }
            else
            {
                LogMessages.Add(new PgnParsingLog
                {
                    ErrorLevel = ErrorLevel.Warning,
                    Message = $"Could not find symbol match for {nagBuffer}"
                });
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

        public bool VisitVariationEnd(PGNParserOptions options)
        {
            _nextMoveIsVariation = false;
            _variationDepth--;
            if (!options.IgnoreVariations)
            {
                Game.ExitVariation();
            }
            return true;
        }

        public bool VisitVariationStart(PGNParserOptions options)
        {
            _nextMoveIsVariation = true;
            _variationDepth++;
            return true;
        }

        protected int[] GetNAGSybolMatches(string possibleNagStart)
        {
            var rv = new List<int>();
            rv.AddRange(MoveNags.Where(x => x.Item1 == possibleNagStart).Select(x => (int)x.Item2));
            rv.AddRange(PositionalNags.Where(x => x.Item1 == possibleNagStart).Select(x => (int)x.Item2));
            rv.AddRange(TimeTroubleNags.Where(x => x.Item1 == possibleNagStart).Select(x => (int)x.Item2));
            rv.AddRange(NonStandardNags.Where(x => x.Item1 == possibleNagStart).Select(x => (int)x.Item2));
            return rv.ToArray();
        }

        internal static string ReadUntil(in StringReader reader, in char c)
        {
            var buffer = "";
            var nNextChar = -1;
            while ((nNextChar = reader.Peek()) != -1)
            {
                var readChar = (char)nNextChar;
                if (readChar == c)
                {
                    break;
                }
                buffer += readChar;
                reader.Read();
            }

            return buffer;
        }

        private static IEnumerable<(string, TEnum)> EnumerateNagSymbols<TEnum>()
            where TEnum : struct, Enum
        {
            var symbolFormat = Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<SymbolAttribute>()?.Symbol);
            foreach (var moveNag in Enums.GetValues<TEnum>(EnumMemberSelection.Distinct))
            {
                var symbol = moveNag.AsString(symbolFormat);
                if (!string.IsNullOrWhiteSpace(symbol))
                {
                    yield return (symbol, moveNag);
                }
            }
        }

        private void InitNagInfo()
        {
            MoveNags = EnumerateNagSymbols<MoveNAG>().ToArray();
            PositionalNags = EnumerateNagSymbols<PositionalNAG>().ToArray();
            TimeTroubleNags = EnumerateNagSymbols<TimeTroubleNAG>().ToArray();
            NonStandardNags = EnumerateNagSymbols<NonStandardNAG>().ToArray();
            var firstChars = MoveNags.Select(x => x.Item1[0]).ToList();
            firstChars.AddRange(PositionalNags.Select(x => x.Item1[0]));
            firstChars.AddRange(TimeTroubleNags.Select(x => x.Item1[0]));
            firstChars.AddRange(NonStandardNags.Select(x => x.Item1[0]));
            _nagStartSymbols = firstChars.ToArray();
        }

        private bool IsCharLegalForMove(char c)
        {
            return char.IsLetterOrDigit(c) || c == '-' || c == '=';
        }

        private bool IsPossiblyNag(char c)
        {
            return _nagStartSymbols.Contains(c);
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
                if (move.BoardStateHash == options.BoardStateHash)
                {
                    _foundGame = true;
                }
                else if (_plyCount >= options.FilterPlyLimit)
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
            int nChar;
            while ((nChar = reader.Peek()) != -1)
            {
                var c = (char)nChar;
                if (!IsCharLegalForMove(c))
                {
                    break;
                }

                buffer += c;
                reader.Read();
            }

            var strategy = _nextMoveIsVariation
                ? MoveApplicationStrategy.Variation
                : MoveApplicationStrategy.ContinueMainLine;
            _nextMoveIsVariation = false;
            if (options.IgnoreVariations && _variationDepth != 0)
            {
                return true;
            }
            var move = Game.ApplySanMove(buffer, strategy);
            _plyCount = Game.PlyCount;

            if (move != null && options.ShouldFilterDuringParsing)
            {
                if (options.ShouldUseFenFilter)
                {
                    if (!ValidatePositionFilter(options, move.Value))
                    {
                        Game = null;
                        return false;
                    }
                }

                if (options.ShouldLimitPlyCount && _plyCount >= options.MaximumPlyPerGame)
                {
                    return false;
                }
            }

            return true;
        }
    }
}