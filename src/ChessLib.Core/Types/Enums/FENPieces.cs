namespace ChessLib.Core.Types.Enums
{
    /// <summary>
    ///     Pieces/sections of a Premove FEN. />
    /// </summary>
    public enum FENPieces
    {
        PiecePlacement = 0,
        ActiveColor,
        CastlingAvailability,
        EnPassantSquare,
        HalfmoveClock,
        FullMoveCounter
    }
}