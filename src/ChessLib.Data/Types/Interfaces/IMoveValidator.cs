using ChessLib.Data.Exceptions;

namespace ChessLib.Types.Interfaces
{
    public interface IMoveValidator
    {
        MoveExceptionType? Validate();
    }

    public abstract class MoveValidatorBase : IMoveValidator
    {
        protected IBoard _boardInfo;
        protected MoveExt _move;
        public MoveValidatorBase(in IBoard board, in MoveExt move)
        {
            _boardInfo = board;
            _move = move;
        }

        public abstract MoveExceptionType? Validate();
    }

}