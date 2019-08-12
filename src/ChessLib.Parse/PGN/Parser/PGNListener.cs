using Antlr4.Runtime.Misc;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Boards;
using ChessLib.Data.MoveRepresentation.NAG;

namespace ChessLib.Parse.PGN.Parser
{
    public sealed class PGNListener : PGNBaseListener
    {
        private string _tagName;
        private readonly List<Tuple<uint, long>> _gamePerformance;
        private readonly Stopwatch _stopwatch;
        private uint _moveCount;
        public List<Game<MoveStorage>> Games;
        public PGNListener()
        {
            Games = new List<Game<MoveStorage>>();
            _gamePerformance = new List<Tuple<uint, long>>();
            _stopwatch = new Stopwatch();
        }

        public double AvgTimePerMove => _gamePerformance.Where(x => x.Item1 != 0).Average(x => x.Item2 / x.Item1);

        public double AvgTimePerGame => _gamePerformance.Where(x => x.Item1 != 0).Select(x => x.Item2).Average();

        public double TotalTime { get; private set; }




        private Game<MoveStorage> CurrentGame { get; set; }
        private MoveNode<MoveStorage> _currentMove;
        private bool _nextMoveIsVariation;

        private MoveTree<MoveStorage> _currentList;

        public override void EnterPgn_database(PGNParser.Pgn_databaseContext context)
        {
            TotalTime = 0;
        }

        public override void EnterPgn_game([NotNull] PGNParser.Pgn_gameContext context)
        {
            CurrentGame = new Game<MoveStorage>();
            _moveCount = 0;
            _currentList = CurrentGame.MoveSection;
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
            _currentMove.MoveData.Comment = context.GetText().Replace("\r\n", " ").TrimEnd('}').TrimStart('{').Trim();
        }

        public override void EnterNag(PGNParser.NagContext context)
        {
            _currentMove.MoveData.Annotation = new NumericAnnotation(context.GetText());
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
            Console.WriteLine($"Applying {moveText}" + (_nextMoveIsVariation ? " as variation." : ""));
            var strategy = _nextMoveIsVariation ? MoveApplicationStrategy.Variation : MoveApplicationStrategy.ContinueMainLine;
            _currentMove = CurrentGame.ApplySANMove(moveText, strategy);
            _nextMoveIsVariation = false;
        }
        public override void EnterRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            _nextMoveIsVariation = true;
        }

        public override void ExitRecursive_variation([NotNull] PGNParser.Recursive_variationContext context)
        {
            CurrentGame.ExitVariation();
        }
    }
}
