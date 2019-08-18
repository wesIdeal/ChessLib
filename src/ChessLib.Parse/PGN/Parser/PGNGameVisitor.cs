using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using ChessLib.Parse.PGN.Parser.BaseClasses;

namespace ChessLib.Parse.PGN.Parser
{
    internal class PGNGameVisitor : PGNBaseVisitor<string>
    {
        private AntlrInputStream _input;

        public PGNGameVisitor(AntlrInputStream stream)
        {
            _input = stream;
        }

        public override string VisitPgn_game(PGNParser.Pgn_gameContext ctx)
        {
            int a = ctx.Start.StartIndex;
            int b = ctx.Stop.StopIndex;
            var interval = new Interval(a, b);
            var cs = ctx.Start.TokenSource.InputStream;
            return _input.GetText(interval);
        }
    }
}
