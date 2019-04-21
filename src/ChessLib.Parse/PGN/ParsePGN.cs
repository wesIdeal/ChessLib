using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChessLib.Parse.PGN
{
    public class Game<T> where T : IEquatable<T>
    {
        public Tags TagSection = new Tags();
        public MoveTree<T> MoveSection = new MoveTree<T>(null);
    }
    public class ParsePGN
    {
        public string PgnDatabase;
        AntlrInputStream inputStream;
        public const string GameRegEx = @"(?<pgnGame>\s*(?:\[\s*(?<tagName>\w+)\s*""(?<tagValue>[^""]*)""\s*\]\s*)+(?:(?<moveNumber>\d+)(?<moveMarker>\.|\.{3})\s*(?<moveValue>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?)(?:\s*(?<moveValue2>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?))?\s*(?:\s*\(\s*(?<variation>(?:(?<varMoveNumber>\d+)(?<varMoveMarker>\.|\.{3})\s*(?<varMoveValue>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?)(?:\s*(?<varMoveValue2>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?))?\s*(?:\((?<varVariation>.*)\s*\)\s*)?(?:\{(?<varComment>[^\}]*?)\}\s*)?)*)\s*\)\s*)*(?:\{(?<comment>[^\}]*?)\}\s*)?)*(?<endMarker>1\-?0|0\-?1|1/2\-?1/2|\*)?\s*)";

        public ParsePGN()
        {
        }

        public ParsePGN(string pgnPath)
        {
            PgnDatabase = File.ReadAllText(pgnPath);

        }

        public static ParsePGN MakeParser(string p)
        {
            return new ParsePGN() { PgnDatabase = p };
        }

        private void SanitizePgnFile()
        {
            var whiteSpace = "(\\s)+";
            var openParen = "(\\()";
            var closeParn = "(\\))+";
            var regOpenParen = $"{whiteSpace}(?<var>{openParen})";
            var regCloseParn = $"{whiteSpace}(?<var>{closeParn}){whiteSpace}";
            PgnDatabase = Regex.Replace(PgnDatabase, @"((\r\n)|\n){2,}", "##BREAK##");
            PgnDatabase = PgnDatabase.Replace(Environment.NewLine, " ");
            PgnDatabase = PgnDatabase.Replace("##BREAK##", Environment.NewLine);
            PgnDatabase = Regex.Replace(PgnDatabase, @"(\ {2,})", " ");
            PgnDatabase = Regex.Replace(PgnDatabase, @"(?<b>\S)(?<p>\))", @"${b} ${p}");

        }



        public List<Game<MoveText>> GetGameObjects()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var listener = new PGNListener();
            inputStream = new AntlrInputStream(PgnDatabase);
            PGNLexer lexer = new PGNLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PGNParser(tokens);
            var parseTree = parser.parse();
            var walker = new ParseTreeWalker();
            walker.Walk(listener, parseTree);
            sw.Stop();
            Debug.WriteLine($"Completed parsing {listener.Games.Count} games, {listener.Games.Select(x=>x.MoveSection.Count()).Sum()} half-moves total, in {sw.ElapsedMilliseconds}ms.");
            return listener.Games;
        }


    }
}
