using System;
using System.ComponentModel;

namespace ChessLib.Core.Types.Enums
{
    // ReSharper disable InconsistentNaming
    // ReSharper restore InconsistentNaming

    [Flags]
    public enum FENError
    {
        [Description("")] None = 0,

        [Description(
            "Invalid PremoveFEN. PremoveFEN string must have 6 pieces. See See https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation for more information")]
        InvalidFENString = 1,

        [Description("Piece Placement section should have exactly 8 ranks.")]
        PiecePlacementRankCount = 2,

        [Description("Each Rank in Piece Placement section should contain exactly 8 squares of information.")]
        PiecePlacementPieceCountInRank = 4,

        [Description(
            "Invalid characters in Piece Placement section found. Lower and uppercase variants of the following letters, 'p','r','b','n','k','q' and the digits 1-8 are allowed.")]
        PiecePlacementInvalidChars = 8,

        [Description("Active Color (side to move) must either be 'w' or 'b'.")]
        InvalidActiveColor = 16,

        [Description(
            "Invalid Castling information - unrecognized character found. Only the following are accepted: 'k', 'K', 'q', 'Q', '-' (no castling availability)")]
        CastlingUnrecognizedChar = 32,

        [Description("Invalid Castling Information - repeated characters found.")]
        CastlingStringRepetition = 64,

        [Description("Invalid Castling Information - no castling availability found.")]
        CastlingNoStringPresent = 256,

        [Description("Invalid Halfmove clock.")]
        HalfmoveClock = 512,

        [Description("Invalid Full MoveValue Counter.")]
        FullMoveCounter = 1024,

        [Description(
            "En Passant square is invalid. Must be 2 characters long and translate to a square on a chessboard. Ex. [file_letter][rank_number].")]
        InvalidEnPassantSquare = 2048
    }
}