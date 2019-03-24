using System;

namespace ChessLib.Data.MoveRepresentation
{
    public class MoveText : IEquatable<MoveText>
    {
        public MoveText(string move)
        {
            MoveSAN = move;
        }

        public int MoveNumber { get; set; }
        public string MoveSAN { get; set; }
        public string PremoveComment { get; set; }
        public string MoveComment { get; set; }

        public bool Equals(MoveText other)
        {
            return MoveNumber == MoveNumber
                && MoveSAN == MoveSAN
                && PremoveComment == PremoveComment
                && MoveComment == MoveComment;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is MoveText)
            {
                return this.Equals(obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return MoveNumber.GetHashCode() ^ MoveSAN.GetHashCode() ^ PremoveComment.GetHashCode() ^ MoveComment.GetHashCode();
        }

        public override string ToString()
        {
            return MoveSAN;
        }
    }
}
