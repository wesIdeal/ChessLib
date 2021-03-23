using System;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;

namespace ChessLib.Data.Helpers
{
    public static class PieceHelpers
    {
        internal static readonly int King = (int) Piece.King;
        internal static readonly int Queen = (int) Piece.Queen;
        internal static readonly int Rook = (int) Piece.Rook;
        internal static readonly int Bishop = (int) Piece.Bishop;
        internal static readonly int Knight = (int) Piece.Knight;
        internal static readonly int Pawn = (int) Piece.Pawn;


        /// <summary>
        ///     Returns uppercase character representing promotion pieces
        /// </summary>
        /// <exception cref="PieceException">
        ///     Thrown if promotion piece is not found in enumeration. Highly unlikely and indicative
        ///     of another problem.
        /// </exception>
        public static char GetCharFromPromotionPiece(PromotionPiece p)
        {
            switch (p)
            {
                case PromotionPiece.Bishop: return 'B';
                case PromotionPiece.Knight: return 'N';
                case PromotionPiece.Queen: return 'Q';
                case PromotionPiece.Rook: return 'R';
                default:
                    throw new PieceException(
                        $"Promotion Piece not found in switch cases.{Environment.NewLine}(Exception is not likely and probably indicative of another issue.)");
            }
        }

        /// <summary>
        ///     Gets a lowercase character to represent a piece
        /// </summary>
        public static char GetCharRepresentation(this Piece p)
        {
            switch (p)
            {
                case Piece.Bishop: return 'b';
                case Piece.Knight: return 'n';
                case Piece.Rook: return 'r';
                case Piece.Queen: return 'q';
                case Piece.King: return 'k';
                case Piece.Pawn: return 'p';
                default: throw new Exception("Piece's char representation not found.");
            }
        }

        /// <summary>
        ///     Gets the PremoveFEN character representation for a piece.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static char GetFENCharPieceRepresentation(Color c, Piece p)
        {
            var pChar = GetCharRepresentation(p);
            if (c == Color.Black)
            {
                return char.ToLower(pChar);
            }

            return char.ToUpper(pChar);
        }

        /// <summary>
        ///     Gets a Piece <see cref="Piece" />
        /// </summary>
        /// <param name="strPiece">String representation of a piece</param>
        /// <returns>A Piece <see cref="Piece" /> enumeration value.</returns>
        /// <remarks>
        ///     <paramref name="strPiece">strPiece</paramref>should be 1 character. Calls
        ///     <see cref="GetPiece(char)">GetPiece(char).</see>
        /// </remarks>
        public static Piece GetPiece(string strPiece)
        {
            if (strPiece.Length > 1) throw new Exception("Piece should be a single char.");
            return GetPiece(strPiece[0]);
        }

        /// <summary>
        ///     Gets a Piece <see cref="Piece" />
        /// </summary>
        public static Piece GetPiece(char p)
        {
            switch (char.ToLower(p))
            {
                case 'k':
                    return Piece.King;
                case 'n':
                    return Piece.Knight;
                case 'b':
                    return Piece.Bishop;
                case 'p':
                    return Piece.Pawn;
                case 'r':
                    return Piece.Rook;
                case 'q':
                    return Piece.Queen;
                default: throw new PieceException("Cannot determine piece for " + p);
            }
        }


        /// <summary>
        ///     Gets a piece and it's color from a character.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static PieceOfColor GetPieceOfColor(char p)
        {
            var poc = new PieceOfColor {Color = char.IsUpper(p) ? Color.White : Color.Black, Piece = GetPiece(p)};
            return poc;
        }

        /// <summary>
        ///     Gets a PromotionPiece object from character (uppercase or lowercase).
        /// </summary>
        /// <remarks>Piece case is insensitive, as this should work with both SAN and LAN promotion strings.</remarks>
        /// <param name="promotionPieceCharacter">
        ///     The piece represented by promotion string. If null, this returns
        ///     <see cref="PromotionPiece" />.Knight, as this is specified for the condensed move in <see cref="ushort" /> format.
        /// </param>
        /// <returns>
        ///     The <see cref="PromotionPiece" />PromotionPiece enum value represented by
        ///     <param name="promotionPieceCharacter">promotionPieceCharacter</param>
        /// </returns>
        /// <exception cref="PieceException">
        ///     <param name="promotionPieceCharacter"></param>
        ///     is not a valid promotion character [null|n|b|r|q], case insensitive.
        /// </exception>
        public static PromotionPiece GetPromotionPieceFromChar(char? promotionPieceCharacter)
        {
            if (promotionPieceCharacter == null) return PromotionPiece.Knight;
            switch (char.ToUpper(promotionPieceCharacter.Value))
            {
                case 'B': return PromotionPiece.Bishop;
                case 'N': return PromotionPiece.Knight;
                case 'Q': return PromotionPiece.Queen;
                case 'R': return PromotionPiece.Rook;
                default: throw new Exception("Char / Piece not found in switch cases.");
            }
        }
    }
}