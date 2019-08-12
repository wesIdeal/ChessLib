using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.MoveRepresentation
{
    public class MoveBase : IMove
    {
        protected ushort _move;
        private readonly bool _isNullMove = false;

        public MoveBase()
        {
            Move = 0;
            _isNullMove = true;
        }

        public MoveBase(ushort move)
        {
            Move = move;
        }

        public ushort Move
        {
            get => _move; protected set => _move = value;
        }

        public bool IsNullMove => _isNullMove;

        public override bool Equals(object obj)
        {
            var other = (MoveBase)obj;
            if(obj == null || other == null) { return false; }
            return _move == other._move;
        }

        public override int GetHashCode()
        {
            return _move.GetHashCode();
        }

        public override string ToString()
        {
            return Move.ToString();
        }
    }
}