using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN;
using ChessLib.Parse.PGN.Parser;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Diagnostics;
using System.Linq;
using ChessLib.Data;

namespace ChessLib.Parse.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var games = new List<Game<MoveStorage>>();
            var listener = new PGNListener();
            using (var fStream = File.OpenRead(".\\PGN\\tal.pgn"))
            {
                var inputStream = new AntlrInputStream(fStream);
                PGNLexer lexer = new PGNLexer(inputStream);
                var tokens = new CommonTokenStream(lexer);
                var parser = new PGNParser(tokens);
                var parseTree = parser.parse();
                var walker = new ParseTreeWalker();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                walker.Walk(listener, parseTree);
                games = listener.Games.Cast<Game<MoveStorage>>().ToList();
                stopwatch.Stop();
                System.Console.WriteLine($"Completed in {stopwatch.ElapsedMilliseconds}ms.");
                var g = games[0];
                var e = games[games.Count - 1];
                var withVariations = games.Where(x => x.TagSection["Black"].Contains("Averbakh"));
                System.Console.WriteLine($"Found {games.Count} games.");
            }
        }
    }
}
