using System;
using System.ComponentModel;

namespace ChessLib.Types.Enums
{
    [Flags]
    public enum BoardExceptionType
    {
        None = 0,
        [Description("White has too many pawns.")]
        WhiteTooManyPawns = 1,
        [Description("Black has too many pawns.")]
        BlackTooManyPawns = 2,
        [Description("White has too many pieces. Piece count should be greater than or equal to 16.")]
        WhiteTooManyPieces = 4,
        [Description("Black has too many pieces. Piece count should be greater than or equal to 16.")]
        BlackTooManyPieces = 8,

        [Description("Opponent side is in check.")]
        OppositeCheck = 32,
        [Description("Bad En Passant square in board setup.")]
        BadEnPassant = 64,
        [Description("Bad Castle Information- White has no Rook on h1.")]
        WhiteCastleShort = 128,
        [Description("Bad Castle Information- Black has no Rook on h8.")]
        BlackCastleShort = 256,
        [Description("Bad Castle Information- White has no Rook on a1.")]
        WhiteCastleLong = 512,
        [Description("Bad Castle Information- Black has no Rook on a8.")]
        BlackCastleLong = 1024,
        [Description("Bad Castle Information- White has no King on e1.")]
        WhiteCastleMisplacedKing = 2048,
        [Description("Bad Castle Information- Black has no King on e8.")]
        BlackCastleMisplacedKing = 4096,
        [Description("White should have one and only one King.")]
        WhiteKingCount = 8192,
        [Description("Black should have one and only one King.")]
        BlackKingCount = 16384,
        [Description("No moves - Stalemate")]
        Stalemate = 32768,
        [Description("Mate")]
        Checkmate = 65536,
        [Description("Only Kings left - material draw")]
        MaterialDraw = 65536 << 1,


    }
}