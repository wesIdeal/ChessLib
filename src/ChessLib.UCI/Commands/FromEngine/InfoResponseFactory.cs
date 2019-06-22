using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChessLib.UCI.Commands.FromEngine
{
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

            public string EngineResponse { get; set; }

            public MoveExt CurrentMove { get; set; }

            public uint CurrentMoveNumber { get; set; }

            public MoveExt[] Variation { get; set; }

            public string SAN { get; set; }

            public PrincipalVariationResponse ToPVResponse()
            {
                return new PrincipalVariationResponse(PVOrdinal, Variation, Score, SelectiveSearchDepth, SearchTime,
                    Nodes, NodesPerSecond, TableBaseHits, Depth, FromFEN, SAN, EngineResponse);
            }

            public InfoCalculationResponse ToInfoCalculationResponse()
            {
                return new InfoCalculationResponse(Depth, CurrentMoveNumber, CurrentMove, FromFEN, SAN, EngineResponse);
            }
        }

        public enum InfoTypes
        {
            /// <summary>
            /// Used to signify the info line coming back contains an analysis of a variation
            /// </summary>
            AnalysisInfo,
            /// <summary>
            /// Used to signify the info line coming back tells which move is being calculated
            /// </summary>
            CalculationInfo
        };

        private static readonly KeyValuePair<string, int>[] _initializers =
            new KeyValuePair<string, int>[] {
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
                new KeyValuePair<string, int>("currmovenumber",1),

            };

        public static readonly Dictionary<string, int> InfoFieldDepth = _initializers.ToDictionary(k => k.Key, v => v.Value);


        static readonly string[] _calcKeywords = new string[] { "currmove", "currmovenumber" };

        public static InfoTypes GetTypeOfInfo(string engineResponse)
        {
            foreach (var kw in _calcKeywords)
            {
                if (engineResponse.Contains(kw)) return InfoTypes.CalculationInfo;
            }
            return InfoTypes.AnalysisInfo;
        }

        public EngineResponseArgs GetInfoResponse(in string fen, in string engineResponse)
        {
            if (engineResponse.StartsWith("bestmove"))
            {
                return GetBestMoveResponse(fen, engineResponse);
            }
            var response = engineResponse.Replace("info", "").Trim();
            var infoDictionary = new Dictionary<string, string>();
            var infoFields = response.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < infoFields.Count(); i++)
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
                    var lastIndex = infoFields.Count();
                    infoDictionary.Add(key, string.Join(" ", infoFields.GetRange(begin, lastIndex - begin)));
                    break;
                }
                else
                {
                    infoDictionary.Add(key, valueArray.FirstOrDefault());
                }
                i += keyLength;

            }
            var infoType = response.Contains("currmove") ? InfoTypes.CalculationInfo : InfoTypes.AnalysisInfo;
            return SetPropertiesFromInfoDict(fen, infoType, infoDictionary, engineResponse);
        }

        private BestMoveResponse GetBestMoveResponse(string fen, string engineResponse)
        {
            var keys = new[] { "bestmove", "ponder" };
            var values = engineResponse.Split(' ').Where(x => !keys.Contains(x)).ToArray();
            var bestMoveUnvalidated = FillUnvalidatedMoves(values);
            var san = new List<string>();
            var bestMoves = GetMoveInfo(fen, bestMoveUnvalidated.ToArray(), out san);
            var bestMove = bestMoves[0];
            var ponderMove = bestMoves.Count() > 1 ? bestMoves[1] : null;
            var ponderSan = san.Count() > 1 ? san[1] : "";
            return new BestMoveResponse(bestMove, ponderMove, san[0], ponderSan, engineResponse);
        }

        private EngineResponseArgs SetPropertiesFromInfoDict(in string fen, in InfoTypes infoType, in Dictionary<string, string> infoDictionary, in string engineResponse)
        {
            var ir = new InfoResponse();
            MoveExt currentMoveUn = null;
            MoveExt[] infoMovesUnvalidated = null;
            foreach (var field in infoDictionary)
            {
                switch (field.Key)
                {
                    case "currmove":
                        currentMoveUn = MoveTranslatorService.FromLANMove(field.Value);
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
                        infoMovesUnvalidated = FillUnvalidatedMoves(field.Value.Split(' ')).ToArray();
                        break;
                    case "bound":
                        ir.Score.Bound = field.Value == "lowerbound" ? Bound.Lower : field.Value == "upperbound" ? Bound.Upper : Bound.None;
                        break;

                }
            }

            var san = new List<string>();
            MoveExt[] validatedMoves;
            if (infoType == InfoTypes.AnalysisInfo)
            {
                Debug.Assert(infoMovesUnvalidated != null);
                validatedMoves = GetMoveInfo(fen, infoMovesUnvalidated, out san).ToArray();
                ir.Variation = validatedMoves;
                return ir.ToPVResponse();
            }
            else
            {
                Debug.Assert(currentMoveUn != null);
                var moves = GetMoveInfo(fen, new MoveExt[] { currentMoveUn }, out san).ToArray();
                if (moves.Any())
                {
                    ir.CurrentMove = moves[0];
                }
                else
                {
                    ir.CurrentMove = null;
                }
                return ir.ToInfoCalculationResponse();
            }
        }

        private IEnumerable<MoveExt> FillUnvalidatedMoves(string[] lanMoves)
        {
            foreach (var move in lanMoves)
            {
                yield return MoveTranslatorService.FromLANMove(move);
            }
        }

        private List<MoveExt> GetMoveInfo(string fen, MoveExt[] mvs, out List<string> san)
        {
            var board = new BoardInfo(fen);
            var sanMoveArray = new List<string>();
            var moveReturnValue = new List<MoveExt>();
            var count = 0;
            foreach (var move in mvs)
            {
                var isPromotion = move.MoveType == MoveType.Promotion;
                if (!isPromotion)
                {
                    var isEnPassant = Bitboard.IsEnPassantCapture(board, move);
                    var isCastlingMove = Bitboard.IsCastlingMove(board, move);
                    move.MoveType = isEnPassant ? MoveType.EnPassant : isCastlingMove ? MoveType.Castle : MoveType.Normal;
                }

                string moveNumber = "";
                if (count == 0 && board.ActivePlayer == Color.Black)
                {
                    moveNumber += board.FullmoveCounter + "...";

                }
                else if (board.ActivePlayer == Color.White)
                {
                    moveNumber = board.FullmoveCounter + ". ";
                }
                board.ApplyValidatedMove(move);
                sanMoveArray.Add($"{moveNumber}{board.MoveTree.Last().SAN}");
                count++;
                moveReturnValue.Add(move);
            }
            san = sanMoveArray;
            return moveReturnValue;
        }

    }
}
