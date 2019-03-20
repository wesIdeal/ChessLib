using ChessLib.Parse.PGNPieces;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Parse.Parser.Base;
using ChessLib.Parse.Parser;
using System;

namespace ChessLib.Parse
{
    public class MoveText
    {
        public string Move { get; set; }

    }
    public class ParsePGN
    {
        AntlrFileStream fileStream;
        AntlrInputStream inputStream;
        public const string GameRegEx = @"(?<pgnGame>\s*(?:\[\s*(?<tagName>\w+)\s*""(?<tagValue>[^""]*)""\s*\]\s*)+(?:(?<moveNumber>\d+)(?<moveMarker>\.|\.{3})\s*(?<moveValue>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?)(?:\s*(?<moveValue2>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?))?\s*(?:\(\s*(?<variation>(?:(?<varMoveNumber>\d+)(?<varMoveMarker>\.|\.{3})\s*(?<varMoveValue>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?)(?:\s*(?<varMoveValue2>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?))?\s*(?:\((?<varVariation>.*)\)\s*)?(?:\{(?<varComment>[^\}]*?)\}\s*)?)*)\s*\)\s*)*(?:\{(?<comment>[^\}]*?)\}\s*)?)*(?<endMarker>1\-?0|0\-?1|1/2\-?1/2|\*)?\s*)";
        public ParsePGN(string pgnPath)
        {
            var pgnText = System.IO.File.ReadAllText(pgnPath);
            var splitPgn = Regex.Matches(pgnText, GameRegEx);
           
           
            inputStream = new AntlrInputStream(pgnText);
        }



        public void GetMovesFromPGN()
        {
            TestWalkingListener();
        }
        private void TestWalkingListener()
        {
            PGNLexer lexer = new PGNLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PGNParser(tokens);
            var parseTree = parser.parse();
            var walker = new ParseTreeWalker();
            var listener = new PGNListener();
            walker.Walk(listener, parseTree);
            var moveTexts = listener.Games;
        }
    }
}
