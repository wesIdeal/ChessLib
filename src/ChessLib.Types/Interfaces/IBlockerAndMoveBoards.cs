namespace ChessLib.Types.Interfaces
{
    public interface IBlockerAndMoveBoards
    {
        ulong MoveBoard { get; }
        ulong Occupancy { get; }
        string ToString();
    }
}