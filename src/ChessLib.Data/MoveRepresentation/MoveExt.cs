﻿using System;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.MoveRepresentation
{
    public class MoveExt : IMoveExt, ICloneable, IMove
    {
        public MoveExt(ushort move) { Move = move; }



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