using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ChessLib.Parse.Parser.Base;
namespace ChessLib.Parse.Parser
{
    class PGNListener : PGNBaseListener
    {
        public int plyCount = 0;
        public int variationDepth = 0;
        public StringBuilder moves = new StringBuilder();
        public PGNListener()
        {
            Moves = new LinkedList<MoveText>();
            
        }

        public LinkedList<MoveText> Moves { get; set; }
        protected LinkedList<MoveText> CurrentMoveList;
        protected MoveText CurrentMove;
        protected MoveText ParentMove;
        protected LinkedList<MoveText> ParentMoveList = null;
        public override void EnterPgn_database([NotNull] PGNParser.Pgn_databaseContext context)
        {
            var g = context.GetText();
            CurrentMoveList = Moves;
        }



        public override void EnterMovetext_section([NotNull] PGNParser.Movetext_sectionContext context)
        {
            
        }
        public override void EnterElement_sequence([NotNull] PGNParser.Element_sequenceContext context)
        {

        }
        public override void VisitErrorNode([NotNull] IErrorNode node)
        {
            base.VisitErrorNode(node);
        }
        public override void EnterSan_move([NotNull] PGNParser.San_moveContext context)
        {
            CurrentMoveList.AddLast(new MoveText() { Move = context.GetText() });
            CurrentMove = CurrentMoveList.Last();
            var depth = new string('\t', variationDepth);
            moves.AppendLine(depth + context.GetText());

        }
        public override void EnterRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            CurrentMove.Variations = new LinkedList<MoveText>();
            ParentMove = CurrentMove;
            ParentMoveList = CurrentMoveList;
            CurrentMoveList = CurrentMove.Variations;
            variationDepth++;
        }

        public override void ExitRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            CurrentMoveList = ParentMoveList;
            CurrentMove = ParentMove;
            variationDepth--;
        }
    }
}
