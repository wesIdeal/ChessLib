using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Misc;
using ChessLib.Parse.Parser.Base;
namespace ChessLib.Parse.Parser
{
    class PGNVisitor : PGNBaseVisitor<object>
    {
        public override object VisitPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            return base.VisitPgn_game(context);
        }
    }
}
