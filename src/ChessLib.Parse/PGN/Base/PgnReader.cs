using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Parse.PGN.Base
{
    public class PgnReader
    {
        protected static string TokenSectionEnd = $"{Environment.NewLine}{Environment.NewLine}";
        private readonly string _pgn;
        private MemoryStream _stream;
        protected char TokenTagBegin = '[';

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
        }

        public int GameCount { get; private set; }

        public string[] SectionSeparatorToken { get; } = {TokenSectionEnd};

        public IEnumerable<Game<MoveStorage>> Parse()
        {
            var rv = new List<Game<MoveStorage>>();
            var strGames = SplitPgnIntoGames();
            GameCount = strGames.Count();
            var lexer = new PgnLexer();
            foreach (var strGame in strGames)
            {
                var game = lexer.ParseGame(strGame, out var logs);
                rv.Add(game);
            }

            return rv;
        }

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

        private string NormalizeNewLines()
        {
            var tmp = _pgn.Replace("\r", "");
            tmp = tmp.Replace("\n", Environment.NewLine);
            return tmp;
        }


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
    }
}