using System;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types.Interfaces
{
    public interface IBoardState : IEquatable<IBoardState>
    {
        /// <summary>
        ///     FIELD               [bit index, from smallest]
        ///     Half MoveValue Clock     [Bits 0 - 7]
        ///     En Passant Index    [Bits 8 - 12] 0-based index for board ranks 3 and 6, A3-H3, A6-H6
        ///     Castling Rights     [Bits 13 - 15]
        ///     Captured Piece      [Bits 17 - 19] 1-5 (pawn - Queen, 0 for none)
        ///     GameState           [Bits 20 - 21]
        ///     Full MoveValue Count     [Bits 22 - 30]
        ///     Active Color        [Bit  31]
        /// </summary>
        uint BoardStateStorage { get; }

        GameState GameState { get; }
        CastlingAvailability CastlingAvailability { get; }
        Piece? PieceCaptured { get; }

        /// <summary>
        ///     Index of en passant square, if available. Null if no en passant capture exists.
        /// </summary>
        ushort? EnPassantSquare { get; }

        /// <summary>
        ///     Number of halfmoves since either a) a pawn advance or b) capture. Used to determine draws for 50-move rule.
        /// </summary>
        ushort HalfMoveClock { get; }


        /// <summary>
        ///     The number of the full move. Starts at 1. After each move from black, it is incremented.
        /// </summary>
        uint FullMoveCounter { get; }

        Color ActivePlayer { get; }
    }
}