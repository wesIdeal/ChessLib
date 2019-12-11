using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using ChessLib.Parse.PGN.Parser.Visitor;

namespace ChessLib.Parse.PGN
{
    using GameContext = Parser.BaseClasses.PGNParser.Pgn_gameContext;
    using DatabaseContext = Parser.BaseClasses.PGNParser.Pgn_databaseContext;

    public class PGNParser
    {
        private readonly PGNParserOptions _parserOptions;

        public PGNParser()
        {
            _parserOptions = new PGNParserOptions();
        }

        public PGNParser(PGNParserOptions options)
        {
            _parserOptions = options;
        }


        public async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(string pgn)
        {
            var context = GetContext(new AntlrInputStream(pgn));
            return await GetAllGamesAsync(context);
        }

        public async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(Stream chessDatabaseStream)
        {
            var context = GetContext(new AntlrInputStream(chessDatabaseStream));
            return await GetAllGamesAsync(context);
        }

        public EventHandler<ParsingUpdateEventArgs> UpdateProgress;


        private async Task<Game<MoveStorage>[]> GetAllGamesAsync(DatabaseContext context)
        {
            var taskList = new List<Task>();
            var gameCount = context.pgn_game().Length;
            var rv = new Game<MoveStorage>[gameCount];
            var count = 0;

            var tasks = context.pgn_game().Select((gameCtx, idx) =>
                Task.Factory.StartNew(() =>
                {
                    var game = ParseGame(gameCtx);
                    rv[idx] = game;
                    count++;
                }).ContinueWith((t) =>
                {
                    if (count % _parserOptions.UpdateFrequency == 0)
                    {
                        var args = new ParsingUpdateEventArgs() { Maximum = gameCount, NumberComplete = count };
                        UpdateProgress?.Invoke(this, args);
                    }
                }));
            taskList.AddRange(tasks);
            await Task.WhenAll(taskList.ToArray());
            return rv;
        }

        private Game<MoveStorage> ParseGame(GameContext gameCtx)
        {
            var gameVisitor = new GameVisitor();
            var game = gameVisitor.VisitGame(gameCtx);
            return game;
        }

        private DatabaseContext GetContext(AntlrInputStream gameStream)
        {
            var sw = new Stopwatch();
            sw.Start();
            var lexer = new PGNLexer(gameStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new Parser.BaseClasses.PGNParser(commonTokenStream);
            sw.Stop();
            Console.WriteLine($"Parsed games in {sw.ElapsedMilliseconds} ms.");
            return parser.pgn_database();
        }
    }
}