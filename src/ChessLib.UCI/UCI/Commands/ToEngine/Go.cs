using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.EngineInterface.UCI.Commands.ToEngine
{
    public class Go : CommandInfo
    {

        public bool IgnoreCalculationInformationLines { get; }

        private readonly IEnumerable<MoveExt> _searchMoves;
        private readonly int? _depth;
        private readonly int? _nodes;
        private readonly TimeSpan? _moveTime;
        private readonly string _commandText;

        public Go(TimeSpan timeSpan, IEnumerable<MoveExt> searchMoves = null) : this(timeSpan, null, searchMoves)
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
        private string GetMoveTimeParameter()
        {
            if (_moveTime != null)
            {
                return $" movetime {_moveTime.Value.TotalMilliseconds}";
            }

            return "";
        }

        public new string ToString() => _commandText;
    }

}
