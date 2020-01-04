using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Parse.PGN
{
    public enum ParseSection
    {
        Tag,
        Move
    };

    public enum ErrorLevel
    {
        Info,
        Warning,
        Error
    };

    public class PgnParsingLog
    {

        public ErrorLevel ErrorLevel { get; set; }
        public string Message { get; set; }

    }

    public class PgnParser
    {
        public Game<MoveStorage> Game;
        public List<PgnParsingLog> LogMessages = new List<PgnParsingLog>();

        public PgnParser()
        {
            Game = new Game<MoveStorage>();
        }

        public bool VisitResult(char c)
        {
            if (c == '*')
            {
                Game.GameResult = GameResult.None;
            }

            return false;
        }

        private bool _nextMoveIsVariation = false;

        public bool VisitVariationStart()
        {
            _nextMoveIsVariation = true;
            return true;
        }

        public bool VisitVariationEnd()
        {
            _nextMoveIsVariation = false;
            Game.ExitVariation();
            return true;
        }

        public bool VisitTagPair(in StringReader reader)
        {
            string key, value, buffer;
            key = VisitTagKey(reader);
            value = VisitTagValue(reader);
            AddTagPair(key, value);
            return true;
        }

        private string VisitTagValue(in StringReader reader)
        {
            var buffer = PgnLexer.ReadUntil(reader, ']');
            return buffer.Trim().Trim('"').Trim();
        }

        private string VisitTagKey(in StringReader reader)
        {
            var buffer = PgnLexer.ReadUntil(reader, '"');
            return buffer.Trim().TrimStart('[').Trim();
        }

        public void AddTagPair(string key, string value)
        {
            Game.TagSection.Add(key, value);
        }
    }

    public class PgnLexer
    {
        public Game<MoveStorage> ParseGame(in string game, out List<PgnParsingLog> parseLogs)
        {
            Game<MoveStorage> rv = new Game<MoveStorage>();
            var gameLength = game.Length - 1;
            _pgnParser = new PgnParser();
            using (var reader = new StringReader(game))
            {
                while (ParseNextToken(reader))
                {

                }
            }

            parseLogs = _pgnParser.LogMessages;
            return rv;
        }

        private readonly char[] SelfTerminatingTokens = new[]
        {
            ' ', '.', '<', '>',
            '\n', '\v', '\t', '\r'
        };

        private PgnParser _pgnParser;

        private bool ParseNextToken(in StringReader reader)
        {
            char token = (char)reader.Read();
            if (token == -1)
            {
                return false;
            }

            if (IsSelfTerminatingToken(token))
            {
                return true;
            }

            switch (token)
            {
                case '*':
                    return _pgnParser.VisitResult('*');
                case '(':
                    return _pgnParser.VisitVariationStart();
                case ')':
                    return _pgnParser.VisitVariationEnd();
                case '[':
                    return _pgnParser.VisitTagPair(reader);
                default: return true;
            }
        }





        internal static string ReadUntil(in StringReader reader, in char c)
        {
            var buffer = "";
            char readChar;
            while ((readChar = (char)reader.Read()) != -1 && readChar != c)
            {
                buffer += readChar;
            }

            return buffer;
        }

        private bool IsSelfTerminatingToken(char token) => SelfTerminatingTokens.Contains(token);
    }

    public class PgnReader
    {
        private MemoryStream _stream;
        private int _gameNumber;
        private string _pgn;

        public PgnReader(string pgnString) : this()
        {
            _pgn = pgnString;
        }

        public PgnReader(Stream pgnFileStream) : this()
        {
            InitStream(pgnFileStream);
            using (var sr = new StreamReader(_stream))
            {
                _pgn = sr.ReadToEnd();
                _stream.Close();
                _stream.Dispose();
            }
        }

        private PgnReader()
        {
            _gameNumber = 0;

        }

        public IEnumerable<Game<MoveStorage>> Parse()
        {
            var rv = new List<Game<MoveStorage>>();
            List<string> strGames = SplitPgnIntoGames();
            GameCount = strGames.Count();
            return rv;
        }

        public int GameCount { get; private set; }


        private List<string> SplitPgnIntoGames()
        {
            var rv = new List<string>();
            var tmp = NormalizeNewLines();
            var split = tmp.Split(SectionSeparatorToken, StringSplitOptions.RemoveEmptyEntries);
            var dbIndex = -1;
            foreach (var section in split.Select(x => x.Trim()))
            {
                if (section.StartsWith(TokenTagBegin.ToString()))
                {
                    rv.Add(section);
                    dbIndex++;
                }
                else
                {
                    rv[dbIndex] += TokenSectionEnd + section;
                }
            }

            return rv;
        }

        private string NormalizeNewLines()
        {
            var tmp = _pgn.Replace("\r", "");
            tmp = tmp.Replace("\n", Environment.NewLine);
            return tmp;
        }

        public string[] SectionSeparatorToken { get; } = new[] { TokenSectionEnd };
        protected static string TokenSectionEnd = $"{Environment.NewLine}{Environment.NewLine}";
        protected char TokenTagBegin = '[';

        private void InitStream(Stream pgnFileStream)
        {
            if (!pgnFileStream.CanRead)
            {
                throw new ArgumentException("PgnReader(): Cannot read from provided stream.",
                    nameof(pgnFileStream));
            }

            if (!pgnFileStream.CanSeek)
            {
                throw new ArgumentException("PgnReader(): Cannot seek provided stream.", nameof(pgnFileStream));
            }

            pgnFileStream.Position = 0;
            _stream = new MemoryStream();
            pgnFileStream.CopyTo(_stream);
        }
    }
}
