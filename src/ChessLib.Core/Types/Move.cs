using System;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types
{
    public class Move : IMove, ICloneable, IEquatable<Move>
    {
        internal const int NullMoveValue = 0;

        public string SAN { get; set; }

        public bool IsNullMove => MoveValue == NullMoveValue;
        public static Move NullMove => NullMoveValue;

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
            MoveValue = node.MoveValue;
            SAN = node.SAN;
        }

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

        public ushort SourceIndex => (ushort)((MoveValue >> 6) & 63);

        public ulong SourceValue => 1ul << SourceIndex;

        public ushort DestinationIndex => (ushort)(MoveValue & 63);

        public ulong DestinationValue => 1ul << DestinationIndex;

        public MoveType MoveType => (MoveType)((MoveValue >> 14) & 3);

        public PromotionPiece PromotionPiece => (PromotionPiece)((MoveValue >> 12) & 3);

        public bool Equals(IMove other)
        {
            if (other == null)
            {
                return false;
            }

            return MoveValue == other.MoveValue;
        }

        public ushort MoveValue { get; }

        public bool Equals(ushort other)
        {
            return MoveValue == other;
        }

        public static implicit operator Move(ushort uMove)
        {
            return new Move(uMove);
        }

        public static implicit operator ushort(Move move)
        {
            return move.MoveValue;
        }


        public override string ToString()
        {
            if (IsNullMove)
            {
                return "NULL_MOVE";
            }

            return !string.IsNullOrEmpty(SAN)
                ? SAN
                : $"{SourceIndex.ToSquareString()}->{DestinationIndex.ToSquareString()}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return MoveValue == obj as Move;
        }

        public override int GetHashCode()
        {
            return MoveValue.GetHashCode();
        }
    }
}