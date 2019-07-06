using ChessLib.Data.MoveRepresentation;
using System;

namespace ChessLib.UCI.Commands.FromEngine
{
    public interface IInfoResponse : IResponseObject
    {
        uint Depth { get; }
        string FromFEN { get; }
        string SAN { get; }
    }

    public interface IBestMoveResponse : IResponseObject
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

    public class InfoCalculationResponse : IInfoCalculationResponse
    {
        public InfoCalculationResponse(uint depth, uint currentMoveNumber, MoveExt currentMove, string fromFEN, string san)
        {
            Depth = depth;
            CurrentMoveNumber = currentMoveNumber;
            CurrentMove = currentMove;
            FromFEN = fromFEN;
        }

        public bool MovesValidated { get => FromFEN != string.Empty; }
        public MoveExt CurrentMove { get; }
        public uint CurrentMoveNumber { get; }
        public uint Depth { get; }
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

    public class PrincipalVariationResponse : IPrincipalVariationResponse
    {
        public PrincipalVariationResponse(ushort pVOrdinal, MoveExt[] variation, IInfoScore score, uint selectiveSearchDepth,
            TimeSpan searchTime, ulong nodes, ulong nodesPerSecond, ushort tableBaseHits, uint depth,
            string fromFEN, string san)
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

        public uint SelectiveSearchDepth { get; }
        public ushort PVOrdinal { get; }
        public IInfoScore Score { get; }
        public TimeSpan SearchTime { get; }
        public ulong Nodes { get; }
        public ulong NodesPerSecond { get; }
        public ushort TableBaseHits { get; }
        public uint Depth { get; }
        public string FromFEN { get; }
        public string SAN { get; }
        public MoveExt[] Variation { get; }
    }

    public class BestMoveResponse : IResponseObject, IBestMoveResponse
    {
        public BestMoveResponse(MoveExt bestMove, MoveExt ponderMove, string bestMoveSan, string ponderMoveSan)
        {
            BestMove = bestMove;
            PonderMove = ponderMove;
            BestMoveSan = bestMoveSan;
            PonderMoveSan = ponderMoveSan;
        }

        public MoveExt BestMove { get; }

        public MoveExt PonderMove { get; }

        public string BestMoveSan { get; }

        public string PonderMoveSan { get; }

        public override string ToString() => BestMoveSan + " " + PonderMoveSan;
    }
}
