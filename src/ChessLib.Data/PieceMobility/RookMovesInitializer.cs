﻿using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.PieceMobility
{
    internal class RookMovesInitializer : MoveInitializer
    {
        public RookMovesInitializer() : base(SlidingPieceDirectionConstants.RookDirections) { }
    }
}
