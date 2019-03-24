using Antlr4.Runtime.Misc;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.Parser.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace ChessLib.Parse.Parser
{
    class PGNListener : PGNBaseListener
    {
        private string tagName;
        private List<double> times;
        public PGNListener()
        {
            Games = new List<Game<MoveText>>();
            times = new List<double>();
        }

        private Stack<MoveTree<MoveText>> _moveTreeStack = new Stack<MoveTree<MoveText>>();
        private Game<MoveText> _game;
        public List<Game<MoveText>> Games;
        public Game<MoveText> Game { get => _game; }
        private LinkedListNode<MoveNode<MoveText>> _currentMove;
        public double AvgTimePerGame { get => times.Average(); }
        private MoveTree<MoveText> _currentList;
        private DateTime _dtStart;

        public override void EnterPgn_database([NotNull] PGNParser.Pgn_databaseContext context)
        {
            _game = new Game<MoveText>();
        }
        public override void EnterPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            //Debug.WriteLine($"GAME {++gCount}");

            _currentList = _game.MoveSection;
            _dtStart = DateTime.Now;
            base.EnterPgn_game(context);
        }
        public override void ExitPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            times.Add(DateTime.Now.Subtract(_dtStart).TotalMilliseconds);
        }
        public override void EnterTag_section([NotNull] PGNParser.Tag_sectionContext context)
        {
            //Debug.WriteLine("Reading Tags");
        }

        public override void EnterTag_name([NotNull] PGNParser.Tag_nameContext context)
        {
            tagName = context.GetText();
        }

        public override void EnterTag_value([NotNull] PGNParser.Tag_valueContext context)
        {
            var tagVal = context.GetText();
            if (tagName != "")
            {
                _game.TagSection.Add(tagName, tagVal);
            }
            tagName = "";
        }

        public override void ExitTag_section([NotNull] PGNParser.Tag_sectionContext context)
        {
            //Debug.WriteLine("Finished reading tags.");
        }
        public override void EnterMovetext_section([NotNull] PGNParser.Movetext_sectionContext context)
        {
            //Debug.WriteLine("Reading moves");
        }



        public override void ExitMovetext_section([NotNull] PGNParser.Movetext_sectionContext context)
        {
            Games.Add(_game);
            //Debug.WriteLine("Finished reading moves.");
        }
        public override void EnterElement_sequence([NotNull] PGNParser.Element_sequenceContext context)
        {
            Debug.WriteLine($"Reading element sequence {context.ToString()} {context.GetText()}");
        }
        public override void EnterNag_item([NotNull] PGNParser.Nag_itemContext context)
        {

            _currentMove.Value.NAG = context.GetText();
        }
        public override void EnterElement([NotNull] PGNParser.ElementContext context)
        {
            var element = context.GetText();
        }
        public override void EnterSan_move([NotNull] PGNParser.San_moveContext context)
        {
            var moveText = context.GetText();
            _currentMove = _currentList.Add(new MoveNode<MoveText>(new MoveText(moveText)));
        }
        public override void EnterRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            _moveTreeStack.Push(_currentList);
            _currentList = _currentMove.Value.AddVariation();
        }

        public override void ExitRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            //Debug.WriteLine($"Finished Writing Variation:\r\n{_currentList.ToString()}");
            _currentList = _moveTreeStack.Pop();
            _currentMove = _currentList.Last;
        }
    }
}
