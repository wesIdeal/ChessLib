using ChessLib.Data.MoveRepresentation;
using System;

namespace ChessLib.UCI.Commands.FromEngine
{
    public interface IInfoResponse : IEngineResponse
    {
        uint Depth { get; }
        string FromFEN { get; }
        string SAN { get; }
    }

    public interface IBestMoveResponse : IEngineResponse
    {
        MoveExt BestMove { get; }
        MoveExt PonderMove { get; }
        string BestMoveSan { get; }
        string PonderMoveSan { get; }

        string ToString();
    }

    public interface IInfoCalculationResponse : IInfoResponse
    {
        MoveExt CurrentMove { get; }
        uint CurrentMoveNumber { get; }
    }

    public enum Bound { None, Lower, Upper }

    public interface IInfoScore
    {
        short? CentipawnScore { get; set; }
        short? MateInXMoves { get; set; }
        Bound Bound { get; set; }
    }

    public interface IPrincipalVariationResponse : IInfoResponse
    {
        uint SelectiveSearchDepth { get; }
        ushort PVOrdinal { get; }
        IInfoScore Score { get; }
        ulong Nodes { get; }
        ulong NodesPerSecond { get; }
        ushort TableBaseHits { get; }
        TimeSpan SearchTime { get; }
        MoveExt[] Variation { get; }
    }

    public class InfoCalculationResponse : EngineResponseArgs, IInfoCalculationResponse
    {
        public InfoCalculationResponse(uint depth, uint currentMoveNumber, MoveExt currentMove, string fromFEN, string san, string engineResponse) : base(engineResponse)
        {
            Depth = depth;
            CurrentMoveNumber = currentMoveNumber;
            CurrentMove = currentMove;
            FromFEN = fromFEN;
        }

        public bool MovesValidated { get => FromFEN != string.Empty; }
        public MoveExt CurrentMove { get; private set; }
        public uint CurrentMoveNumber { get; private set; }
        public uint Depth { get; private set; }
        public string FromFEN { get; }
        public string SAN { get; }
    }

    public class PrincipalVariationScore : IInfoScore
    {
        public PrincipalVariationScore()
        {
        }

        public PrincipalVariationScore(short? centipawnScore, short? mateInXMoves, Bound bound)
        {
            CentipawnScore = centipawnScore;
            MateInXMoves = mateInXMoves;
            Bound = bound;
        }

        public short? CentipawnScore { get; set; }

        public short? MateInXMoves { get; set; }

        public Bound Bound { get; set; }

        public override string ToString()
        {
            var integerValue = CentipawnScore.HasValue ? CentipawnScore.Value / 100 : 0;
            var floatLength = integerValue.ToString().Length + 3;
            return (MateInXMoves.HasValue ? $"M{MateInXMoves.Value}" :
                    CentipawnScore.HasValue ? $"{(float)CentipawnScore.Value / 100}".PadRight(floatLength, '0') : "");
        }
    }

    public class PrincipalVariationResponse : EngineResponseArgs, IPrincipalVariationResponse
    {
        public PrincipalVariationResponse(ushort pVOrdinal, MoveExt[] variation, IInfoScore score, uint selectiveSearchDepth,
            TimeSpan searchTime, ulong nodes, ulong nodesPerSecond, ushort tableBaseHits, uint depth,
            string fromFEN, string san, string response) : base(response)
        {
            PVOrdinal = pVOrdinal;
            Score = score;
            SelectiveSearchDepth = selectiveSearchDepth;
            SearchTime = searchTime;
            Nodes = nodes;
            NodesPerSecond = nodesPerSecond;
            TableBaseHits = tableBaseHits;
            Depth = depth;
            FromFEN = fromFEN;
            Variation = variation;
            SAN = san;
        }

        public uint SelectiveSearchDepth { get; private set; }
        public ushort PVOrdinal { get; private set; }
        public IInfoScore Score { get; private set; }
        public TimeSpan SearchTime { get; private set; }
        public ulong Nodes { get; private set; }
        public ulong NodesPerSecond { get; private set; }
        public ushort TableBaseHits { get; private set; }
        public uint Depth { get; private set; }
        public string FromFEN { get; private set; }
        public string SAN { get; }
        public MoveExt[] Variation { get; private set; }
    }

    public class BestMoveResponse : EngineResponseArgs, IBestMoveResponse
    {
        public BestMoveResponse(MoveExt bestMove, MoveExt ponderMove, string bestMoveSan, string ponderMoveSan, string engineResponse)
            : base(engineResponse)
        {
            BestMove = bestMove;
            PonderMove = ponderMove;
            BestMoveSan = bestMoveSan;
            PonderMoveSan = ponderMoveSan;
        }

        public MoveExt BestMove { get; private set; }

        public MoveExt PonderMove { get; private set; }

        public string BestMoveSan { get; private set; }

        public string PonderMoveSan { get; private set; }

        public override string ToString() => BestMoveSan + " " + PonderMoveSan;
    }
}
