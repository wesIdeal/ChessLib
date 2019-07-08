using Antlr4.Runtime.Misc;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Parse.PGN.Parser
{
    public sealed class PGNListener : PGNBaseListener
    {
        private string _tagName;
        private readonly List<Tuple<uint, long>> _gamePerformance;
        private readonly Stopwatch _stopwatch;
        private uint _moveCount;
        public PGNListener()
        {

            Games = new List<Game<IMoveText>>();
            _gamePerformance = new List<Tuple<uint, long>>();
            _stopwatch = new Stopwatch();
        }

        public double AvgTimePerMove => _gamePerformance.Where(x => x.Item1 != 0).Average(x => x.Item2 / x.Item1);

        public double AvgTimePerGame => _gamePerformance.Where(x => x.Item1 != 0).Select(x => x.Item2).Average();

        public double TotalTime { get; private set; }


        private readonly Stack<MoveTree<IMoveText>> _moveTreeStack = new Stack<MoveTree<IMoveText>>();
        public List<Game<IMoveText>> Games;

        private Game<IMoveText> CurrentGame { get; set; }
        private IMoveNode<IMoveText> _currentMove;
        private MoveTree<IMoveText> _currentList;

        public override void EnterPgn_database(PGNParser.Pgn_databaseContext context)
        {
            TotalTime = 0;
        }

        public override void EnterPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            CurrentGame = new Game<IMoveText>();
            _moveCount = 0;
            _currentList = (MoveTree<IMoveText>)CurrentGame.MoveSection;
            _stopwatch.Start();
        }
        public override void ExitPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            _stopwatch.Stop();
            _gamePerformance.Add(new Tuple<uint, long>(_moveCount, _stopwatch.ElapsedMilliseconds));
            TotalTime += _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Reset();
            Games.Add(CurrentGame);
        }

        public override void EnterComment(PGNParser.CommentContext context)
        {
            _currentMove.MoveData.Comment = context.GetText().Replace("\r\n", "");
        }

        public override void EnterNag(PGNParser.NagContext context)
        {
            _currentMove.MoveData.NAG = context.GetText();
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
            _moveCount++;
            _currentMove = _currentList.AddMove((IMoveText)new MoveText(moveText));
        }
        public override void EnterRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            _moveTreeStack.Push(_currentList);
            _currentList = (MoveTree<IMoveText>)_currentMove.AddVariation();
        }

        public override void ExitRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            _currentList = _moveTreeStack.Pop();
            _currentMove = _currentList.LastMove;
        }
    }
}
