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
    public class PgnVisitor
    {
        protected const char TokenVariationStart = '(';
        protected const char TokenVariationEnd = ')';
        protected const char TokenCommentStart = '{';
        protected const char TokenCommentEnd = '}';
        private const string MoveRegEx = "[a-h]|[x]|[O-O]|[O-O-O]|[KNBQR]|[1-8]|[=Q|=R|=B|=N]|[+|#]";
        public Dictionary<string, List<ulong>> FenToHashDictionary = new Dictionary<string, List<ulong>>();
        private static readonly char[] TokensChars = { ' ', ')', '(', '{', '}' };
        private readonly Regex _moveRegex = new Regex(MoveRegEx);
        private bool _foundGame;

        private char[] _nagStartSymbols;
        private bool _nextMoveIsVariation;
        private int _plyCount;
        private int _variationDepth;
        public Game<MoveStorage> Game;
        public List<PgnParsingLog> LogMessages = new List<PgnParsingLog>();
        protected (string, MoveNAG)[] MoveNags;
        protected (string, NonStandardNAG)[] NonStandardNags;
        protected (string, PositionalNAG)[] PositionalNags;
        protected (string, TimeTroubleNAG)[] TimeTroubleNags;

        public PgnVisitor()
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
            moveSection = ParseResultFromMoveSection(moveSection);
            var reader = new StringReader(moveSection);
            int nNextCharacter;
            while ((nNextCharacter = reader.Peek()) != -1)
            {
                var nextChar = (char)nNextCharacter;
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

            var plentyOfHashes = FenToHashDictionary.Where(x => x.Value.Count() > 1).ToArray();
            if (plentyOfHashes.Any())
            {
                Console.WriteLine($"\r\nGame Contained Multiple Hashes\r\n{new string('*', 20)}\r\n");
                foreach (var hashSet in plentyOfHashes)
                {
                    Console.WriteLine($"{hashSet.Value}");
                    foreach (var key in hashSet.Key)
                    {
                        Console.WriteLine($"\t{key}");
                    }
                    Console.WriteLine();
                }
            }
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
                Game.AddParsingLogItem(ParsingErrorLevel.Warning,
                        $"Tag Section: Could not parse tag pair for the following line{Environment.NewLine}{tagSection}"
                );
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

        protected int[] GetNAGSymbolMatches(string possibleNagStart)
        {
            var rv = new List<int>();
            rv.AddRange(MoveNags.Where(x => x.Item1 == possibleNagStart).Select(x => (int)x.Item2));
            rv.AddRange(PositionalNags.Where(x => x.Item1 == possibleNagStart).Select(x => (int)x.Item2));
            rv.AddRange(TimeTroubleNags.Where(x => x.Item1 == possibleNagStart).Select(x => (int)x.Item2));
            rv.AddRange(NonStandardNags.Where(x => x.Item1 == possibleNagStart).Select(x => (int)x.Item2));
            return rv.ToArray();
        }

        internal static string ReadUntil(in StringReader reader, params char[] c)
        {
            var buffer = "";
            int nNextChar;
            while ((nNextChar = reader.Peek()) != -1)
            {
                var readChar = (char)nNextChar;
                if (c.Contains(readChar))
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
            var symbolFormat =
                Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<SymbolAttribute>()?.Symbol);
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

        private bool IsPossiblyNag(char c)
        {
            return _nagStartSymbols.Contains(c);
        }

        private string ParseResultFromMoveSection(string moveSection)
        {
            const string resultMatchRegEx = "(?<result>(1-0)|(1/2-1/2)|(0-1)|(\\*))(\\s)*$";
            var regEx = new Regex(resultMatchRegEx);
            var match = regEx.Match(moveSection);
            if (match.Success && match.Groups["result"].Success)
            {
                var result = match.Groups["result"].Value;
                Game.Result = result;
                regEx.Replace(moveSection, "");
            }
            else
            {
                Game.GameResult = GameResult.None;
            }

            return moveSection;
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

        private void VisitNAGSymbol(StringReader reader)
        {
            var nagBuffer = ReadUntil(reader, TokensChars);
            var nags = GetNAGSymbolMatches(nagBuffer);
            if (nags.Any())
            {
                if (nags.Length == 1)
                {
                    Game.ApplyNAG(nags[0]);
                }
                else
                {
                    Game.AddParsingLogItem(
                         ParsingErrorLevel.Warning,
                        $"Found multiple symbol matches for {nagBuffer}", nagBuffer);
                }
            }
            else
            {
                Game.AddParsingLogItem(ParsingErrorLevel.Warning,
                     $"Could not find symbol match for {nagBuffer}", nagBuffer);
            }
        }

        private void VisitNumericNag(StringReader reader)
        {
            var nagBuffer = ReadUntil(reader, TokensChars);
            nagBuffer = nagBuffer.TrimStart('$');
            Game.ApplyNAG(Convert.ToInt32(nagBuffer));
        }

        private bool VisitSanMove(in StringReader reader, PGNParserOptions options)
        {
            var buffer = "";
            int nReadChar;
            while ((nReadChar = reader.Peek()) != -1)
            {
                var c = (char)nReadChar;
                if (!_moveRegex.IsMatch(c.ToString()))
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
            AddPositionToDictionary(move);
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

        private void AddPositionToDictionary(LinkedListNode<MoveStorage> move)
        {
            var fen = Game.CurrentFEN;
            var hash = move.Value.BoardStateHash;
            if (!FenToHashDictionary.ContainsKey(fen))
            {
                FenToHashDictionary.Add(fen, new List<ulong>());
            }

            if (!FenToHashDictionary[fen].Contains(hash))
            {
                FenToHashDictionary[fen].Add(hash);
            }
        }
    }
}