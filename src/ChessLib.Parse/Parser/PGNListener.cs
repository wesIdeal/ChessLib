using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using ChessLib.Parse.Parser.Base;
namespace ChessLib.Parse.Parser
{
    class PGNListener : PGNBaseListener
    {
        public int gameCount = 1;

        public PGNListener()
        {
            Games = new List<string>();
        }

        public List<string> Games { get; set; }
        public override void EnterPgn_database([NotNull] PGNParser.Pgn_databaseContext context)
        {


        }
        public override void EnterPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            gameCount++;
            base.EnterPgn_game(context);
        }

        public override void EnterMovetext_section([NotNull] PGNParser.Movetext_sectionContext context)
        {

        }
        public override void EnterElement_sequence([NotNull] PGNParser.Element_sequenceContext context)
        {
            var text = context.GetText();
            if (!string.IsNullOrWhiteSpace(text))
                Games.Add(context.GetText());
        }

    }
}
