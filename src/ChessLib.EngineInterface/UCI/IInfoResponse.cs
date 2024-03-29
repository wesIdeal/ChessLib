﻿using System;
using System.Linq;
using ChessLib.Core;
using ChessLib.Core.Types;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;

namespace ChessLib.EngineInterface.UCI
{
    public interface ICalculationInfoResponse : IResponseObject
    {
        CalculationResponseTypes ResponseType { get; }
    }
    public interface IInfoResponse : ICalculationInfoResponse
    {
    }

    public interface IBestMoveResponse : ICalculationInfoResponse
    {
        string BestMoveLong { get; }
        Move BestMove { get; set; }
        string PonderMoveLong { get; }
        Move PonderMove { get; set; }
        string ToString();
    }

    public interface IInfoCalculationResponse : IInfoResponse
    {
        string CurrentMoveLong { get; }
        Move CurrentMove { get; set; }
    }

    public enum Bound { None, Lower, Upper }

    public interface IInfoScore
    {
        short? CentipawnScore { get; set; }
        short? MateInXMoves { get; set; }
        Bound Bound { get; set; }
        string ToString();
    }

    public interface IPrincipalVariationResponse : IInfoResponse
    {
        ulong Nodes { get; }
        string[] VariationLong { get; }
        Move[] Variation { get; set; }
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
        public Move CurrentMove { get; set; }
        public string CurrentMoveLong { get; set; }
        public uint CurrentMoveNumber { get; set; }
        public uint Depth { get; }
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
        public Move[] Variation { get; set; }
        public string[] VariationLong { get; }
        public override string ToString()
        {
            return Variation != null
                ? string.Join(" ", Variation.Select(x => x.SAN))
                : string.Join(" ", VariationLong);
        }

        public string ScoreText => (Score as PrincipalVariationScore)?.ToString();
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

        public Move BestMove { get; set; }

        public Move PonderMove { get; set; }

        public override string ToString()
        {
            var str = "{0} {1}";
            var bmSan = BestMove != null ? BestMove.SAN : BestMoveLong;
            var pvSan = PonderMove != null ? PonderMove.SAN : PonderMoveLong;
            return string.Format(str, bmSan, pvSan);
        }
    }
}
