using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Types
{


    public class MoveExt : IMoveExt
    {
        public MoveExt(ushort move) { Move = move; }
        private ushort _move;
        public ushort Move { get => _move; private set { _move = value; } }

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

        public PromotionPiece PromotionPiece => throw new System.NotImplementedException();

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
    }
}