using System;
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

        public override void EnterTag_section([NotNull] PGNParser.Tag_sectionContext context)
        {
            Debug.WriteLine("Reading Tags");
        }
        public override void ExitTag_section([NotNull] PGNParser.Tag_sectionContext context)
        {
            Debug.WriteLine("Finished reading tags.");
        }
        public override void EnterMovetext_section([NotNull] PGNParser.Movetext_sectionContext context)
        {
            Debug.WriteLine("Reading moves");
        }
        public override void ExitMovetext_section([NotNull] PGNParser.Movetext_sectionContext context)
        {
            Debug.WriteLine("Finished reading moves.");
        }
        public override void EnterElement_sequence([NotNull] PGNParser.Element_sequenceContext context)
        {
            Debug.WriteLine($"Reading element sequence {context.ToString()} {context.GetText()}");
        }

        public override void VisitErrorNode([NotNull] IErrorNode node)
        {
            Debug.WriteLine($"ERROR at depth {variationDepth}. Parent node was {node.Parent.GetText()}:\t" + node.GetText() + Environment.NewLine + node.Parent.GetText());
            base.VisitErrorNode(node);
        }
        public override void EnterSan_move([NotNull] PGNParser.San_moveContext context)
        {
            var moveText = context.GetText();
            Debug.WriteLine($"\tReading move {moveText}");
            CurrentMoveList.AddLast(new MoveText() { Move = moveText });
            CurrentMove = CurrentMoveList.Last();
            var depth = new string('-', variationDepth);
            moves.AppendLine(depth + moveText);

        }
        public override void EnterRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            ParentMove = CurrentMove;
            ParentMoveList = CurrentMoveList;
            
            CurrentMove.Variations.AddLast(new LinkedList<MoveText>());
            CurrentMoveList = CurrentMove.Variations.Last();
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
