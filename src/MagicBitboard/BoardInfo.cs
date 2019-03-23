using System;
using System.Linq;
using System.Text;
using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using static MagicBitboard.Helpers.MoveHelpers;

namespace MagicBitboard
{
    using Move = UInt16;
    public class BoardInfo
    {
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
            MoveExt move = GenerateMoveFromText(moveText);

        }

        public MoveExt GenerateMoveFromText(string moveText)
        {
            var md = MoveHelpers.GetAvailableMoveDetails(moveText, ActivePlayer);
            if (!md.SourceFile.HasValue || !md.SourceRank.HasValue)
            {
                var sourceIndex = FindPieceSource(md);
            }
            if (md.MoveType == MoveType.Promotion)
            {

            }
            bool isValid = true;

            switch (ActivePlayer)
            {
                case Color.Black:
                    break;
                case Color.White:
                    break;
            }

            return new MoveExt(0);
        }

        private ushort FindPieceSource(MoveDetail md)
        {

            switch (md.Piece)
            {
                case Piece.Pawn:
                    if (md.IsCapture)
                    {
                        throw new MoveException("Could not determine source square for pawn capture.");
                    }
                    return FindPawnMoveSourceIndex(md, ActivePieceOccupancy[(int)Piece.Pawn]);

                case Piece.Knight:
                    return FindKnightMoveSourceIndex(md);
                case Piece.Bishop:
                case Piece.Rook:
                case Piece.Queen:
                case Piece.King:
                    break;

            }
            return 0;
        }

        private ushort FindKnightMoveSourceIndex(MoveDetail md)
        {
            var possibleSquares = PieceAttackPatternHelper.KnightAttackMask[md.DestRank.Value, md.DestFile.Value];
            var occupancy = ActivePieceOccupancy[(int)Piece.Knight];
            var squares = possibleSquares & occupancy;
            if (squares == 0) throw new MoveException("No Knight can possible get to the specified destination.");
            var indices = squares.GetSetBitIndexes();
            if (indices.Count() > 1) throw new MoveException("More than one Knight can get to the specified square.");
            return indices[0];
        }

        public ushort FindPawnMoveSourceIndex(MoveDetail md, ulong? relevantPieceOccupancy = null)
        {
            var file = md.DestFile;
            var rank = md.Color == Color.Black ? md.DestRank.Value.RankCompliment() : md.DestRank;
            ushort sourceIndex = 0;
            if (!relevantPieceOccupancy.HasValue) relevantPieceOccupancy = ActivePieceOccupancy[(int)Piece.Pawn];
            var adjustedRelevantPieceOccupancy = md.Color == Color.Black ? relevantPieceOccupancy.Value.FlipVertically() : relevantPieceOccupancy;
            if (md.Color == Color.Black)
            {
                //if (md.DestRank == 4) // 2 possible source ranks, 6 & 7 (offsets 2 & 3)
                //{
                //    //Check 6th rank first, logically if a pawn is there that is the source
                //    if ((relevantPieceOccupancy & BoardHelpers.RankMasks[5]) != 0) sourceIndex = (ushort)((5 * 8) + file % 8);
                //    if ((relevantPieceOccupancy & BoardHelpers.RankMasks[6]) != 0) sourceIndex = (ushort)((6 * 8) + file % 8);
                //}
                //else //else source square was destination + 8, but we need to make sure a pawn was there
                //{
                //    var supposedRank = (ushort)md.DestRank + 1;
                //    if (md.DestRank > 6) { throw new MoveException($"{md.MoveText}: Cannot possibly be a pawn at the source square implied by move."); }
                //    sourceIndex = (ushort)((supposedRank * 8) + md.DestFile.Value);
                //}

            }
            ushort supposedRank = (ushort)(rank - 1);
            if (rank == 3) // 2 possible source ranks, 2 & 3 (offsets 1 & 2)
            {
                //Check 3rd rank first, logically if a pawn is there that is the source
                if ((adjustedRelevantPieceOccupancy & BoardHelpers.RankMasks[2]) != 0) sourceIndex = (ushort)((2 * 8) + (file % 8));
                if ((adjustedRelevantPieceOccupancy & BoardHelpers.RankMasks[1]) != 0) sourceIndex = (ushort)((1 * 8) + (file % 8));
            }
            else //else source square was destination + 8, but we need to make sure a pawn was there
            {

                var supposedIndex = BoardHelpers.RankAndFileToIndex(md.Color == Color.Black ? supposedRank.RankCompliment() : supposedRank, md.DestFile.Value);
                if (supposedRank == 0) { throw new MoveException($"{md.MoveText}: Cannot possibly be a pawn at the source square {supposedIndex.IndexToSquareDisplay()} implied by move."); }
                sourceIndex = (ushort)((supposedRank * 8) + md.DestFile.Value);
            }

            var idx = md.Color == Color.Black ? sourceIndex.FlipIndexVertically() : sourceIndex;
            ValidatePawnMove(md.Color, idx, md.DestinationIndex.Value, relevantPieceOccupancy.Value, TotalOccupancy, md.MoveText);
            return idx;
        }
        public static void ValidatePawnMove(Color c, ushort sourceIndex, ushort destinationIndex, ulong pawnOccupancy, ulong boardOccupancy, string moveText = "")
        {
            moveText = moveText != "" ? moveText + ": " : "";
            var sourceValue = sourceIndex.IndexToValue();
            var isCapture = sourceIndex.FileFromIdx() != destinationIndex.FileFromIdx();
            var destValue = destinationIndex.IndexToValue();
            //validate pawn is at supposed source
            var pawnAtSource = sourceValue & pawnOccupancy;
            if (pawnAtSource == 0) throw new MoveException($"There is no pawn on {sourceIndex.IndexToSquareDisplay()} to move to {destinationIndex.IndexToSquareDisplay()}.");

            //validate pawn move to square is valid
            var pawnMoves = isCapture ? PieceAttackPatternHelper.PawnAttackMask[(int)c][sourceIndex] : PieceAttackPatternHelper.PawnMoveMask[(int)c][sourceIndex];
            if ((pawnMoves & destValue) == 0)
            {

                throw new MoveException($"{moveText}Pawn from {sourceIndex.IndexToSquareDisplay()} to {destinationIndex.IndexToSquareDisplay()} is illegal.");
            }

            var destinationOccupancy = (destValue & boardOccupancy);
            //validate pawn is not blocked from move, if move is not a capture
            if (!isCapture)
            {
                if (destinationOccupancy != 0)
                    throw new MoveException($"{moveText}Destination square is occupied.");
            }
            else // validate Piece is on destination for capture
            {
                if (destinationOccupancy == 0)
                    throw new MoveException($"{moveText}Destination capture square is unoccupied.");
            }

        }
        public Piece GetActivePieceByValue(ulong pieceInSquareValue)
        {
            for (Piece p = 0; p < Piece.King; p++)
            {
                if ((ActivePieceOccupancy[(int)p] & pieceInSquareValue) != 0) return p;
            }
            throw new MoveException("No piece found with the specified value.");
        }

        private void ApplyMove(MoveExt move)
        {
            ValidateMove(move);
        }

        public void ValidateMove(MoveExt move)
        {
            var pieceMoving = GetActivePieceByValue(move.SourceValue);
            var isCapture = (OpponentTotalOccupancy & move.DestinationValue) != 0;

            switch (move.MoveType)
            {
                case MoveType.Promotion:
                    ValidatePromotion(ActivePlayer, move.SourceIndex, move.DestinationIndex);
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

        public void ValidatePromotion(Color color, ushort moveSourceIdx, ushort moveDestIdx)
        {
            var moveSourceVal = 1ul << moveSourceIdx;
            var moveDestVal = 1ul << moveDestIdx;

            if ((ActivePieceOccupancy[(int)Piece.Pawn] & moveSourceVal) == 0)
            {
                throw new MoveException("Promotion move issue - no pawn at source.");
            }
            else if ((TotalOccupancy & moveDestVal) != 0 && (PieceAttackPatternHelper.PawnAttackMask[(int)color][moveDestIdx] == 0))
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
            if (castlingAvailability.HasFlag(CastlingAvailability.WhiteQueenside) || castlingAvailability.HasFlag(CastlingAvailability.WhiteKingside)
                && !piecesOnBoard[white][king].IsBitSet(4))
            {
                message.AppendLine("White cannot castle witout the King on e1.");
            }
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) || castlingAvailability.HasFlag(CastlingAvailability.BlackKingside)
                && !piecesOnBoard[black][king].IsBitSet(60))
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
            if ((PieceAttackPatternHelper.PawnAttackMask[notNColor][squareIndex] & PiecesOnBoard[nColor][Piece.Pawn.ToInt()]) != 0) return true;
            if ((PieceAttackPatternHelper.KnightAttackMask[r, f] & PiecesOnBoard[nColor][Piece.Knight.ToInt()]) != 0) return true;
            if ((bishopAttack & (PiecesOnBoard[nColor][Piece.Bishop.ToInt()] | PiecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((rookAttack & (PiecesOnBoard[nColor][Piece.Rook.ToInt()] | PiecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((PieceAttackPatternHelper.KingMoveMask[r, f] & PiecesOnBoard[nColor][Piece.King.ToInt()]) != 0) return true;
            return false;
        }

        public ulong[][] PiecesOnBoard = new ulong[2][];
        public CastlingAvailability CastlingAvailability { get; set; }
        public string FEN { get; }
        public uint HalfmoveClock { get; set; }
        public uint MoveCounter { get; set; }
        public Color ActivePlayer { get; set; }
        public ushort? EnPassentIndex { get; set; }
    }
}
