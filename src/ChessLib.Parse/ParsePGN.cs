using ChessLib.Parse.PGNPieces;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Parse.Parser.Base;
using ChessLib.Parse.Parser;

namespace ChessLib.Parse
{
    public class MoveText
    {
        public string Move { get; set; }

    }
    public class ParsePGN
    {
        AntlrFileStream fileStream;
        public ParsePGN(string pgnPath)
        {
            fileStream = new AntlrFileStream(pgnPath);
        }



        public void GetMovesFromPGN()
        {
            TestWalkingListener();
        }
        private void TestWalkingListener()
        {
            PGNLexer lexer = new PGNLexer(fileStream);
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
