namespace ChessLib.Core.Types.Enums
{
    /// <summary>
    ///     Represents the type of move in 3 bits.
    /// </summary>
    public enum MoveType
    {
        Normal = 0,
        Promotion = 1,
        EnPassant = 2,
        Castle = 3
    }
}