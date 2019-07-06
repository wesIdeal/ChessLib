using System;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.MoveRepresentation
{
    public class MoveExt : IMoveExt, ICloneable, IMove
    {
        public MoveExt(ushort move) { Move = move; }
        public string SAN { get; set; }
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

        public PromotionPiece PromotionPiece => (PromotionPiece)((_move >> 12) & 3);

        public bool Equals(ushort other)
        {
            return this._move == other;
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(SAN) ? SAN : $"{SourceIndex.IndexToSquareDisplay()}->{DestinationIndex.IndexToSquareDisplay()}";
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