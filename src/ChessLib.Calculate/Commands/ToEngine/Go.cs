using ChessLib.Data.MoveRepresentation;
using System;
using System.Linq;

namespace ChessLib.UCI.Commands.ToEngine
{
    public class Go : CommandInfo
    {
        public readonly PrincipalVariationReceivedHandler AnalysisEventHandler;
        public readonly BestMoveReceivedHandler BestMoveEventHandler;
        public bool IgnoreCalculationInformationLines { get; }

        private MoveExt[] _searchMoves;
        private int? _depth;
        private int? _nodes;
        private TimeSpan? _moveTime;
        private string _commandText;

        public Go(PrincipalVariationReceivedHandler onVariationAnalysis, BestMoveReceivedHandler onBestMove,
            bool ignoreMoveCalculationLines = true, MoveExt[] searchMoves = null,
            int? depth = null, int? nodes = null, TimeSpan? moveTime = null) : base(AppToUCICommand.Go)
        {
            AnalysisEventHandler = onVariationAnalysis;
            BestMoveEventHandler = onBestMove;
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
