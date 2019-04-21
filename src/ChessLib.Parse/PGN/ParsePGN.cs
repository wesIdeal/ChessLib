using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChessLib.Parse.PGN
{
    public class Game<T> where T : IEquatable<T>
    {
        public Tags TagSection = new Tags();
        public MoveTree<T> MoveSection = new MoveTree<T>(null);
    }
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

        public List<Game<IMoveText>> GetGameObjects()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var listener = new PGNListener();
            PGNLexer lexer = new PGNLexer(_inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PGNParser(tokens);
            var parseTree = parser.parse();
            var walker = new ParseTreeWalker();
            walker.Walk(listener, parseTree);
            sw.Stop();
            Debug.WriteLine($"Completed parsing {listener.Games.Count} games, {listener.Games.Select(x => x.MoveSection.Count).Sum()} half-moves total, in {sw.ElapsedMilliseconds}ms.");
            return listener.Games;
        }


    }
}
