using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Base;

namespace ChessLib.Parse.PGN
{
    public class PGNParser
    {
        protected static string TokenSectionEnd = $"{Environment.NewLine}{Environment.NewLine}";
        public readonly PGNParserOptions ParserOptions;

        private string _pgn;
        private MemoryStream _stream;
        public int Completed;

        protected Stopwatch Stopwatch = new Stopwatch();
        protected char TokenTagBegin = '[';

        public EventHandler<ParsingUpdateEventArgs> UpdateProgress;

        public PGNParser() : this(new PGNParserOptions())
        {
        }

        public PGNParser(PGNParserOptions options)
        {
            ParserOptions = options;
        }

        public string[] SectionSeparatorToken { get; } = { TokenSectionEnd };

        public int GameCount { get; protected set; }


        public async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(string pgn)
        {
            _pgn = pgn;
            return await GetGamesFromPGNAsync();
        }

        public async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(Stream pgnStream)
        {
            SendUpdate($"Opening and reading stream." + Environment.NewLine);
            Stopwatch.Restart();
            InitStream(pgnStream);
            _stream.Position = 0;
            var bufferSize = 128 * 1024;
            var buffer = new byte[bufferSize];
            int readBytes;
            while ((readBytes = _stream.Read(buffer, 0, bufferSize)) != 0)
            {
                SendUpdate($"Read {readBytes} from stream");
                var str = Encoding.Default.GetString(buffer);
                _pgn += str;
            }

            _stream.Close();
            _stream.Dispose();
            SendUpdate($"Finished reading stream to memory in {Stopwatch.ElapsedMilliseconds} ms.{Environment.NewLine}" + Environment.NewLine);
            Stopwatch.Restart();
            return await GetGamesFromPGNAsync();
        }

        private async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync()
        {
            Stopwatch.Restart();
            var strGames = SplitPgnIntoGames();
            GameCount = strGames.Count;
            GameCount = strGames.Count;
            var take = ParserOptions.LimitGameCount ? ParserOptions.GameCountToParse : GameCount;
            var rv = new Game<MoveStorage>[take];

            var parseTasks = strGames.Take(take)
                .Select((g, idx) => Task.Factory.StartNew(() =>
                {
                    var lexer = new PgnLexer();
                    var game = lexer.ParseGame(g, ParserOptions, out var logs);
                    if (game != null)
                    {
                        game.GoToInitialState();
                        rv[idx] = game;
                    }
                    Completed++;
                }).ContinueWith(t =>
                {
                    if (Completed % ParserOptions.UpdateFrequency == 0)
                    {
                        SendUpdate();
                    }
                }));
            var taskList = new List<Task>();
            taskList.AddRange(parseTasks);
            await Task.WhenAll(taskList.ToArray())
                .ContinueWith(task =>
                {
                    Stopwatch.Stop();
                    SendUpdate();
                    Stopwatch.Reset();
                });
            return rv.Where(x => x != null);
        }

        private void SendUpdate()
        {
            var args = new ParsingUpdateEventArgs(Stopwatch.Elapsed)
            { Maximum = GameCount, NumberComplete = Completed };
            UpdateProgress?.Invoke(this, args);
        }

        private void SendUpdate(string message)
        {
            var args = new ParsingUpdateEventArgs(message);
            UpdateProgress?.Invoke(this, args);
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

        /// <summary>
        /// Split pgn file into separate games
        /// </summary>
        /// <returns>List of found games</returns>
        private List<string> SplitPgnIntoGames()
        {
            SendUpdate("Splitting PGN file." + Environment.NewLine);
            Stopwatch.Restart();
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
            SendUpdate($"Finished splitting PGN file in {Stopwatch.ElapsedMilliseconds} ms." + Environment.NewLine);
            Stopwatch.Restart();
            return rv;
        }
    }
}