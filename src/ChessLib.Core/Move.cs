using System;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core
{
    public class Move : IMove, ICloneable, IEquatable<Move>
    {
        internal const int NullMoveValue = 0;

        internal Move()
        {
            MoveValue = NullMoveValue;
        }
        public Move(ushort move)
        {
            MoveValue = move;
        }

        protected Move(Move node)
        {
            this.MoveValue = node.MoveValue;
            this.SAN = node.SAN;
        }

        public string SAN { get; set; }

        public object Clone()
        {
            return new Move(MoveValue);
        }

        public bool Equals(Move other)
        {
            if (other == null)
            {
                return false;
            }

            return MoveValue == other.MoveValue;
        }

        public ushort SourceIndex => (ushort) ((MoveValue >> 6) & 63);

        public ulong SourceValue => 1ul << SourceIndex;

        public ushort DestinationIndex => (ushort) (MoveValue & 63);

        public ulong DestinationValue => 1ul << DestinationIndex;

        public MoveType MoveType => (MoveType) ((MoveValue >> 14) & 3);

        public PromotionPiece PromotionPiece => (PromotionPiece) ((MoveValue >> 12) & 3);

        public bool Equals(ushort other)
        {
            return MoveValue == other;
        }

        public override string ToString()
        {
            if (IsNullMove)
            {
                return "NULL_MOVE";
            }

            return !string.IsNullOrEmpty(SAN)
                ? SAN
                : $"{SourceIndex.IndexToSquareDisplay()}->{DestinationIndex.IndexToSquareDisplay()}";
        }

        public bool Equals(IMove other)
        {
            if (other == null)
            {
                return false;
            }

            return MoveValue == other.MoveValue;
        }

        public ushort MoveValue { get; }

        public bool IsNullMove => MoveValue == NullMoveValue;
        public static Move NullMove => new Move(NullMoveValue);

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
            return MoveValue.GetHashCode();
        }
    }
}