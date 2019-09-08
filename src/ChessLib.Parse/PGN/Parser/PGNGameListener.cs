using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        private CancellationToken _cancellationToken = CancellationToken.None;
        public event EventHandler<int> BatchProcessed;
        private int _gamesToProcessBeforeUpdate = 20;
        private int _gamesProcessed = 0;
        public PGNGameListener()
        {
            Games = new List<string>();
        }


        private AntlrInputStream _input;

        public PGNGameListener(AntlrInputStream stream, CancellationToken cancellationToken) : this()
        {
            _input = stream;
            _cancellationToken = cancellationToken;
        }

        public override void EnterPgn_game([NotNull] BaseClasses.PGNParser.Pgn_gameContext ctx)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                throw new ParseCanceledException("Cancellation requested.");
            }

            int a = ctx.Start.StartIndex;
            int b = ctx.Stop.StopIndex;
            var interval = new Interval(a, b);
            var cs = ctx.Start.TokenSource.InputStream;
            var game = _input.GetText(interval);
            Games.Add(game);
            _gamesProcessed++;
            if (BatchProcessed != null && _gamesProcessed >= _gamesToProcessBeforeUpdate)
            {
                BatchProcessed.Invoke(this, Games.Count);
                _gamesProcessed = 0;
            }
        }

        public override void ExitPgn_game([NotNull] BaseClasses.PGNParser.Pgn_gameContext context)
        {

        }

    }
}





