using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private PGNParserOptions _options;
        private int _gameCount;
        public int Completed;

        public PgnReader(string pgnString, PGNParserOptions options = null) : this(options)
        {
            _pgn = pgnString;
        }

        public PgnReader(Stream pgnFileStream, PGNParserOptions options = null) : this(options)
        {
            InitStream(pgnFileStream);
            using (var sr = new StreamReader(_stream))
            {
                _pgn = sr.ReadToEnd();
                _stream.Close();
                _stream.Dispose();
            }
        }

        private PgnReader(PGNParserOptions pgnParseOptions)
        {
            _options = pgnParseOptions ?? new PGNParserOptions();
        }

        public int GameCount { get; private set; }

        public string[] SectionSeparatorToken { get; } = { TokenSectionEnd };

        protected Stopwatch Stopwatch = new Stopwatch();
        public EventHandler<ParsingUpdateEventArgs> UpdateProgress;

        public async Task<IEnumerable<Game<MoveStorage>>> Parse()
        {
            var strGames = SplitPgnIntoGames();
            GameCount = strGames.Count();
            _gameCount = strGames.Count;
            var take = _options.LimitGameCount ? _options.GameCountToParse : _gameCount;
            var rv = new Game<MoveStorage>[take];

            var parseTasks = strGames.Take(take)
                .Select((g, idx) => Task.Factory.StartNew(() =>
                {
                    var lexer = new PgnLexer();
                    var game = lexer.ParseGame(g, _options, out var logs);
                    rv[idx] = game;
                    Completed++;
                }).ContinueWith(t =>
                {
                    if (Completed % _options.UpdateFrequency == 0)
                    {
                        var args = new ParsingUpdateEventArgs(Stopwatch.Elapsed)
                        { Maximum = _gameCount, NumberComplete = Completed };
                        UpdateProgress?.Invoke(this, args);
                    }
                }));
            var taskList = new List<Task>();
            taskList.AddRange(parseTasks);
            await Task.WhenAll(taskList.ToArray());
            return rv.Where(x => x != null);
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