using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Helpers;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules
{
    public class CastlingAvailabilityRule : IBoardRule
    {
      
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            var rv = BoardExceptionType.None;
            var castlingAvailability = boardInfo.CastlingAvailability;
            if (castlingAvailability == CastlingAvailability.NoCastlingAvailable) return BoardExceptionType.None;

            var whiteRooks = boardInfo.Occupancy.Occupancy(Color.White, Piece.Rook);
            var blackRooks = boardInfo.Occupancy.Occupancy(Color.Black, Piece.Rook);
            var whiteKing = boardInfo.Occupancy.Occupancy(Color.White, Piece.King);
            var blackKing = boardInfo.Occupancy.Occupancy(Color.Black, Piece.King);
            //Check for Rook placement
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) &&
                !whiteRooks.IsBitSet(0))
                rv |= BoardExceptionType.WhiteCastleLong;
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) &&
                !blackRooks.IsBitSet(56))
                rv |= BoardExceptionType.BlackCastleLong;
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside) &&
                !whiteRooks.IsBitSet(7))
                rv |= BoardExceptionType.WhiteCastleShort;
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackKingside) &&
                !blackRooks.IsBitSet(63))
                rv |= BoardExceptionType.BlackCastleShort;

            ////Check for King placement
            if ((castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) ||
                 castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside))
                && !whiteKing.IsBitSet(4))
                rv |= BoardExceptionType.WhiteCastleMisplacedKing;
            if ((castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) ||
                 castlingAvailability.HasFlag(CastlingAvailability.BlackKingside))
                && !blackKing.IsBitSet(60))
                rv |= BoardExceptionType.BlackCastleMisplacedKing;
            return rv;
        }
    }
}