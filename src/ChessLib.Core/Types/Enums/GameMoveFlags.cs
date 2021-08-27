using System;

namespace ChessLib.Core.Types.Enums
{
    /// <summary>
    ///     Flags the move details as they relate to a game.
    /// </summary>
    [Flags]
    public enum GameMoveFlags
    {
        NullMove = 0,
        
       
        InitialMove = 1 << 4,
        Variation = 1 << 5,
        BeginVariation = Variation | InitialMove,
        
    }
}