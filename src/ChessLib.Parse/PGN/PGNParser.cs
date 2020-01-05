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
using ChessLib.Parse.PGN.Base;
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
            var reader = new PgnReader(pgn, ParserOptions);
            return await GetGamesFromPGNAsync(reader);
        }

        public async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(Stream pgnStream)
        {
            var reader = new PgnReader(pgnStream, ParserOptions);
            return await GetGamesFromPGNAsync(reader);
        }

        private async Task<IEnumerable<Game<MoveStorage>>> GetGamesFromPGNAsync(PgnReader reader)
        {
            var rv = await reader.Parse();
            UpdateProgress?.Invoke(this,
                new ParsingUpdateEventArgs(Stopwatch.Elapsed) { Maximum = reader.GameCount, NumberComplete = reader.Completed });
            return rv;
        }





        private Game<MoveStorage> ParseGame(GameContext gameCtx)
        {
            var gameVisitor = new GameVisitor();
            var game = gameVisitor.VisitGame(gameCtx, ParserOptions);
            return game;
        }
    }
}