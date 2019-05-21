using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System.Linq;
using System.Text;

namespace ChessLib.Data.Boards
{
    public abstract class BoardInformationService<T> : BoardFENInfo, IBoardInformationService<T> where T : IMoveStorage
    {
        public abstract IMoveTree<T> MoveTree { get; set; }

        public IMoveTree<T> Moves
        {
            get { return MoveTree; }
        }

        /// <summary>
        ///     Creates a BoardInfo object given FEN notation.
        /// </summary>
        /// <param name="fen">string of FEN notation.</param>
        /// <param name="chess960">[unused right now] Is this a chess960 position (possible non-standard starting position)</param>
        /// <returns>A BoardInfo object representing the board state after parsing the FEN</returns>
        //public BoardInformationService(string fen, bool chess960 = false) : base(fen, chess960)
        //{
        //    ValidateFields();
        //}
        public abstract void ApplyMove(string moveText);
        public abstract ulong GetAttackedSquares(Piece p, ushort index, ulong occupancy);

        #region General Board Information







        #endregion

       

        




       

        private void ValidateFields()
        {
            var errors = new StringBuilder();
            errors.AppendLine(ValidateNumberOfPiecesOnBoard());
            errors.AppendLine(ValidateEnPassantSquare());
            errors.AppendLine(ValidateCastlingRights());
            errors.AppendLine(ValidateChecks());
        }

        public string ValidateNumberOfPiecesOnBoard(ulong[][] piecesOnBoard)
        {
            var message = new StringBuilder("");
            if (piecesOnBoard[Color.White.ToInt()].Sum(x => x.CountSetBits()) > 16)
                message.AppendLine("White has too many pieces on the board.");
            if (piecesOnBoard[Color.Black.ToInt()].Sum(x => x.CountSetBits()) > 16)
                message.AppendLine("Black has too many pieces on the board.");
            return message.ToString();
        }

        public static string ValidateEnPassantSquare(ulong[][] piecesOnBoard, ushort? enPassantSquare,
            Color activePlayer)
        {
            if (enPassantSquare == null) return "";
            if (activePlayer == Color.White && (enPassantSquare < 40 || enPassantSquare > 47)
                ||
                activePlayer == Color.Black && (enPassantSquare < 16 || enPassantSquare > 23))
                return "Bad En Passant Square detected.";
            return "";
        }

        public static string ValidateCastlingRights(ulong[][] piecesOnBoard, CastlingAvailability castlingAvailability,
            bool chess960 = false)
        {
            if (castlingAvailability == CastlingAvailability.NoCastlingAvailable) return "";
            var message = new StringBuilder();
            var white = (int)Color.White;
            var black = (int)Color.Black;
            var rook = ROOK;
            var king = (int)Piece.King;
            //Check for Rook placement
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) &&
                !piecesOnBoard[white][rook].IsBitSet(0))
                message.AppendLine("White cannot castle long with no Rook on a1.");
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside) &&
                !piecesOnBoard[white][rook].IsBitSet(7))
                message.AppendLine("White cannot castle short with no Rook on h1.");
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) &&
                !piecesOnBoard[black][rook].IsBitSet(56))
                message.AppendLine("Black cannot castle long with no Rook on a8.");
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackKingside) &&
                !piecesOnBoard[black][rook].IsBitSet(63))
                message.AppendLine("Black cannot castle short with no Rook on h8.");

            //Check for King placement
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) ||
                castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside)
                && !piecesOnBoard[white][king].IsBitSet(4))
                message.AppendLine("White cannot castle without the King on e1.");
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) ||
                castlingAvailability.HasFlag(CastlingAvailability.BlackKingside)
                && !piecesOnBoard[black][king].IsBitSet(60))
                message.AppendLine("Black cannot castle without the King on e1.");
            return message.ToString();
        }


        private string ValidateNumberOfPiecesOnBoard()
        {
            return ValidateNumberOfPiecesOnBoard(PiecePlacement);
        }

        private string ValidateEnPassantSquare()
        {
            return ValidateEnPassantSquare(PiecePlacement, EnPassantSquare, ActivePlayer);
        }

        private string ValidateCastlingRights()
        {
            return ValidateCastlingRights(PiecePlacement, CastlingAvailability);
        }

        public string ValidateChecks()
        {
            if (((IBoard)this).IsOpponentInCheck())
            {
                return "Illegal position- opponent is in check.";
            }

            return "";
        }




        public abstract bool DoesPieceAtSquareAttackSquare(ushort attackerSquare, ushort attackedSquare,
            Piece attackerPiece);

        #region Constant Piece Values for Indexing arrays
        // ReSharper disable InconsistentNaming
        protected const int PAWN = (int)Piece.Pawn;
        protected const int BISHOP = (int)Piece.Bishop;
        protected const int KNIGHT = (int)Piece.Knight;
        protected const int ROOK = (int)Piece.Rook;
        protected const int QUEEN = (int)Piece.Queen;
        protected const int KING = (int)Piece.King;

        protected const int BLACK = (int)Color.Black;

        protected const int WHITE = (int)Color.White;
        // ReSharper restore InconsistentNaming

        #endregion





    }
}
