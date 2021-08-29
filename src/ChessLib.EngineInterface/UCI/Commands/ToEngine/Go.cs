using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core;
using ChessLib.Core.Types;

namespace ChessLib.EngineInterface.UCI.Commands.ToEngine
{
    public class Go : CommandInfo
    {

        public bool IgnoreCalculationInformationLines { get; }

        private readonly IEnumerable<Move> _searchMoves;
        private readonly int? _depth;
        private readonly int? _nodes;
        private readonly TimeSpan? _moveTime;
        private readonly string _commandText;

        public Go(TimeSpan timeSpan, IEnumerable<Move> searchMoves = null) : this(timeSpan, null, searchMoves)
        {

        }

        public Go(): base(AppToUCICommand.Go)
        {
            _commandText = "go infinite";
        }

        public Go(TimeSpan? moveTime = null, int? depth = null, IEnumerable<Move> searchMoves = null,
            int? nodes = null, bool ignoreMoveCalculationLines = true) : base(AppToUCICommand.Go)
        {
            IgnoreCalculationInformationLines = ignoreMoveCalculationLines;
            _searchMoves = searchMoves;
            if (moveTime == null && depth == null && nodes == null)
            {
                _commandText = "go infinite";
                return;
            }
            _depth = depth;
            _nodes = nodes;
            _moveTime = moveTime;
            _commandText = $"go {GetMoveTimeParameter()}{GetSearchMovesParameter()}{GetDepthParameter()}{GetNodesParameter()}";

        }


        private string GetDepthParameter() => _depth.HasValue ? $" depth {_depth.Value}" : "";
        private string GetNodesParameter() => _nodes.HasValue ? $" nodes {_nodes.Value}" : "";
        private string GetSearchMovesParameter() =>
            _searchMoves != null && _searchMoves.Any() ? $" searchmoves {_searchMoves.UCIMovesFromMoveObjects()}" : "";
        private string GetMoveTimeParameter()
        {
            if (_moveTime != null)
            {
                return $" movetime {_moveTime.Value.TotalMilliseconds}";
            }

            return "";
        }

        public override string ToString() => _commandText;
    }

}
