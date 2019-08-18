using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Data;
using ChessLib.Data.Boards;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Parse.PGN.Parser;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChessLib.Parse.PGN
{
    public class ParsePgn
    {
        public TimeSpan TotalValidationTime;

        public IEnumerable<Game<MoveStorage>> GetGamesFromPGN(string pgn) => GetGamesFromPGNAsync(pgn).Result;

        public IEnumerable<Game<MoveStorage>> GetGamesFromPGN(Stream stream) => GetGamesFromPGNAsync(stream).Result;

        public async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(string pgn) => await GetValidatedGames(new AntlrInputStream(pgn));

        public async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(Stream stream) => await GetValidatedGames(new AntlrInputStream(stream));

        private IEnumerable<string> GetIndividualGames(AntlrInputStream inputStream, int groupSize = 100)
        {
            var lexer = new PGNLexer(inputStream);
            lexer.RemoveErrorListeners();
            var parser = new PGNParser(new CommonTokenStream(lexer));
            var parseTree = parser.parse();
            var walker = new ParseTreeWalker();
            var listener = new PGNGameListener(inputStream);
            walker.Walk(listener, parseTree);
            var games = listener.Games;
            var grouped = games
                .Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / groupSize, x => x.Item);
            return grouped.Select(x => string.Join("", x));
        }

        protected async Task<List<Game<MoveStorage>>> GetValidatedGames(AntlrInputStream inputStream)
        {
            var gameGroups = GetIndividualGames(inputStream);
            var tasks = gameGroups.Select(ProcessGames);
            var rv = await Task.WhenAll(tasks);
            return rv.SelectMany(x => x).ToList();
        }

        protected async Task<List<Game<MoveStorage>>> ProcessGames(string gameGroup)
        {
            var lexer = new PGNLexer(new AntlrInputStream(gameGroup));
            lexer.RemoveErrorListeners();
            var parser = new PGNParser(new CommonTokenStream(lexer));
            var parseTree = parser.parse();
            var walker = new ParseTreeWalker();
            var listener = new PGNGameDetailListener();
            return await Task.Run(() => { walker.Walk(listener, parseTree); })
                .ContinueWith((state) =>
                {
                    listener.Games.ForEach(game => game.GoToInitialState());
                    return listener.Games;
                });
        }

       

    }
}
