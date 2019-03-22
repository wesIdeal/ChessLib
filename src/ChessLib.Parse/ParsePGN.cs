using ChessLib.Parse.PGNPieces;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Parse.Parser.Base;
using ChessLib.Parse.Parser;
using System;
using System.Diagnostics;

namespace ChessLib.Parse
{
    public class MoveText
    {
        public int MoveNumber { get; set; }
        public string Move { get; set; }
        public LinkedList<MoveText> Variations { get; set; }
        public string MoveSAN { get; set; }
        public string PremoveComment { get; set; }
        public string MoveComment { get; set; }
    }
    public class ParsePGN
    {
        AntlrFileStream fileStream;
        public string PgnDatabase;
        AntlrInputStream inputStream;
        public const string GameRegEx = @"(?<pgnGame>\s*(?:\[\s*(?<tagName>\w+)\s*""(?<tagValue>[^""]*)""\s*\]\s*)+(?:(?<moveNumber>\d+)(?<moveMarker>\.|\.{3})\s*(?<moveValue>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?)(?:\s*(?<moveValue2>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?))?\s*(?:\s*\(\s*(?<variation>(?:(?<varMoveNumber>\d+)(?<varMoveMarker>\.|\.{3})\s*(?<varMoveValue>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?)(?:\s*(?<varMoveValue2>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?))?\s*(?:\((?<varVariation>.*)\s*\)\s*)?(?:\{(?<varComment>[^\}]*?)\}\s*)?)*)\s*\)\s*)*(?:\{(?<comment>[^\}]*?)\}\s*)?)*(?<endMarker>1\-?0|0\-?1|1/2\-?1/2|\*)?\s*)";
        public ParsePGN(string pgnPath)
        {
            //Debug.WriteLine(PgnDatabase);
            var whiteSpace = "(\\s)+";
            var openParen = "(\\()";
            var closeParn = "(\\))+";
            var regOpenParen = $"{whiteSpace}(?<var>{openParen})";
            var regCloseParn = $"{whiteSpace}(?<var>{closeParn}){whiteSpace}";
            PgnDatabase = Regex.Replace(PgnDatabase, @"((\r\n)|\n){2,}", "##BREAK##");
            PgnDatabase = PgnDatabase.Replace(Environment.NewLine, " ");
            PgnDatabase = PgnDatabase.Replace("##BREAK##", Environment.NewLine + Environment.NewLine);
            PgnDatabase = Regex.Replace(PgnDatabase, @"(\t|\f){2,}", " ");

            //PgnDatabase = Regex.Replace(PgnDatabase, $"{whiteSpace}{regCloseParn}", " ${var} ", RegexOptions.RightToLeft);
            //PgnDatabase = Regex.Replace(PgnDatabase, regOpenParen, " ${var} ");
            ////PgnDatabase = Regex.Replace(PgnDatabase, @"\)(\n)*(\s)*(?<moveNumber>[\d]+)", ") ${moveNumber}", RegexOptions.RightToLeft);

            Debug.WriteLine(PgnDatabase);
        }



        public void GetMovesFromPGN()
        {
            TestWalkingListener();
        }
        private void TestWalkingListener()
        {

            var splitPgn = Regex.Matches(PgnDatabase, GameRegEx);

            foreach (Match game in splitPgn)
            {
                MatchCollection mc = Regex.Matches(game.Value, GameRegEx);
                var ms = mc[0].Groups["moveNumber"];
                Debug.WriteLine(game.Value);
                //gamePgn = Regex.Replace(gamePgn, @"(\n+\s*\))", ") ");
                inputStream = new AntlrInputStream(game.Value);
                PGNLexer lexer = new PGNLexer(inputStream);
                var tokens = new CommonTokenStream(lexer);
                var parser = new PGNParser(tokens);
                var parseTree = parser.parse();
                var walker = new ParseTreeWalker();
                var listener = new PGNListener();
                walker.Walk(listener, parseTree);
                var moves = listener.Moves;

            }
        }
    }
}
