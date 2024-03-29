﻿using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.MagicBitboard
{
    public interface IBitboard
    {
        ulong GetPseudoLegalMoves(ushort squareIndex, Piece pieceMoving, Color color, ulong occupancy);

        bool IsSquareAttackedByColor(ushort squareIndex, Color attackingColor, ulong[][] piecesOnBoard,
            ushort? enPassantSquare);
    }
}