using Antlr4.Runtime.Misc;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Antlr4.Runtime;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Boards;
using ChessLib.Data.MoveRepresentation.NAG;
using ChessLib.Data.Helpers;

namespace ChessLib.Parse.PGN.Parser
{
    internal sealed class PGNGameDetailListener : PGNBaseListener
    {
        private Tags _currentTags;
        private string _tagName;
        private readonly List<Tuple<uint, long>> _gamePerformance;
        private readonly Stopwatch _stopwatch;
        private uint _moveCount;
        public List<Game<MoveStorage>> Games;
        public PGNGameDetailListener()
        {
            Games = new List<Game<MoveStorage>>();
            _gamePerformance = new List<Tuple<uint, long>>();
            _stopwatch = new Stopwatch();
        }

        public double AvgTimePerMove => _gamePerformance.Where(x => x.Item1 != 0).Average(x => x.Item2 / x.Item1);

        public double AvgTimePerGame => _gamePerformance.Where(x => x.Item1 != 0).Select(x => x.Item2).Average();

        public double TotalTime { get; private set; }




        private Game<MoveStorage> CurrentGame { get; set; }
        public event EventHandler GameProcessed;
        private LinkedListNode<MoveStorage> _currentMove;
        private bool _nextMoveIsVariation;

        private MoveTree _currentList;

        public PGNGameDetailListener(CommonTokenStream tokens)
        {
            throw new NotImplementedException();
        }

        public override void EnterPgn_database(BaseClasses.PGNParser.Pgn_databaseContext context)
        {
            TotalTime = 0;
        }

        public override void EnterPgn_game([NotNull] BaseClasses.PGNParser.Pgn_gameContext context)
        {
            CurrentGame = new Game<MoveStorage>();
            _moveCount = 0;
            _currentList = CurrentGame.MainMoveTree;
        }
        public override void EnterMovetext_section([NotNull] BaseClasses.PGNParser.Movetext_sectionContext context)
        {
            _moveCount = 0;
            _currentList = CurrentGame.MainMoveTree;
        }

        public override void ExitPgn_game([NotNull] BaseClasses.PGNParser.Pgn_gameContext context)
        {
            _stopwatch.Stop();
            _gamePerformance.Add(new Tuple<uint, long>(_moveCount, _stopwatch.ElapsedMilliseconds));
            TotalTime += _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Reset();
            CurrentGame.GoToInitialState();
            Games.Add(CurrentGame);
            _currentTags = null;
            GameProcessed?.Invoke(this, EventArgs.Empty);
        }

        public override void EnterComment(BaseClasses.PGNParser.CommentContext context)
        {
            _currentMove.Value.Comment = context.GetText().Replace("\r\n", " ").TrimEnd('}').TrimStart('{').Trim();
        }

        public override void EnterNag(BaseClasses.PGNParser.NagContext context)
        {
            _currentMove.Value.Annotation = new NumericAnnotation(context.GetText());
        }

        public override void EnterTag_name([NotNull] BaseClasses.PGNParser.Tag_nameContext context)
        {
            _tagName = context.GetText().Replace("\"", "");
        }

        public override void EnterTag_value([NotNull] BaseClasses.PGNParser.Tag_valueContext context)
        {
            var tagVal = context.GetText().Replace("\"", "");
            if (!string.IsNullOrWhiteSpace(_tagName))
            {
                CurrentGame.TagSection.Add(_tagName, tagVal);
            }
            _tagName = "";
        }

        public override void EnterSan_move([NotNull] BaseClasses.PGNParser.San_moveContext context)
        {
            var moveText = context.GetText();
            var strategy = _nextMoveIsVariation ? MoveApplicationStrategy.Variation : MoveApplicationStrategy.ContinueMainLine;
            _currentMove = CurrentGame.ApplySANMove(moveText, strategy);
            _nextMoveIsVariation = false;
        }
        public override void EnterRecursive_variation([NotNull] BaseClasses.PGNParser.Recursive_variationContext context)
        {
            _nextMoveIsVariation = true;
        }

        public override void ExitRecursive_variation([NotNull] BaseClasses.PGNParser.Recursive_variationContext context)
        {
            CurrentGame.ExitVariation();
        }
    }
}





