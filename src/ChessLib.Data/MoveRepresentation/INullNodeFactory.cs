namespace ChessLib.Data.MoveRepresentation
{
    public interface IHasNullMoveValue<T>
    {
        T NullNodeConst { get; }
        bool IsNullNode { get; }
    }
}