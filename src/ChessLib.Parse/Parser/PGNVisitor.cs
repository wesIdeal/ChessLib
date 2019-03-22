using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Misc;
using ChessLib.Parse.Parser.Base;
namespace ChessLib.Parse.Parser
{
    class PGNVisitor : PGNBaseVisitor<object>
    {
        public List<string> Games { get; set; }
        public override object VisitPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            var gameText = context.GetText();
            Games.Add(gameText);
            return gameText;
        }
        public override object VisitRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            return context.GetText();
        }
    }
}
