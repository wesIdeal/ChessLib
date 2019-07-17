using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation.Rules
{
    public class CastlingAvailabilityRule : IBoardRule
    {
        //public static string ValidateCastlingRights(ulong[][] piecesOnBoard, CastlingAvailability castlingAvailability,
        //    bool chess960 = false)
        //{

        //}
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            var rv = BoardExceptionType.None;
            var castlingAvailability = boardInfo.CastlingAvailability;
            if (castlingAvailability == CastlingAvailability.NoCastlingAvailable) return BoardExceptionType.None;

            var whiteRooks = boardInfo.GetPiecePlacement().Occupancy(Color.White, Piece.Rook);
            var blackRooks = boardInfo.GetPiecePlacement().Occupancy(Color.Black, Piece.Rook);
            var whiteKing = boardInfo.GetPiecePlacement().Occupancy(Color.White, Piece.King);
            var blackKing = boardInfo.GetPiecePlacement().Occupancy(Color.Black, Piece.King);
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
    public class EnPassantSquareRule : IBoardRule
    {
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            if (boardInfo.EnPassantSquare == null) return BoardExceptionType.None;
            if (boardInfo.ActivePlayer == Color.White &&
                (boardInfo.EnPassantSquare < 40 || boardInfo.EnPassantSquare > 47)
                ||
                boardInfo.ActivePlayer == Color.Black &&
                (boardInfo.EnPassantSquare < 16 || boardInfo.EnPassantSquare > 23))
                return BoardExceptionType.BadEnPassant;
            var possiblePawnLocation =
                boardInfo.ActivePlayer == Color.White ? boardInfo.EnPassantSquare - 8 : boardInfo.EnPassantSquare + 8;
            var possiblePawnLocationValue = 1ul << possiblePawnLocation;
            var pawnsOnBoard = boardInfo.GetPiecePlacement().Occupancy(boardInfo.ActivePlayer.Toggle(), Piece.Pawn);

            return (pawnsOnBoard & possiblePawnLocationValue) == 0 ? BoardExceptionType.BadEnPassant : BoardExceptionType.None;
        }
    }
}

