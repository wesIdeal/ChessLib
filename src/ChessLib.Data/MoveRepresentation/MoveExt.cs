using ChessLib.Data.Helpers;
using System;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.MoveRepresentation
{
    public class MoveExt : MoveBase, IMoveExt, ICloneable, IEquatable<MoveExt>
    {
        public MoveExt() : base() { }

        public MoveExt(ushort move) : base(move) { }

        public MoveExt(MoveExt move) : base(move.Move)
        {
            this.SAN = move.SAN;
        }

        public string SAN { get; set; }
        
        public ushort SourceIndex => (ushort)((_move >> 6) & 63);

        public ulong SourceValue => 1ul << SourceIndex;

        public ushort DestinationIndex => (ushort)(_move & 63);

        public ulong DestinationValue => 1ul << DestinationIndex;

        public MoveType MoveType
        {
            get => (MoveType)((_move >> 14) & 3);
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
            return _move == other;
        }

        public override string ToString()
        {
            if(IsNullMove)
            {
                return "NULL_MOVE";
            }
            return !string.IsNullOrEmpty(SAN) ? SAN : $"{SourceIndex.IndexToSquareDisplay()}->{DestinationIndex.IndexToSquareDisplay()}";
        }

        public bool Equals(IMoveExt other)
        {
            if (other == null)
            {
                return false;
            }

            return _move == other.Move;
        }

        public object Clone()
        {
            return new MoveExt(Move);
        }

        public bool Equals(MoveExt other)
        {
            if (other == null)
            {
                return false;
            }
            return Move == other.Move;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}