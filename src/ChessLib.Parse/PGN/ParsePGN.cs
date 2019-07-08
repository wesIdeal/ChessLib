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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Parse.PGN
{
    public class ParsePgn
    {


        public ParsePgn()
        {
        }

        public TimeSpan TotalValidationTime;


        public IEnumerable<Game<IMoveText>> GetGameTexts(AntlrInputStream inputStream, int? maxGames = null)
        {
            var listener = new PGNListener();
            PGNLexer lexer = new PGNLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PGNParser(tokens);
            var parseTree = parser.parse();
            var walker = new ParseTreeWalker();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            walker.Walk(listener, parseTree);
            sw.Stop();
            Debug.WriteLine($"Parsed {listener.Games.Count()} games in {sw.ElapsedMilliseconds} ms, ({sw.ElapsedMilliseconds / 1000} seconds.)");
            return listener.Games;
        }

        /// <summary>
        /// Gets MoveExt objects from PGN and validates moves while parsing.
        /// </summary>
        /// <returns>Validated Moves</returns>
        public List<Game<MoveStorage>> ParseAndValidateGames(string strGameDatabase, int? MaxGames = null)
        {
            AntlrInputStream stream = new AntlrInputStream(strGameDatabase);
            var games = GetGameTexts(stream, MaxGames);
            return ValidateGames(games, MaxGames);
        }

        /// <summary>
        /// Gets MoveExt objects from PGN and validates moves while parsing.
        /// </summary>
        /// <returns>Validated Moves</returns>
        public List<Game<MoveStorage>> ParseAndValidateGames(FileStream fs, int? MaxGames = null)
        {
            AntlrInputStream stream = new AntlrInputStream(fs);
            var games = GetGameTexts(stream, MaxGames);
            return ValidateGames(games, MaxGames);
        }

        /// <summary>
        /// Gets MoveExt objects from PGN and validates moves while parsing.
        /// </summary>
        /// <returns>Validated Moves</returns>
        private List<Game<MoveStorage>> ValidateGames(IEnumerable<Game<IMoveText>> games, int? MaxGames = null)
        {
            var rv = new List<Game<MoveStorage>>();
            var sw = new Stopwatch();
            var gameCount = MaxGames ?? games.Count();
            sw.Start();
            Parallel.ForEach(games, (game) =>
             {
                 var fen = game.TagSection.FENStart;
                 var boardInfo = new BoardInfo();
                 foreach (var move in game.MoveSection)
                 {
                     boardInfo.ApplySANMove(move.SAN);
                 }

                 rv.Add(new Game<MoveStorage>() { TagSection = game.TagSection, MoveSection = boardInfo.MoveTree });
             });
            sw.Stop();
            TotalValidationTime = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            Debug.WriteLine($"Validated {games.Count()} games in {sw.ElapsedMilliseconds} ms, ({sw.ElapsedMilliseconds / 1000} seconds.)");
            return rv;
        }

    }
}
