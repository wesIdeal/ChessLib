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

namespace ChessLib.Parse.PGN.Parser.Visitor
{
    using GameContext = BaseClasses.PGNParser.Pgn_gameContext;
    using DatabaseContext = BaseClasses.PGNParser.Pgn_databaseContext;

    public class GameDatabaseVisitor
    {
        private const string DbgCategory = "Game Parsing/Validation";
        private readonly DatabaseContext _context;

        public GameDatabaseVisitor(Stream gameDatabaseStream)
        {
            _context = GetContext(new AntlrInputStream(gameDatabaseStream));
        }

        public GameDatabaseVisitor(string gameDatabaseTxt)

        {
            _context = GetContext(new AntlrInputStream(gameDatabaseTxt));
        }

        public int GameCount => _context.pgn_game().Length;

        public async Task<Game<MoveStorage>[]> GetAllGames()
        {
            var taskList = new List<Task>();
            var sw = new Stopwatch();
            Debug.WriteLine($"***************{Environment.NewLine}Parsing and Validating {GameCount} games.{Environment.NewLine}***************", DbgCategory);
            var rv = new Game<MoveStorage>[GameCount];
            sw.Start();
            var count = 0;
            var message = "Processed {0} games after {1} ms.";

            var tasks = _context.pgn_game().Select((gameCtx, idx) =>
                Task.Factory.StartNew(() =>
                {
                    var game = ParseGame(gameCtx);
                    rv[idx] = game;
                    count++;
                    Debug.WriteLineIf(count % 100 == 0,
                        string.Format(message, count, sw.ElapsedMilliseconds), DbgCategory);
                }));
            taskList.AddRange(tasks);
            await Task.WhenAll(taskList.ToArray());
            sw.Stop();
            Debug.WriteLine("***************" + string.Format(message, count, sw.ElapsedMilliseconds) + " * **************",
                DbgCategory);
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
            var parser = new BaseClasses.PGNParser(commonTokenStream);
            sw.Stop();
            Debug.WriteLine($"Parsed games in {sw.ElapsedMilliseconds} ms.");
            return parser.pgn_database();
        }
    }
}