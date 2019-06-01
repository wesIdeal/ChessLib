using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System.Linq;
using System.Text;

namespace ChessLib.Data.Boards
{

    ////private void ValidateFields()
    ////{
    ////    var errors = new StringBuilder();
    ////    errors.AppendLine(ValidateNumberOfPiecesOnBoard());
    ////    errors.AppendLine(ValidateEnPassantSquare());
    ////    errors.AppendLine(ValidateCastlingRights());
    ////    errors.AppendLine(ValidateChecks());
    ////}

    //public string ValidateNumberOfPiecesOnBoard(ulong[][] piecesOnBoard)
    //{
    //    var message = new StringBuilder("");
    //    if (piecesOnBoard[Color.White.ToInt()].Sum(x => x.CountSetBits()) > 16)
    //        message.AppendLine("White has too many pieces on the board.");
    //    if (piecesOnBoard[Color.Black.ToInt()].Sum(x => x.CountSetBits()) > 16)
    //        message.AppendLine("Black has too many pieces on the board.");
    //    return message.ToString();
    //}

    //public static string ValidateEnPassantSquare(ulong[][] piecesOnBoard, ushort? enPassantSquare,
    //    Color activePlayer)
    //{
    //    if (enPassantSquare == null) return "";
    //    if (activePlayer == Color.White && (enPassantSquare < 40 || enPassantSquare > 47)
    //        ||
    //        activePlayer == Color.Black && (enPassantSquare < 16 || enPassantSquare > 23))
    //        return "Bad En Passant Square detected.";
    //    return "";
    //}

    //public static string ValidateCastlingRights(ulong[][] piecesOnBoard, CastlingAvailability castlingAvailability,
    //    bool chess960 = false)
    //{
    //    if (castlingAvailability == CastlingAvailability.NoCastlingAvailable) return "";
    //    var message = new StringBuilder();
    //    var white = (int)Color.White;
    //    var black = (int)Color.Black;
    //    var rook = ROOK;
    //    var king = (int)Piece.King;
    //    //Check for Rook placement
    //    if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) &&
    //        !piecesOnBoard[white][rook].IsBitSet(0))
    //        message.AppendLine("White cannot castle long with no Rook on a1.");
    //    if (castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside) &&
    //        !piecesOnBoard[white][rook].IsBitSet(7))
    //        message.AppendLine("White cannot castle short with no Rook on h1.");
    //    if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) &&
    //        !piecesOnBoard[black][rook].IsBitSet(56))
    //        message.AppendLine("Black cannot castle long with no Rook on a8.");
    //    if (castlingAvailability.HasFlag(CastlingAvailability.BlackKingside) &&
    //        !piecesOnBoard[black][rook].IsBitSet(63))
    //        message.AppendLine("Black cannot castle short with no Rook on h8.");

    //    //Check for King placement
    //    if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) ||
    //        castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside)
    //        && !piecesOnBoard[white][king].IsBitSet(4))
    //        message.AppendLine("White cannot castle without the King on e1.");
    //    if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) ||
    //        castlingAvailability.HasFlag(CastlingAvailability.BlackKingside)
    //        && !piecesOnBoard[black][king].IsBitSet(60))
    //        message.AppendLine("Black cannot castle without the King on e1.");
    //    return message.ToString();
    //}


    //private string ValidateNumberOfPiecesOnBoard()
    //{
    //    return ValidateNumberOfPiecesOnBoard(GetPiecePlacement);
    //}

    //private string ValidateEnPassantSquare()
    //{
    //    return ValidateEnPassantSquare(PieceP, EnPassantSquare, ActivePlayer);
    //}

    //private string ValidateCastlingRights()
    //{
    //    return ValidateCastlingRights(GetPiecePlacement, CastlingAvailability);
    //}

    //public string ValidateChecks()
    //{
    //    if (((IBoard)this).IsOpponentInCheck())
    //    {
    //        return "Illegal position- opponent is in check.";
    //    }

    //    return "";
    //}










}
//}
