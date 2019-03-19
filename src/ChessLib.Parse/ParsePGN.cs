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
        private readonly PGNLexer pgnLexer;
        private readonly PGNParser pgnParser;
        private readonly ParseTreeWalker treeWalker;
        private readonly PGNParser.ParseContext context;
        private readonly PGNVisitor visitor;

        public ParsePGN(string pgn)
        {
            pgnLexer = new PGNLexer(new AntlrInputStream(pgn));
            pgnParser = new PGNParser(new CommonTokenStream(pgnLexer));
            treeWalker = new ParseTreeWalker();
            context = pgnParser.parse();
            visitor = new PGNVisitor();

        }



        public void GetMovesFromPGN(string pgn)
        {

            visitor.VisitPgn_game(pgnParser.pgn_game());
        }
    }
}
