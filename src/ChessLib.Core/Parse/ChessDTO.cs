namespace ChessLib.Core.Parse
{
    public abstract class ChessDTO<TFrom, TTo>
    {
        public abstract TTo Translate(TFrom from);
    }
}