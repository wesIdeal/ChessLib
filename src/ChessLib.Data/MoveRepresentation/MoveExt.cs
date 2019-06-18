using System;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.MoveRepresentation
{


    public class MoveExt : IMoveExt, ICloneable, IMove
    {
        public MoveExt(ushort move) { Move = move; }

        /// <summary>
        /// Create move from long alg. notation
        /// 
        /// </summary>
        /// <example>e2e4 from inital position is 1. e4. e7e8q would be e8=Q</example>
        /// <param name="lanMove"></param>
        /// <remarks>
        /// Move will need to be validated against a board, as it can only know if the move is normal or promotion.
        /// It will not be aware of En Passant captures or Castling.
        /// This constructor's main use is to interpret moves from an engine and not for normal use.
        /// </remarks>
        /// <returns>A basic move that needs to be validated against a board of pieces. Null if the move source and/or destination are invalid.</returns>
        public static MoveExt FromLANMove(string lanMove)
        {
            var moveType = MoveType.Normal;
            var promotionPiece = PromotionPiece.Knight;
            var source = lanMove.Substring(0, 2).SquareTextToIndex();
            var dest = lanMove.Substring(2, 2).SquareTextToIndex();
            if (source == null || dest == null) { return null; }
            if (lanMove.Length == 5)
            {
                moveType = MoveType.Promotion;
                var promotionPieceChar = lanMove[4];
                promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionPieceChar);
            }
            return MoveHelpers.GenerateMove(source.Value, dest.Value, moveType, promotionPiece);
        }

        private ushort _move;
        public ushort Move { get => _move; protected set { _move = value; } }

        public ushort SourceIndex => (ushort)((_move >> 6) & 63);

        public ulong SourceValue => 1ul << SourceIndex;

        public ushort DestinationIndex => (ushort)(_move & 63);

        public ulong DestinationValue => 1ul << DestinationIndex;

        public MoveType MoveType
        {
            get { return (MoveType)((_move >> 14) & 3); }
            set
            {
                ushort mt = (ushort)((ushort)value << 14);
                Move &= 0x1fff;
                Move |= mt;
            }
        }

        public string LAN => $"{SourceIndexText}{DestIndexText}{PromotionPieceChar}";

        public PromotionPiece PromotionPiece => (PromotionPiece)((_move >> 12) & 3);

        public char? PromotionPieceChar => MoveType == MoveType.Promotion ?
            char.ToLower(PieceHelpers.GetCharFromPromotionPiece(PromotionPiece)) :
            (char?)null;

        public string SourceIndexText => SourceIndex.IndexToSquareDisplay();

        public string DestIndexText => DestinationIndex.IndexToSquareDisplay();

        public bool Equals(ushort other)
        {
            return this._move == other;
        }

        public override string ToString()
        {
            var srcFile = (char)('a' + (SourceIndex % 8));
            var srcRank = (char)('1' + (SourceIndex / 8));
            var dstFile = (char)('a' + (DestinationIndex % 8));
            var dstRank = (char)('1' + (DestinationIndex / 8));
            var from = $"{srcFile}{srcRank}";
            var to = $"{dstFile}{dstRank}";
            return $"{from}->{to}";
        }

        public bool Equals(IMoveExt other)
        {
            return this._move == other.Move;
        }

        public object Clone()
        {
            return new MoveExt(Move);
        }
    }
}