using System;
using System.Linq;
using System.Text;
using MagicBitboard.Enums;
using MagicBitboard.Helpers;

namespace MagicBitboard
{
    using Move = UInt16;
    public class BoardInfo
    {
        private readonly string _fen;
        public readonly bool Chess960 = false;
        private BoardInfo(bool chess960 = false)
        {
            Chess960 = chess960;
        }

        public BoardInfo(ulong[][] piecesOnBoard, Color activePlayer, CastlingAvailability castlingAvailability, ushort? enPassentIndex, uint halfmoveClock, uint moveCounter, bool chess960 = false)
        {
            PiecesOnBoard = piecesOnBoard;
            ActivePlayer = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassentIndex = enPassentIndex;
            HalfmoveClock = halfmoveClock;
            MoveCounter = moveCounter;
            Chess960 = chess960;

            ValidateFields();
        }

        private void ValidateFields()
        {
            var errors = new StringBuilder();
            errors.AppendLine(ValidateNumberOfPiecesOnBoard());
            errors.AppendLine(ValidateEnPassentSquare());
            errors.AppendLine(ValidateCastlingRights());
            errors.AppendLine(ValidateChecks());
        }

        public void ApplyMove(string moveText)
        {
            Move move = MoveHelpers.GenerateMoveFromText(moveText, ActivePlayer);
            ApplyMove(move);
        }

        public Piece GetActivePieceByValue(ulong pieceInSquareValue)
        {
            for (Piece p = 0; p < Piece.King; p++)
            {
                if ((ActivePieceOccupancy[(int)p] & pieceInSquareValue) != 0) return p;
            }
            throw new MoveException("No piece found with the specified value.");
        }

        private void ApplyMove(ushort move)
        {
            ValidateMove(move);
        }

        private void ValidateMove(ushort move)
        {
            var source = 1ul << move.Source();
            var dest = 1ul << move.Destination();
            var pieceMoving = GetActivePieceByValue(source);
            var isCapture = (OpponentTotalOccupancy & dest) != 0;


            switch (move.GetMoveType())
            {
                case MoveType.Promotion:
                    ValidatePromotion(move);
                    break;
                default: return;
            }
        }

        //private string ValidateChecks() => ValidateChecks(PiecesOnBoard);
        public ulong[] ActivePieceOccupancy => PiecesOnBoard[(int)ActivePlayer];
        public ulong[] OpponentPieceOccupancy => PiecesOnBoard[(int)ActivePlayer.Toggle()];
        public ulong ActiveTotalOccupancy
        {
            get
            {
                ulong rv = 0;
                for (int i = 0; i < ActivePieceOccupancy.Length; i++) rv |= ActivePieceOccupancy[i];
                return rv;
            }
        }
        public ulong OpponentTotalOccupancy
        {
            get
            {
                ulong rv = 0;
                for (int i = 0; i < OpponentPieceOccupancy.Length; i++) rv |= OpponentPieceOccupancy[i];
                return rv;
            }
        }
        public ulong TotalOccupancy
        {
            get
            {
                ulong rv = 0;
                for (int i = 0; i < ActivePieceOccupancy.Length; i++) rv |= (PiecesOnBoard[0][i]) | PiecesOnBoard[1][i];
                return rv;
            }
        }

        public string ValidateChecks()
        {
            Check c = GetChecks(ActivePlayer);
            if (c.HasFlag(Check.Double))
            {
                return "Both Kings are in check.";
            }
            else if (c.HasFlag(Check.Opposite))
            {
                return "Active side is in check.";
            }
            return "";
        }

        public ushort ActivePlayerKingIndex => PiecesOnBoard[(int)ActivePlayer][Piece.King.ToInt()].GetSetBitIndexes()[0];
        public ushort OpposingPlayerKingIndex => PiecesOnBoard[(int)ActivePlayer.Toggle()][Piece.King.ToInt()].GetSetBitIndexes()[0];

        private void ValidatePromotion(Move move)
        {
            if ((ActivePieceOccupancy[(int)Piece.Pawn] & ((ulong)1 << move.Source())) == 0)
            {
                throw new MoveException("Promotion move issue - no pawn at source.");
            }
            else if ((TotalOccupancy & move.Destination()) != 0)
            {
                throw new MoveException("Promotion move issue - A piece is at the destination.");
            }
        }



        private Check GetChecks(Color activePlayer)
        {
            Check rv = Check.None;

            var currentPlayerInCheck = IsAttackedBy(activePlayer.Toggle(), ActivePlayerKingIndex);
            var opposingPlayerInCheck = IsAttackedBy(activePlayer, OpposingPlayerKingIndex);
            if (currentPlayerInCheck && opposingPlayerInCheck)
            {
                rv &= ~Check.None;
                rv |= Check.Double;
            }
            else if (opposingPlayerInCheck)
            {
                rv &= ~Check.None;
                rv |= Check.Opposite;
            }
            else if (currentPlayerInCheck)
            {
                rv &= ~Check.None;
                rv |= Check.Normal;
            }
            return rv;
        }

        public static string ValidateNumberOfPiecesOnBoard(ulong[][] piecesOnBoard)
        {
            var message = new StringBuilder("");
            if (piecesOnBoard[Color.White.ToInt()].Sum(x => x.CountSetBits()) > 16)
            {
                message.AppendLine("White has too many pieces on the board.");
            }
            if (piecesOnBoard[Color.Black.ToInt()].Sum(x => x.CountSetBits()) > 16)
            {
                message.AppendLine("Black has too many pieces on the board.");
            }
            return message.ToString();
        }
        public static string ValidateEnPassentSquare(ulong[][] piecesOnBoard, ushort? enPassentSquare, Color activePlayer)
        {
            if (enPassentSquare == null) return "";
            var message = new StringBuilder("");
            if ((activePlayer == Color.White && (enPassentSquare < 40 || enPassentSquare > 47))
                ||
                (activePlayer == Color.Black && (enPassentSquare < 16 || enPassentSquare > 23)))
            {
                return "Bad En Passent Square deteced.";
            }
            return "";
        }
        public static string ValidateCastlingRights(ulong[][] piecesOnBoard, CastlingAvailability castlingAvailability, bool chess960 = false)
        {
            if (castlingAvailability == CastlingAvailability.NoCastlingAvailable) return "";
            var message = new StringBuilder();
            var white = (int)Color.White;
            var black = (int)Color.Black;
            var rook = (int)Piece.Rook;
            var king = (int)Piece.King;
            //Check for Rook placement
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) && !piecesOnBoard[white][rook].IsBitSet(0))
            {
                message.AppendLine("White cannot castle long with no Rook on a1.");
            }
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside) && !piecesOnBoard[white][rook].IsBitSet(7))
            {
                message.AppendLine("White cannot castle short with no Rook on h1.");
            }
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) && !piecesOnBoard[black][rook].IsBitSet(56))
            {
                message.AppendLine("Black cannot castle long with no Rook on a8.");
            }
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackKingside) && !piecesOnBoard[black][rook].IsBitSet(63))
            {
                message.AppendLine("Black cannot castle short with no Rook on h8.");
            }

            //Check for King placement
            if ((castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) || (castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside))
                && !piecesOnBoard[white][king].IsBitSet(4)))
            {
                message.AppendLine("White cannot castle witout the King on e1.");
            }
            if ((castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) || (castlingAvailability.HasFlag(CastlingAvailability.BlackKingside))
                && !piecesOnBoard[black][king].IsBitSet(60)))
            {
                message.AppendLine("Black cannot castle witout the King on e1.");
            }
            return message.ToString();
        }


        private string ValidateNumberOfPiecesOnBoard() => ValidateNumberOfPiecesOnBoard(PiecesOnBoard);
        private string ValidateEnPassentSquare() => ValidateEnPassentSquare(PiecesOnBoard, EnPassentIndex, ActivePlayer);
        private string ValidateCastlingRights() => ValidateCastlingRights(PiecesOnBoard, CastlingAvailability, Chess960);

        public bool IsAttackedBy(Color color, ushort squareIndex)
        {

            var nColor = (int)color;
            var notNColor = nColor ^ 1;
            var r = squareIndex / 8;
            var f = squareIndex % 8;
            var totalOccupancy = TotalOccupancy;
            var bishopAttack = Bitboard.GetAttackedSquares(Piece.Bishop, squareIndex, totalOccupancy);
            var rookAttack = Bitboard.GetAttackedSquares(Piece.Rook, squareIndex, totalOccupancy);
            if ((PieceAttackPatternHelper.PawnAttackMask[notNColor, r, f] & PiecesOnBoard[nColor][Piece.Pawn.ToInt()]) != 0) return true;
            if ((PieceAttackPatternHelper.KnightAttackMask[r, f] & PiecesOnBoard[nColor][Piece.Knight.ToInt()]) != 0) return true;
            if ((bishopAttack & (PiecesOnBoard[nColor][Piece.Bishop.ToInt()] | PiecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((rookAttack & (PiecesOnBoard[nColor][Piece.Rook.ToInt()] | PiecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((PieceAttackPatternHelper.KingMoveMask[r, f] & PiecesOnBoard[nColor][Piece.King.ToInt()]) != 0) return true;
            return false;
        }

        public ulong[][] PiecesOnBoard = new ulong[2][];
        public CastlingAvailability CastlingAvailability { get; set; }
        public string FEN { get => _fen; }
        public uint HalfmoveClock { get; set; }
        public uint MoveCounter { get; set; }
        public Color ActivePlayer { get; set; }
        public ushort? EnPassentIndex { get; set; }
    }
}
