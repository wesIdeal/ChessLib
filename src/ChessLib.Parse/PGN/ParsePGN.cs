using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Data;
using ChessLib.Data.Boards;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChessLib.Parse.PGN
{
    public class ParsePgn
    {
        AntlrInputStream _inputStream;

        public static ParsePgn FromFilePath(string pgnPath)
        {
            return new ParsePgn() { _inputStream = new AntlrFileStream(pgnPath) };
        }

        public static ParsePgn FromText(string pgnText)
        {
            return new ParsePgn() { _inputStream = new AntlrInputStream(pgnText) };
        }

        private ParsePgn()
        {
        }

        public double AvgTimePerMove;
        public double AvgTimePerGame;
        public double AvgValidationTimePerGame;
        public double TotalValidationTime;
        public double TotalTime;

        public List<Game<IMoveText>> GetGameTexts(int? maxGames = null)
        {

            var listener = new PGNListener();
            PGNLexer lexer = new PGNLexer(_inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PGNParser(tokens);
            var parseTree = parser.parse();
            var walker = new ParseTreeWalker();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            walker.Walk(listener, parseTree);
            sw.Stop();
            AvgTimePerMove = listener.AvgTimePerMove;
            AvgTimePerGame = listener.AvgTimePerGame;
            TotalTime = sw.Elapsed.TotalSeconds;
            return listener.Games;
        }

        /// <summary>
        /// Gets MoveExt objects from PGN and validates moves while parsing.
        /// </summary>
        /// <returns>Validated Moves</returns>
        public List<Game<TMoveStorage>> GetGames<TBoardService, TMoveStorage>(int? MaxGames = null)
            where TBoardService : BoardInformationService<TMoveStorage> where TMoveStorage : MoveStorage
        {
            var rv = new List<Game<TMoveStorage>>();

            var perGame = new List<long>();
            var sw = new Stopwatch();
            var games = GetGameTexts();
            var gameCount = MaxGames ?? games.Count();
            foreach (var game in games.Take(gameCount))
            {
                sw.Reset();
                sw.Start();
                var fen = game.TagSection.FENStart;
                var boardInfo = (TBoardService)Activator.CreateInstance(typeof(TBoardService), fen, false);
                foreach (var move in game.MoveSection)
                {
                    boardInfo.ApplyMove(move.Move.SAN);
                }
                sw.Stop();
                perGame.Add(sw.ElapsedMilliseconds);
                rv.Add(new Game<TMoveStorage>() { TagSection = game.TagSection, MoveSection = boardInfo.MoveTree });
            }

            AvgValidationTimePerGame = perGame.Average();
            TotalValidationTime = (double)perGame.Sum() / 1000;
            return rv;
        }

    }
}
