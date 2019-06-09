using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Data.Helpers;
using System.Diagnostics;
using ChessLib.Types.Enums;

namespace ChessLib.UCI
{
    public class UCIInfoString
    {
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
                new KeyValuePair<string,int>("pv", 1)//special case - string of moves
            };
        public static readonly Dictionary<string, int> InfoFieldDepth = _initializers.ToDictionary(k => k.Key, v => v.Value);

        protected Dictionary<string, string> InfoDictionary { get; set; }
        public MoveExt[] InfoMoves { get; private set; }
        public uint Depth { get; private set; }
        public uint SelectedSearchDepth { get; private set; }
        public ushort Variation { get; private set; }
        public short? CentipawnScore { get; private set; }
        public short? MateInXMoves { get; private set; }
        public string Fen { get; }
        public long Nodes { get; private set; }
        public long NodesPerSecond { get; private set; }
        public long HashFull { get; private set; }
        public long TableBaseMatches { get; private set; }
        public TimeSpan SearchTime { get; private set; }


        public string ScoreDisplay
        {
            get
            {

                var integerValue = CentipawnScore.HasValue ? CentipawnScore.Value / 100 : 0;
                var floatLength = integerValue.ToString().Length + 3;
                return (MateInXMoves.HasValue ? $"M{MateInXMoves.Value}" :
                        CentipawnScore.HasValue ? $"{(float)CentipawnScore.Value / 100}".PadRight(floatLength, '0') : "");
            }
        }

        private string _variationDisplay = null;
        public string VariationDisplay => _variationDisplay;



        public UCIInfoString(string fenPosition, string infoStr)
        {
            Fen = fenPosition;
            InfoDictionary = new Dictionary<string, string>();
            var trimmed = infoStr.Replace("info", "");
            var infoFields = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < infoFields.Count(); i++)
            {
                var data = infoFields[i];
                Debug.Assert(InfoFieldDepth.ContainsKey(data));
                var keyLength = InfoFieldDepth[data];
                var key = string.Join(" ", infoFields.Skip(i).Take(keyLength));
                var valueArray = infoFields.Skip(i + keyLength).Take(1);
                var value = key == "pv" ? string.Join(" ", valueArray) : valueArray.FirstOrDefault();
                InfoDictionary.Add(key, value);

                if (key == "pv")
                {
                    var moveArray = infoFields.Skip(i + 1).ToArray();
                    InfoMoves = FillMoves(fenPosition, moveArray).ToArray();
                    break;
                }
                i += keyLength;

            }
            SetPropertiesFromInfoDict();
        }

        private void SetPropertiesFromInfoDict()
        {
            foreach (var field in InfoDictionary)
            {
                switch (field.Key)
                {
                    case "depth":
                        Depth = uint.Parse(field.Value);
                        break;
                    case "seldepth":
                        SelectedSearchDepth = uint.Parse(field.Value);
                        break;
                    case "multipv":
                        Variation = ushort.Parse(field.Value);
                        break;
                    case "score cp":
                        CentipawnScore = short.Parse(field.Value);
                        break;
                    case "score mate":
                        MateInXMoves = short.Parse(field.Value);
                        break;
                    case "nodes":
                        Nodes = long.Parse(field.Value);
                        break;
                    case "nps":
                        NodesPerSecond = long.Parse(field.Value);
                        break;
                    case "hashfull":
                        HashFull = long.Parse(field.Value);
                        break;
                    case "tbhits":
                        TableBaseMatches = long.Parse(field.Value);
                        break;
                    case "time":
                        SearchTime = TimeSpan.FromMilliseconds(long.Parse(field.Value));
                        break;
                    case "pv":
                        FillMoves(Fen, field.Value.Split(' '));
                        break;

                }


            }
        }

        private IEnumerable<MoveExt> FillMoves(string fen, string[] moveArray)
        {
            var board = new BoardInfo(fen);
            var sanMoves = new List<string>();
            var count = 0;
            foreach (var move in moveArray)
            {
                ushort source, dest;
                PromotionPiece promotionPiece = PromotionPiece.Knight;
                GetSourceAndDestination(move, out source, out dest);
                var pieceOfColor = board.GetPieceOfColorAtIndex(source);
                Debug.Assert(pieceOfColor.HasValue);
                var piece = pieceOfColor.Value.Piece;
                var isPromotion = IsPromotion(move, piece, source, dest, out promotionPiece);
                var isEnPassant = Bitboard.IsEnPassantCapture(piece, source, dest, board.EnPassantSquare);
                var isCastlingMove = Bitboard.IsCastlingMove(piece, source, dest);
                MoveType moveType = GetMoveTypeFromInformation(isPromotion, isEnPassant, isCastlingMove);
                var generatedMove = MoveHelpers.GenerateMove(source, dest, moveType, promotionPiece);
                string moveNumber = "";
                if (count == 0 && board.ActivePlayer == Color.Black)
                {
                    moveNumber += "... " + board.FullmoveCounter;

                }
                else if (board.ActivePlayer == Color.White)
                {
                    moveNumber = board.FullmoveCounter + ". ";
                }
                board.ApplyValidatedMove(generatedMove);

                sanMoves.Add($"{moveNumber}{board.MoveTree.Last().Move.SAN}");
                count++;
                yield return generatedMove;
            }
            _variationDisplay = string.Join(" ", sanMoves);
        }

        private static MoveType GetMoveTypeFromInformation(bool isPromotion, bool isEnPassant, bool isCastlingMove)
        {
            return isPromotion ? ChessLib.Types.Enums.MoveType.Promotion :
                isCastlingMove ? ChessLib.Types.Enums.MoveType.Castle :
                isEnPassant ? ChessLib.Types.Enums.MoveType.EnPassant : Types.Enums.MoveType.Normal;
        }

        private bool IsPromotion(string move, Piece piece, ushort source, ushort dest, out PromotionPiece promotionPiece)
        {
            var rv = move.Length > 4 && Bitboard.IsPromotion(piece, source, dest);
            var promotionPieceChar = rv ? move[4] : 'n';
            promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionPieceChar);
            return rv;
        }

        private void GetSourceAndDestination(string move, out ushort source, out ushort dest)
        {
            var src = move.Substring(0, 2).SquareTextToIndex();
            var dst = move.Substring(2, 2).SquareTextToIndex();

            Debug.Assert(src.HasValue && dst.HasValue);
            source = src.Value;
            dest = dst.Value;
        }
    }
}
