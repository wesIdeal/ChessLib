namespace ChessLib.Core.Translate
{
    public abstract class ChessDto<TFrom, TTo>
    {
        public abstract TTo Translate(TFrom from);
    }

    public abstract class MoveDto<TTo>
    {
        public abstract TTo Translate(Move move, Board preMoveBoard, Board postMoveBoard);
    }
}