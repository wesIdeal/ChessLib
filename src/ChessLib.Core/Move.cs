using System;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core
{
    public class Move : IMove, ICloneable, IEquatable<Move>
    {
        private ushort _move;
        private readonly bool _isNullMove = false;

        public Move()
        {
            _move = 0;
            _isNullMove = true;
        }

        public Move(ushort move)
        {
            this._move = move;
        }
        public string SAN { get; set; }
        
        public ushort SourceIndex => (ushort)((_move >> 6) & 63);

        public ulong SourceValue => 1ul << SourceIndex;

        public ushort DestinationIndex => (ushort)(_move & 63);

        public ulong DestinationValue => 1ul << DestinationIndex;

        public MoveType MoveType
        {
            get => (MoveType)((_move >> 14) & 3);
            
        }

        public PromotionPiece PromotionPiece => (PromotionPiece)((_move >> 12) & 3);
        
        public bool Equals(ushort other)
        {
            return _move == other;
        }

        public override string ToString()
        {
            if(_isNullMove)
            {
                return "NULL_MOVE";
            }
            return !string.IsNullOrEmpty(SAN) ? SAN : $"{SourceIndex.IndexToSquareDisplay()}->{DestinationIndex.IndexToSquareDisplay()}";
        }

        public bool Equals(IMove other)
        {
            if (other == null)
            {
                return false;
            }

            return _move == other.MoveValue;
        }

        public object Clone()
        {
            return new Move(_move);
        }

        public bool Equals(Move other)
        {
            if (other == null)
            {
                return false;
            }
            return _move == other._move;
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
            return _move.GetHashCode();
        }

        public ushort MoveValue => _move;
        public bool IsNullMove { get; }

        }
}