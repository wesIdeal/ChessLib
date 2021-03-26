namespace ChessLib.Core.Types.Interfaces
{
    public interface IHasNullMoveValue<T>
    {
        T NullNodeConst { get; }
        bool IsNullNode { get; }
    }
}