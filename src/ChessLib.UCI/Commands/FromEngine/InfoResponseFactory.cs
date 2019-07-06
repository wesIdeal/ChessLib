using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChessLib.UCI.Commands.FromEngine
{
    public enum CalculationResponseTypes
    {
        /// <summary>
        /// Used to signify the info line coming back contains an analysis of a variation
        /// </summary>
        PrincipalVariation,
        /// <summary>
        /// Used to signify the info line coming back tells which move is being calculated
        /// </summary>
        CalculationInformation,
        /// <summary>
        /// Used to signify the information contains a best move and ponder move only
        /// </summary>
        BestMove
    }
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class InfoResponseFactory
    {
        internal class InfoResponse : IPrincipalVariationResponse, IInfoCalculationResponse
        {
            
            public InfoResponse()
            {
                Score = new PrincipalVariationScore();
            }

            public uint SelectiveSearchDepth { get; set; }

            public ushort PVOrdinal { get; set; }

            public IInfoScore Score { get; set; }

            public TimeSpan SearchTime { get; set; }

            public ulong Nodes { get; set; }

            public ulong NodesPerSecond { get; set; }

            public ushort TableBaseHits { get; set; }

            public uint Depth { get; set; }

            public string FromFEN { get; set; }

            public string ResponseText { get; set; }

            public string CurrentMoveLong { get; set; }

            public MoveExt CurrentMove { get; set; }

            public uint CurrentMoveNumber { get; set; }

            public string[] VariationLong { get; set; }

            public MoveExt[] Variation { get; set; }

            public string SAN { get; set; }

            public Guid Id { get; set; }

            public PrincipalVariationResponse ToPVResponse()
            {
                return new PrincipalVariationResponse(PVOrdinal, VariationLong, Score, SelectiveSearchDepth, SearchTime,
                    Nodes, NodesPerSecond, TableBaseHits, Depth);
            }

            public InfoCalculationResponse ToInfoCalculationResponse()
            {
                return new InfoCalculationResponse(Depth, CurrentMoveNumber, CurrentMoveLong);
            }

            public CalculationResponseTypes ResponseType { get; set; }
        }



        private static readonly KeyValuePair<string, int>[] InfoFieldNames =
            {
                new KeyValuePair<string,int>("depth", 1),
                new KeyValuePair<string,int>("seldepth", 1),
                new KeyValuePair<string,int>("multipv", 1),
                new KeyValuePair<string,int>("score", 2),
                new KeyValuePair<string,int>("nodes", 1),
                new KeyValuePair<string,int>("nps", 1),
                new KeyValuePair<string,int>("hashfull", 1),
                new KeyValuePair<string,int>("tbhits", 1),
                new KeyValuePair<string,int>("time", 1),
                new KeyValuePair<string,int>("pv", 1),//special case - string of moves after this until newline
                new KeyValuePair<string,int>("currmove",1),
                new KeyValuePair<string, int>("currmovenumber",1)

            };

        public static readonly Dictionary<string, int> InfoFieldDepth = InfoFieldNames.ToDictionary(k => k.Key, v => v.Value);


        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private static readonly string[] CalcKeywords = { "currmove", "currmovenumber" };

        public static CalculationResponseTypes GetTypeOfInfo(string engineResponse)
        {
            foreach (var kw in CalcKeywords)
            {
                if (engineResponse.Contains(kw)) return CalculationResponseTypes.CalculationInformation;
            }
            return CalculationResponseTypes.PrincipalVariation;
        }

        public ICalculationInfoResponse GetInfoResponse(in string engineResponse)
        {
            if (engineResponse.StartsWith("bestmove"))
            {
                return GetBestMoveResponse(engineResponse);
            }
            var response = engineResponse.Replace("info", "").Trim();
            var infoDictionary = new Dictionary<string, string>();
            var infoFields = response.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < infoFields.Count; i++)
            {
                var data = infoFields[i];
                if (data == "lowerbound" || data == "upperbound")
                {
                    infoDictionary.Add("bound", data);
                    continue;
                }
                Debug.Assert(InfoFieldDepth.ContainsKey(data));
                var keyLength = InfoFieldDepth[data];
                var key = string.Join(" ", infoFields.Skip(i).Take(keyLength));
                var valueArray = infoFields.Skip(i + keyLength);
                if (key == "pv")
                {
                    var begin = i + 1;
                    var lastIndex = infoFields.Count;
                    infoDictionary.Add(key, string.Join(" ", infoFields.GetRange(begin, lastIndex - begin)));
                    break;
                }

                infoDictionary.Add(key, valueArray.FirstOrDefault());
                i += keyLength;

            }
            var infoType = response.Contains("currmove") ? CalculationResponseTypes.CalculationInformation : CalculationResponseTypes.PrincipalVariation;
            return SetPropertiesFromInfoDict(infoType, infoDictionary);
        }

        private ICalculationInfoResponse GetBestMoveResponse(string engineResponse)
        {
            var values = engineResponse.Split(' ').ToArray();
            var bestMove = GetKeyValue(values, "bestmove");
            var ponderMove = GetKeyValue(values, "ponder");
            return new BestMoveResponse(bestMove, ponderMove);
        }

        private ICalculationInfoResponse SetPropertiesFromInfoDict(in CalculationResponseTypes calculationResponseType, in Dictionary<string, string> infoDictionary)
        {
            var ir = new InfoResponse();
            MoveExt currentMoveUn = null;
            MoveExt[] infoMovesUnvalidated = null;
            foreach (var field in infoDictionary)
            {
                switch (field.Key)
                {
                    case "currmove":
                        ir.CurrentMoveLong = field.Value;
                        break;
                    case "currmovenumber":
                        ir.CurrentMoveNumber = uint.Parse(field.Value);
                        break;
                    case "depth":
                        ir.Depth = uint.Parse(field.Value);
                        break;
                    case "seldepth":
                        ir.SelectiveSearchDepth = uint.Parse(field.Value);
                        break;
                    case "multipv":
                        ir.PVOrdinal = ushort.Parse(field.Value);
                        break;
                    case "score cp":
                        ir.Score.CentipawnScore = short.Parse(field.Value);
                        ir.Score.MateInXMoves = null;
                        break;
                    case "score mate":
                        ir.Score.MateInXMoves = short.Parse(field.Value);
                        ir.Score.CentipawnScore = null;
                        break;
                    case "nodes":
                        ir.Nodes = (ulong)long.Parse(field.Value);
                        break;
                    case "nps":
                        ir.NodesPerSecond = (ulong)long.Parse(field.Value);
                        break;
                    case "tbhits":
                        ir.TableBaseHits = ushort.Parse(field.Value);
                        break;
                    case "time":
                        ir.SearchTime = TimeSpan.FromMilliseconds(long.Parse(field.Value));
                        break;
                    case "pv":
                        ir.VariationLong = field.Value.Split(' ');
                        break;
                    case "bound":
                        ir.Score.Bound = field.Value == "lowerbound" ? Bound.Lower : field.Value == "upperbound" ? Bound.Upper : Bound.None;
                        break;

                }
            }

            if (calculationResponseType == CalculationResponseTypes.PrincipalVariation)
            {
                return ir.ToPVResponse();
            }

            return ir.ToInfoCalculationResponse();
        }

        private string GetKeyValue(IEnumerable<string> keyValueArray, string key)
        {
            var arr = keyValueArray.ToArray();
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] == key)
                {
                    var valueIdx = i + 1;
                    if (valueIdx < arr.Length)
                    {
                        return arr[valueIdx];
                    }

                    return null;
                }
            }
            return null;
        }
    }
}
