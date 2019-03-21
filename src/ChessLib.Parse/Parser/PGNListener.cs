using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime.Misc;
using ChessLib.Parse.Parser.Base;
namespace ChessLib.Parse.Parser
{
    class PGNListener : PGNBaseListener
    {
        public int plyCount = 0;
        public int variationDepth = 0;
        public PGNListener()
        {
            Moves = new List<MoveText>();
            CurrentMoveList = Moves;
        }

        public List<MoveText> Moves { get; set; }
        protected List<MoveText> CurrentMoveList;
        protected List<MoveText> ParentMoveList = null;
        public override void EnterPgn_database([NotNull] PGNParser.Pgn_databaseContext context)
        {
            var g = context.GetText();
        }



        public override void EnterMovetext_section([NotNull] PGNParser.Movetext_sectionContext context)
        {

        }
        public override void EnterElement_sequence([NotNull] PGNParser.Element_sequenceContext context)
        {

        }

        public override void EnterSan_move([NotNull] PGNParser.San_moveContext context)
        {
            CurrentMoveList.Add(new MoveText() { Move = context.GetText(), Variations = new List<List<MoveText>>() });
            Debug.WriteLine(new string('\t', variationDepth) + context.GetText());
        }
        public override void EnterRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {

            CurrentMoveList.Last().Variations.Add(new List<MoveText>());
            variationDepth++;
            ParentMoveList = CurrentMoveList;
            CurrentMoveList = CurrentMoveList.Last().Variations.Last();
        }
        public override void ExitRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            CurrentMoveList = ParentMoveList;
            variationDepth--;
        }
    }
}
