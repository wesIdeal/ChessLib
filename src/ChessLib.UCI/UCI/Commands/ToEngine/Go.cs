using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.EngineInterface.UCI.Commands.ToEngine
{
    public class Go : CommandInfo
    {

        public bool IgnoreCalculationInformationLines { get; }

        private IEnumerable<MoveExt> _searchMoves;
        private int? _depth;
        private int? _nodes;
        private TimeSpan? _moveTime;
        private string _commandText;

        public Go(TimeSpan timeSpan, IEnumerable<MoveExt> searchMoves = null) : this(timeSpan, null, searchMoves, null, true)
        {

        }

        public Go(TimeSpan? moveTime = null, int? depth = null, IEnumerable<MoveExt> searchMoves = null,
            int? nodes = null, bool ignoreMoveCalculationLines = true) : base(AppToUCICommand.Go)
        {
            IgnoreCalculationInformationLines = ignoreMoveCalculationLines;
            _searchMoves = searchMoves;
            _depth = depth;
            _nodes = nodes;
            _moveTime = moveTime;
            _commandText = $"go {GetMoveTimeParameter()}{GetSearchMovesParameter()}{GetDepthParameter()}{GetNodesParameter()}";

        }


        private string GetDepthParameter() => _depth.HasValue ? $" depth {_depth.Value}" : "";
        private string GetNodesParameter() => _nodes.HasValue ? $" nodes {_nodes.Value}" : "";
        private string GetSearchMovesParameter() =>
            _searchMoves != null && _searchMoves.Any() ? $" searchmoves {_searchMoves.UCIMovesFromMoveObjects()}" : "";
        private string GetMoveTimeParameter() => " " + _moveTime == null ? "infinite" : $"movetime {_moveTime.Value.TotalMilliseconds}";
        public new string ToString() => _commandText;
    }

}
