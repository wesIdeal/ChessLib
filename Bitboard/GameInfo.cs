using System;
using System.Collections;
using System.Linq;
using System.Text;
using MagicBitboard.Enums;
using MagicBitboard.Helpers;

namespace MagicBitboard
{
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

        private string ValidateChecks() => ValidateChecks(PiecesOnBoard);

        public string ValidateChecks(ulong[][] piecesOnBoard)
        {
            Check c = GetChecks(ActivePlayer);
        }

        public static uint[] GetSquareIndexesFromValue(ulong val)
        {
            Array.ConvertAll<>
        }

        private Check GetChecks(Color activePlayer)
        {
            var square = PiecesOnBoard[(int)activePlayer.Toggle()][Piece.King.ToInt()];
            
            if (IsAttackedBy((int)activePlayer.Toggle(), ) ;
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
            if ((PieceAttackPatternHelper.PawnAttackMask[notNColor, r, f] & PiecesOnBoard[nColor][Piece.Pawn.ToInt()]) != 0) return true;
            if ((PieceAttackPatternHelper.KnightAttackMask[r, f] & PiecesOnBoard[nColor][Piece.Knight.ToInt()]) != 0) return true;
            if ((PieceAttackPatternHelper.BishopAttackMask[r, f] & (PiecesOnBoard[nColor][Piece.Bishop.ToInt()]) | PiecesOnBoard[nColor][Piece.Queen.ToInt()]) != 0) return true;
            if ((PieceAttackPatternHelper.RookAttackMask[r, f] & (PiecesOnBoard[nColor][Piece.Knight.ToInt()] | PiecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
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

    public class GameInfo
    {
        public const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private readonly Bitboard BitBoard;
        public readonly BoardInfo BoardInfo;

        public GameInfo() : this(new Bitboard())
        {

        }

        public GameInfo(Bitboard bitboard) : this(bitboard, InitialFEN)
        {

        }

        public GameInfo(Bitboard bitboard, string fen)
        {
            _fen = fen;
            Bitboard = bitboard;
            BoardInfo = FENHelpers.BoardInfoFromFen(fen);
        }

        private string _fen;

        public Bitboard Bitboard { get; }
    }
}
