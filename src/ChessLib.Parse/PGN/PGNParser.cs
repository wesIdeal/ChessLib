using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChessLib.Core;
using ChessLib.Parse.PGN.Base;

namespace ChessLib.Parse.PGN
{
    public class PGNParser
    {
        protected static string TokenSectionEnd = $"{Environment.NewLine}{Environment.NewLine}";

        protected static string EmptyTagSection =
            "[Event \"\"]\r\n[Site \"\"]\r\n[Date \"\"]\r\n[Round \"\"]\r\n[White \"\"]\r\n[Black \"\"]\r\n[Result \"*\"]" +
            TokenSectionEnd;

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

        public int GameCount { get; protected set; }


        public async Task<IEnumerable<Game>> GetGamesFromPGNAsync(string pgn)
        {
            _pgn = pgn;
            return await GetGamesFromPGNAsync();
        }

        public async Task<IEnumerable<Game>> GetGamesFromPGNAsync(Stream pgnStream)
        {
            SendUpdate("Opening and reading stream." + Environment.NewLine);
            Stopwatch.Restart();
            InitStream(pgnStream);
            _stream.Position = 0;
            var bufferSize = 128 * 1024;
            var buffer = new byte[bufferSize];
            int readBytes;
            while ((readBytes = _stream.Read(buffer, 0, bufferSize)) != 0)
            {
                SendUpdate($"Read {readBytes} from stream");
                var str = Encoding.Default.GetString(buffer, 0, readBytes);
                _pgn += str;
            }

            _stream.Close();
            _stream.Dispose();
            SendUpdate(
                $"Finished reading stream to memory in {Stopwatch.ElapsedMilliseconds} ms.{Environment.NewLine}" +
                Environment.NewLine);
            Stopwatch.Restart();
            return await GetGamesFromPGNAsync();
        }

        private async Task<IEnumerable<Game>> GetGamesFromPGNAsync()
        {
            Stopwatch.Restart();
            var strGames = SplitPgnIntoGames();
            GameCount = strGames.Count;
            var take = ParserOptions.MaxGameCount ?? GameCount;
            var rv = new Game[take];

            var parseTasks = strGames.Take(take)
                .Select((g, idx) => Task.Factory.StartNew(() =>
                {
                    var lexer = new PgnLexer();
                    var game = lexer.ParseGame(g, ParserOptions);
                    if (game != null)
                    {
                        game.GoToFirstMove();
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

            await Task.WhenAll(parseTasks.ToArray())
                .ContinueWith(task =>
                {
                    Stopwatch.Stop();
                    SendUpdate();
                    Stopwatch.Reset();
                });
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

        private static string SanitizeInput(string pgn)
        {
            pgn = SanitizeNewLines(pgn);
            pgn = SanitizeResult(pgn);
            return pgn;
        }

        private static string SanitizeNewLines(string pgn)
        {
            pgn = Regex.Replace(pgn, "((\\r?)(\\n)){2,}", TokenSectionEnd);
            return pgn;
        }

        private static string SanitizeResult(string tmp)
        {
            var pattern = "(\\r\\n\\r\\n){1,}(?<result>(\\*)|(0-1)|(1-0)|(1/2-1/2))";
            var replacement = $"{Environment.NewLine}${{result}}";
            return Regex.Replace(tmp, pattern, replacement);
        }

        private void SendUpdate()
        {
            var args = new ParsingUpdateEventArgs(Stopwatch.Elapsed)
                {Maximum = GameCount, NumberComplete = Completed};
            UpdateProgress?.Invoke(this, args);
        }

        private void SendUpdate(string message)
        {
            var args = new ParsingUpdateEventArgs(message);
            UpdateProgress?.Invoke(this, args);
        }

        /// <summary>
        ///     Split pgn file into separate games
        /// </summary>
        /// <returns>List of found games</returns>
        private List<string> SplitPgnIntoGames()
        {
            const string regExSplitGames = "(\\r\\n\\r\\n)[\\s]*";
            SendUpdate("Splitting PGN file." + Environment.NewLine);
            Stopwatch.Restart();
            var rv = new List<string>();
            var tmp = SanitizeInput(_pgn);
            var rxSplitter = new Regex(regExSplitGames);
            var split = rxSplitter.Split(tmp).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            var tagSectionFound = false;
            foreach (var pgnSection in split.Select(x => x.Trim()))
            {
                if (pgnSection[0] == TokenTagBegin)
                {
                    tagSectionFound = true;
                    rv.Add(pgnSection);
                }
                else
                {
                    if (tagSectionFound)
                    {
                        var moveSection = $"{TokenSectionEnd}{pgnSection}{TokenSectionEnd}";
                        tagSectionFound = false;
                        rv[rv.Count - 1] += moveSection;
                    }
                    else
                    {
                        var moveSection = $"{EmptyTagSection}{TokenSectionEnd}{pgnSection}{TokenSectionEnd}";
                        rv.Add(moveSection);
                    }
                }
            }

            SendUpdate($"Finished splitting PGN file in {Stopwatch.ElapsedMilliseconds} ms." + Environment.NewLine);
            Stopwatch.Restart();
            return rv;
        }
    }
}