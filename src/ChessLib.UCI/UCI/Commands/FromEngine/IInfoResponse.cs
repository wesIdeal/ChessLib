using System;
using System.Linq;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.EngineInterface.UCI.Commands.FromEngine
{
    public interface ICalculationInfoResponse : IResponseObject
    {
        CalculationResponseTypes ResponseType { get; }
    }
    public interface IInfoResponse : ICalculationInfoResponse
    {
        uint Depth { get; }
        string FromFEN { get; }
    }

    public interface IBestMoveResponse : ICalculationInfoResponse
    {
        string BestMoveLong { get; }
        MoveExt BestMove { get; set; }
        string PonderMoveLong { get; }
        MoveExt PonderMove { get; set; }
        string ToString();
    }

    public interface IInfoCalculationResponse : IInfoResponse
    {
        string CurrentMoveLong { get; }
        MoveExt CurrentMove { get; set; }
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
        string[] VariationLong { get; }
        MoveExt[] Variation { get; set; }
    }

    public class InfoCalculationResponse : IInfoCalculationResponse
    {
        public CalculationResponseTypes ResponseType => CalculationResponseTypes.CalculationInformation;
        public InfoCalculationResponse(uint depth, uint currentMoveNumber, string currentMoveLong)
        {
            Depth = depth;
            CurrentMoveNumber = currentMoveNumber;
            CurrentMoveLong = currentMoveLong;
        }
        public MoveExt CurrentMove { get; set; }
        public bool MovesValidated => FromFEN != string.Empty;
        public string CurrentMoveLong { get; set; }
        public uint CurrentMoveNumber { get; set; }
        public uint Depth { get; }
        public string FromFEN { get; set; }
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
            var integerValue = CentipawnScore / 100 ?? 0;
            var floatLength = integerValue.ToString().Length + 3;
            return (MateInXMoves.HasValue ? $"M{MateInXMoves.Value}" :
                    CentipawnScore.HasValue ? $"{(float)CentipawnScore.Value / 100}".PadRight(floatLength, '0') : "");
        }
    }

    public class PrincipalVariationResponse : IPrincipalVariationResponse
    {
        public CalculationResponseTypes ResponseType => CalculationResponseTypes.PrincipalVariation;
        public PrincipalVariationResponse(ushort pVOrdinal, string[] variation, IInfoScore score, uint selectiveSearchDepth,
            TimeSpan searchTime, ulong nodes, ulong nodesPerSecond, ushort tableBaseHits, uint depth)
        {
            PVOrdinal = pVOrdinal;
            Score = score;
            SelectiveSearchDepth = selectiveSearchDepth;
            SearchTime = searchTime;
            Nodes = nodes;
            NodesPerSecond = nodesPerSecond;
            TableBaseHits = tableBaseHits;
            Depth = depth;
            VariationLong = variation;
        }

        public uint SelectiveSearchDepth { get; }
        public ushort PVOrdinal { get; }
        public IInfoScore Score { get; }
        public TimeSpan SearchTime { get; }
        public ulong Nodes { get; }
        public ulong NodesPerSecond { get; }
        public ushort TableBaseHits { get; }
        public uint Depth { get; }
        public string FromFEN { get; set; }
        public MoveExt[] Variation { get; set; }
        public string[] VariationLong { get; }
        public override string ToString()
        {
            return Variation != null
                ? string.Join(" ", Variation.Select(x => x.SAN))
                : string.Join(" ", VariationLong);
        }
    }

    public class BestMoveResponse : IBestMoveResponse
    {
        public CalculationResponseTypes ResponseType => CalculationResponseTypes.BestMove;
        public BestMoveResponse(string bestMove, string ponderMove)
        {
            BestMoveLong = bestMove;
            PonderMoveLong = ponderMove;

        }

        public string BestMoveLong { get; }

        public string PonderMoveLong { get; }

        public MoveExt BestMove { get; set; }

        public MoveExt PonderMove { get; set; }

        public override string ToString()
        {
            var str = "{0} {1}";
            var bmSan = BestMove != null ? BestMove.SAN : BestMoveLong;
            var pvSan = PonderMove != null ? PonderMove.SAN : PonderMoveLong;
            return string.Format(str, bmSan, pvSan);
        }
    }
}
