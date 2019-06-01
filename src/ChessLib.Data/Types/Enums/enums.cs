using System;
using System.ComponentModel;

namespace ChessLib.Types.Enums
{
    public enum Rank { R1 = 0, R2, R3, R4, R5, R6, R7, R8 };
    public enum File { A = 0, B, C, D, E, F, G, H };
    public enum Color { White = 0, Black = 1 }

    /// <summary>
    /// A way to refer to the objects that sit on the board. Pawn is included here for that very reason.
    /// </summary>
    public enum Piece
    {
        Pawn, Knight, Bishop, Rook, Queen, King
    }
    /// <summary>
    /// Possible products of a pawn's promotion, small enough to be represented in 3 bits.
    /// </summary>
    public enum PromotionPiece
    {
        Knight, Bishop, Rook, Queen
    }

    /// <summary>
    /// Represents the type of move in 3 bits.
    /// </summary>
    public enum MoveType
    {
        Normal = 0, Promotion = 1, EnPassant = 2, Castle = 3
    }

    /// <summary>
    /// Pieces/sections of a FEN. />
    /// </summary>
    public enum FENPieces { PiecePlacement = 0, ActiveColor, CastlingAvailability, EnPassantSquare, HalfmoveClock, FullMoveCounter }

    // ReSharper disable InconsistentNaming
    [Flags]
    public enum MoveDirection
    {
        N = 1,

        NE = 2,

        E = 4,
        SE = 8,
        S = 16,
        SW = 32,
        W = 64,
        NW = 128
    }
    // ReSharper restore InconsistentNaming

    /// <summary>
    /// Flags to represent a check and the nature of the check.
    /// </summary>
    [Flags]
    public enum Check { Null = 0, None = 1, Normal = 2, Opposite = 4, Double = 8 }

    [Flags]
    public enum CastlingAvailability
    {
        [Description("-")]
        NoCastlingAvailable = 0,
        [Description("K")]
        WhiteKingside = 1,
        [Description("Q")]
        WhiteQueenside = 2,
        [Description("k")]
        BlackKingside = 4,
        [Description("q")]
        BlackQueenside = 8


    }
    public static class SlidingPieceDirectionConstants
    {
        public const MoveDirection RookDirections = MoveDirection.N | MoveDirection.E | MoveDirection.S | MoveDirection.W;
        public const MoveDirection BishopDirections = MoveDirection.NE | MoveDirection.SE | MoveDirection.SW | MoveDirection.NW;
    }

    [Flags]
    public enum BoardException
    {
        None = 0,
        [Description("White has too many pawns.")]
        WhiteTooManyPawns = 2,
        [Description("Black has too many pawns.")]
        BlackTooManyPawns,
        [Description("White has too many pieces. Piece count should be greater than or equal to 16.")]
        WhiteTooManyPieces = 4,
        [Description("Black has too many pieces. Piece count should be greater than or equal to 16.")]
        BlackTooManyPieces = 8,
        [Description("Both sides are in check.")]
        DoubleCheck = 16,
        [Description("Active side (side to move) is in check.")]
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
        Stalemate = 32768,
        Checkmate = 65536
    }

    [Flags]
    public enum FENError
    {
        [Description("")]
        Null = 0,
        [Description("Invalid FEN. FEN string must have 6 pieces. See See https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation for more information")]
        InvalidFENString = 1,
        [Description("Piece Placement section should have exactly 8 ranks.")]
        PiecePlacementRankCount = 2,
        [Description("Each Rank in Piece Placement section should contain exactly 8 squares of information.")]
        PiecePlacementPieceCountInRank = 4,
        [Description("Invalid characters in Piece Placement section found. Lower and uppercase variants of the following letters, 'p','r','b','n','k','q' and the digits 1-8 are allowed.")]
        PiecePlacementInvalidChars = 8,
        [Description("Active Color (side to move) must either be 'w' or 'b'.")]
        InvalidActiveColor = 16,
        [Description("Invalid Castling information - unrecognized character found. Only the following are accepted: 'k', 'K', 'q', 'Q', '-' (no castling availability)")]
        CastlingUnrecognizedChar = 32,
        [Description("Invalid Castling Information - repeated characters found.")]
        CastlingStringRepetition = 64,
        [Description("Invalid Castling Information - cannot use '-' with other castling character.")]
        CastlingStringBadSequence = 128,
        [Description("Invalid Castling Information - no castling availability found.")]
        CastlingNoStringPresent = 256,
        [Description("Invalid Halfmove clock.")]
        HalfmoveClock = 512,
        [Description("Invalid Full Move Counter.")]
        FullMoveCounter = 1024,
        [Description("En Passant square is invalid. Must be 2 characters long and translate to a square on a chessboard. Ex. [file_letter][rank_number].")]
        InvalidEnPassantSquare = 2048,

    }
}
