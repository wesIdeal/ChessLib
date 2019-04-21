using Antlr4.Runtime.Misc;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System.Collections.Generic;

namespace ChessLib.Parse.PGN.Parser
{
    public sealed class PGNListener : PGNBaseListener
    {
        private string _tagName;
        public PGNListener()
        {
            Games = new List<Game<IMoveText>>();
        }
        private readonly Stack<MoveTree<IMoveText>> _moveTreeStack = new Stack<MoveTree<IMoveText>>();
        public List<Game<IMoveText>> Games;

        private Game<IMoveText> CurrentGame { get; set; }
        private LinkedListNode<MoveNode<IMoveText>> _currentMove;
        private MoveTree<IMoveText> _currentList;

        public override void EnterPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            CurrentGame = new Game<IMoveText>();
            _currentList = CurrentGame.MoveSection;
        }

        public override void EnterComment(PGNParser.CommentContext context)
        {
            _currentMove.Value.Move.Comment = context.GetText().Replace("\r\n", "");
        }

        public override void EnterNag(PGNParser.NagContext context)
        {
            _currentMove.Value.Move.NAG = context.GetText();
        }

        public override void ExitPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            Games.Add(CurrentGame);
        }

        public override void EnterTag_name([NotNull] PGNParser.Tag_nameContext context)
        {
            _tagName = context.GetText().Replace("\"", "");
        }

        public override void EnterTag_value([NotNull] PGNParser.Tag_valueContext context)
        {
            var tagVal = context.GetText().Replace("\"", "");
            if (!string.IsNullOrWhiteSpace(_tagName))
            {
                CurrentGame.TagSection.Add(_tagName, tagVal);
            }
            _tagName = "";
        }

        public override void EnterSan_move([NotNull] PGNParser.San_moveContext context)
        {
            var moveText = context.GetText();
            _currentMove = _currentList.Add(new MoveNode<IMoveText>(new MoveText(moveText)));
        }
        public override void EnterRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            _moveTreeStack.Push(_currentList);
            _currentList = _currentMove.Value.AddVariation();
        }

        public override void ExitRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            _currentList = _moveTreeStack.Pop();
            _currentMove = _currentList.Last;
        }
    }
}
