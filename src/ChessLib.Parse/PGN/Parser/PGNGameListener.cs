using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.MoveRepresentation.NAG;
using ChessLib.Parse.PGN.Parser.BaseClasses;

namespace ChessLib.Parse.PGN.Parser
{
    internal sealed class PGNGameListener : PGNBaseListener
    {
        public List<string> Games;
        public PGNGameListener()
        {
            Games = new List<string>();
        }

       
        private AntlrInputStream _input;

        public PGNGameListener(AntlrInputStream stream) : this()
        {
            _input = stream;
        }

        public override void EnterPgn_game([NotNull] BaseClasses.PGNParser.Pgn_gameContext ctx)
        {

            int a = ctx.Start.StartIndex;
            int b = ctx.Stop.StopIndex;
            var interval = new Interval(a, b);
            var cs = ctx.Start.TokenSource.InputStream;
           
            var game = _input.GetText(interval);
            Games.Add(game);
        }

        public override void ExitPgn_game([NotNull] BaseClasses.PGNParser.Pgn_gameContext context)
        {

        }

    }
}





