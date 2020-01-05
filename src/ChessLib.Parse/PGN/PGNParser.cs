using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using ChessLib.Parse.PGN.Parser.Visitor;

namespace ChessLib.Parse.PGN
{
    using GameContext = Parser.BaseClasses.PGNParser.PgnGameContext;
    using DatabaseContext = Parser.BaseClasses.PGNParser.PgnDatabaseContext;

   

    public class PGNParser
    {
        public readonly PGNParserOptions ParserOptions;
        private int _count;

        private int _gameCount;

        protected Stopwatch Stopwatch = new Stopwatch();

        public EventHandler<ParsingUpdateEventArgs> UpdateProgress;

        public PGNParser() : this(new PGNParserOptions())
        {
        }

        public PGNParser(PGNParserOptions options)
        {
            ParserOptions = options;
        }

        public async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(string pgn)
        {
            var context = GetContext(new AntlrInputStream(pgn));
            return await GetAllGamesAsync(context);
        }

        public async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(Stream chessDatabaseStream)
        {
            var context = GetContext(new AntlrInputStream(chessDatabaseStream));
            var rv = await GetAllGamesAsync(context);
            UpdateProgress?.Invoke(this,
                new ParsingUpdateEventArgs(Stopwatch.Elapsed) {Maximum = _gameCount, NumberComplete = _count});
            return rv;
        }

        private async Task<Game<MoveStorage>[]> GetAllGamesAsync(DatabaseContext context)
        {
            var taskList = new List<Task>();
            Stopwatch.Restart();

            _count = 0;
            var gameContexts = ParserOptions.LimitGameCount
                ? context.pgnGame().Take(ParserOptions.GameCountToParse).ToArray()
                : context.pgnGame().ToArray();
            _gameCount = gameContexts.Count();
            var rv = new Game<MoveStorage>[_gameCount];
            var tasks = gameContexts.Select((gameCtx, idx) =>
                Task.Factory.StartNew(() =>
                {
                    var game = ParseGame(gameCtx);
                    rv[idx] = game;
                    _count++;
                }).ContinueWith(t =>
                {
                    if (_count % ParserOptions.UpdateFrequency == 0)
                    {
                        var args = new ParsingUpdateEventArgs(Stopwatch.Elapsed)
                            {Maximum = _gameCount, NumberComplete = _count};
                        UpdateProgress?.Invoke(this, args);
                    }
                }));
            taskList.AddRange(tasks);
            await Task.WhenAll(taskList.ToArray());
            return rv.Where(x => x != null).ToArray();
        }

        private DatabaseContext GetContext(AntlrInputStream gameStream)
        {
            Stopwatch.Restart();
            var lexer = new PGNLexer(gameStream);
            lexer.RemoveErrorListeners();
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new Parser.BaseClasses.PGNParser(commonTokenStream)
            {
                Interpreter = {PredictionMode = PredictionMode.SLL}
            };
            parser.RemoveErrorListeners();
            var context = parser.pgnDatabase();
            Stopwatch.Stop();
            UpdateProgress?.Invoke(this,
                new ParsingUpdateEventArgs(
                        $"Completed getting context in {Stopwatch.ElapsedMilliseconds} ms.{Environment.NewLine}")
                    {NumberComplete = 1, Maximum = 1});
            return context;
        }

        private Game<MoveStorage> ParseGame(GameContext gameCtx)
        {
            var gameVisitor = new GameVisitor();
            var game = gameVisitor.VisitGame(gameCtx, ParserOptions);
            return game;
        }
    }
}