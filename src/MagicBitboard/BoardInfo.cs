using System;
using System.Linq;
using System.Text;
using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using static ChessLib.Data.Helpers.PieceHelpers;

namespace MagicBitboard
{
    public class BoardInfo
    {

        public readonly bool Chess960 = false;
        public ushort? EnPassentIndex { get; private set; }
        MoveTree<MoveExt> MoveTree = new MoveTree<MoveExt>(null);
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

        public static BoardInfo BoardInfoFromFen(string fen, bool chess960 = false)
        {
            FENHelpers.ValidateFENStructure(fen);

            var fenPieces = fen.Split(' ');
            var piecePlacement = fenPieces[(int)FENPieces.PiecePlacement];
            FENHelpers.ValidateFENString(fen);
            ulong[][] pieces = new ulong[2][];
            pieces[(int)Color.White] = new ulong[6];
            pieces[(int)Color.Black] = new ulong[6];
            var ranks = piecePlacement.Split('/').Reverse();

            var activePlayer = FENHelpers.GetActiveColor(fenPieces[(int)FENPieces.ActiveColor]);
            ushort? enPassentSquareIndex = BoardHelpers.SquareTextToIndex(fenPieces[(int)FENPieces.EnPassentSquare]);
            var halfmoveClock = FENHelpers.GetMoveNumberFromString(fenPieces[(int)FENPieces.HalfmoveClock]);
            var fullMoveCount = FENHelpers.GetMoveNumberFromString(fenPieces[(int)FENPieces.FullMoveCounter]);
            uint pieceIndex = 0;

            foreach (var rank in ranks)
            {
                foreach (var f in rank)
                {
                    switch (Char.IsDigit(f))
                    {
                        case true:
                            var emptySquares = uint.Parse(f.ToString());
                            pieceIndex += emptySquares;
                            break;
                        case false:
                            var pieceOfColor = PieceHelpers.GetPieceOfColor(f);
                            pieces[(int)pieceOfColor.Color][(int)pieceOfColor.Piece] |= (1ul << (int)pieceIndex);
                            pieceIndex++;
                            break;
                    }
                }
            }

            return new BoardInfo(pieces, activePlayer, FENHelpers.GetCastlingFromString(fenPieces[(int)FENPieces.CastlingAvailability]),
                enPassentSquareIndex, halfmoveClock, fullMoveCount);
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
            ApplyMove(move);
        }

        protected void ApplyMove(MoveExt move)
        {
            GetPiecesAtSourceAandDestination(move, out PieceOfColor? pocSource, out PieceOfColor? pocDestination);
            ValidateMove(move);

            SetAppropriateEnPassentFlag(move, pocSource);
        }

        private void SetAppropriateEnPassentFlag(MoveExt move, PieceOfColor? pocSource)
        {
            if (pocSource.HasValue)
            {
                var startRank = pocSource.Value.Color == Color.White ? 1 : 6;
                var endRank = pocSource.Value.Color == Color.White ? 3 : 4;
                var enPassentIndexOffset = pocSource.Value.Color == Color.White ? 1 : -1;
                if (pocSource.Value.Piece == Piece.Pawn)
                {
                    if (((move.SourceValue & BoardHelpers.RankMasks[startRank]) != 0)
                        && ((move.DestinationValue & BoardHelpers.RankMasks[endRank]) != 0))
                    {
                        EnPassentIndex = (ushort)(move.SourceIndex + enPassentIndexOffset);
                        return;
                    }
                }
            }
            EnPassentIndex = null;
        }

        private void GetPiecesAtSourceAandDestination(MoveExt move, out PieceOfColor? pocSource, out PieceOfColor? pocDestination)
        {
            var sVal = move.SourceValue;
            var dVal = move.DestinationValue;
            pocSource = null;
            pocDestination = null;
            foreach (Piece piece in Enum.GetValues(typeof(Piece)))
            {
                var p = (int)piece;
                if (pocSource == null)
                {
                    if ((PiecesOnBoard[(int)Color.White][p] & sVal) != 0)
                    {
                        pocSource = new PieceOfColor() { Color = Color.White, Piece = piece };
                    }
                    if ((PiecesOnBoard[(int)Color.Black][p] & sVal) != 0)
                    {
                        pocSource = new PieceOfColor() { Color = Color.Black, Piece = piece };
                    }
                }
                if (pocDestination == null)
                {
                    if ((PiecesOnBoard[(int)Color.White][p] & sVal) != 0)
                    {
                        pocDestination = new PieceOfColor() { Color = Color.White, Piece = piece };
                    }
                    if ((PiecesOnBoard[(int)Color.Black][p] & sVal) != 0)
                    {
                        pocDestination = new PieceOfColor() { Color = Color.Black, Piece = piece };
                    }
                }
            }
        }

        public ulong XRayRookAttacks(ushort sqIndex, ulong blockers)
        {
            var rookAttacksFromSquare = Bitboard.GetAttackedSquares(Piece.Rook, sqIndex, TotalOccupancy);
            blockers &= rookAttacksFromSquare;
            return rookAttacksFromSquare ^ Bitboard.GetAttackedSquares(Piece.Rook, sqIndex, TotalOccupancy);
        }

        public ulong XRayBishopAttacks(ushort sqIndex, ulong blockers)
        {
            var BishopAttacksFromSquare = Bitboard.GetAttackedSquares(Piece.Bishop, sqIndex, TotalOccupancy);
            blockers &= BishopAttacksFromSquare;
            return BishopAttacksFromSquare ^ Bitboard.GetAttackedSquares(Piece.Bishop, sqIndex, TotalOccupancy);
        }

        public ulong GetPinnedPieces()
        {
            throw new Exception("Not implemented");
            ulong pinned = 0;
            //var pinner = XRayRookAttacks(ActivePlayerKingIndex, ActiveTotalOccupancy) & opRQ;
            //while (pinner)
            //{
            //    int sq = bitScanForward(pinner);
            //    pinned |= obstructed(sq, squareOfKing) & ownPieces;
            //    pinner &= pinner - 1;
            //}
            //pinner = xrayBishopAttacks(occupiedBB, ownPieces, squareOfKing) & opBQ;
            //while (pinner)
            //{
            //    int sq = bitScanForward(pinner);
            //    pinned |= obstructed(sq, squareOfKing) & ownPieces;
            //    pinner &= pinner - 1;
            //}
            return 0;
        }

        public MoveExt GenerateMoveFromText(string moveText)
        {
            var md = MoveHelpers.GetAvailableMoveDetails(moveText, ActivePlayer);
            if (!md.SourceFile.HasValue || !md.SourceRank.HasValue)
            {
                var sourceIndex = FindPieceSourceIndex(md);
            }
            var moveExt = MoveHelpers.GenerateMove(md.SourceIndex.Value, md.DestinationIndex.Value, md.MoveType, md.PromotionPiece ?? 0);
            return moveExt;
        }

        private ushort FindPieceSourceIndex(MoveDetail md)
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
                    return FindBishopMoveSourceIndex(md);
                case Piece.Rook:
                    return FindRookMoveSourceIndex(md);
                case Piece.Queen:
                    return FindQueenMoveSourceIndex(md);
                case Piece.King:
                    return FindKingMoveSourceIndex(md);
                default: throw new MoveException("Invalid piece specified for move.");
            }
        }

        public ushort FindKingMoveSourceIndex(MoveDetail md)
        {
            var occupancy = ActivePieceOccupancy[(int)Piece.King];
            var possibleSquares = Bitboard.GetAttackedSquares(Piece.King, md.DestinationIndex.Value, TotalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(md.DestinationIndex.Value, possibleSquares, occupancy);
            if (!sourceSquare.HasValue) throw new MoveException("The King can possibly get to the specified destination.");
            return sourceSquare.Value;
        }

        private static ushort? FindPieceMoveSourceIndex(ushort destinationIndes, ulong pieceMoveMask, ulong pieceOccupancy)
        {
            ulong sourceSquares = 0;
            if ((sourceSquares = pieceMoveMask & pieceOccupancy) == 0) return null;
            var indices = sourceSquares.GetSetBits();
            if (indices.Count() != 1) return ushort.MaxValue;
            return indices[0];
        }

        public ushort FindQueenMoveSourceIndex(MoveDetail md)
        {
            var occupancy = ActivePieceOccupancy[(int)Piece.Queen];
            var possibleSquares = Bitboard.GetAttackedSquares(Piece.Queen, md.DestinationIndex.Value, TotalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(md.DestinationIndex.Value, possibleSquares, occupancy);
            if (!sourceSquare.HasValue) throw new MoveException("No Queen can possibly get to the specified destination.");
            if (sourceSquare == ushort.MaxValue) throw new MoveException("More than one Queen can get to the specified square.");
            return sourceSquare.Value;
        }

        public ushort FindRookMoveSourceIndex(MoveDetail md)
        {
            //var possibleSquares = PieceAttackPatternHelper.BishopMoveMask[md.DestRank.Value, md.DestFile.Value];
            var occupancy = ActivePieceOccupancy[(int)Piece.Rook];
            var possibleSquares = Bitboard.GetAttackedSquares(Piece.Rook, md.DestinationIndex.Value, TotalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(md.DestinationIndex.Value, possibleSquares, occupancy);
            if (!sourceSquare.HasValue) throw new MoveException("No Rook can possibly get to the specified destination.");
            if (sourceSquare == ushort.MaxValue) throw new MoveException("More than one Rook can get to the specified square.");
            return sourceSquare.Value;
        }

        public ushort FindBishopMoveSourceIndex(MoveDetail md)
        {
            //var possibleSquares = PieceAttackPatternHelper.BishopMoveMask[md.DestRank.Value, md.DestFile.Value];
            var occupancy = ActivePieceOccupancy[(int)Piece.Bishop];
            var possibleSquares = Bitboard.GetAttackedSquares(Piece.Bishop, md.DestinationIndex.Value, TotalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(md.DestinationIndex.Value, possibleSquares, occupancy);
            if (!sourceSquare.HasValue) throw new MoveException("No Bishop can possibly get to the specified destination.");
            if (sourceSquare == ushort.MaxValue) throw new MoveException("More than one Bishop can get to the specified square.");
            return sourceSquare.Value;
        }

        public ushort FindKnightMoveSourceIndex(MoveDetail md, ulong? relevantPieceOccupancy = null)
        {
            var possibleSquares = PieceAttackPatternHelper.KnightAttackMask[md.DestinationIndex.Value];
            var occupancy = ActivePieceOccupancy[(int)Piece.Knight];
            var sourceSquare = FindPieceMoveSourceIndex(md.DestinationIndex.Value, possibleSquares, occupancy);
            if (!sourceSquare.HasValue) throw new MoveException("No Knight can possibly get to the specified destination.");
            if (sourceSquare == short.MaxValue) throw new MoveException("More than one Knight can get to the specified square.");
            return sourceSquare.Value;
        }

        public ushort FindPawnMoveSourceIndex(MoveDetail md, ulong? relevantPieceOccupancy = null)
        {
            var file = md.DestinationFile;
            var rank = md.Color == Color.Black ? md.DestinationRank.Value.RankCompliment() : md.DestinationRank;
            ushort sourceIndex = 0;
            if (!relevantPieceOccupancy.HasValue) relevantPieceOccupancy = ActivePieceOccupancy[(int)Piece.Pawn];
            var adjustedRelevantPieceOccupancy = md.Color == Color.Black ? relevantPieceOccupancy.Value.FlipVertically() : relevantPieceOccupancy;

            ushort supposedRank = (ushort)(rank - 1);
            if (rank == 3) // 2 possible source ranks, 2 & 3 (offsets 1 & 2)
            {
                //Check 3rd rank first, logically if a pawn is there that is the source
                if ((adjustedRelevantPieceOccupancy & BoardHelpers.RankMasks[2]) != 0) sourceIndex = (ushort)((2 * 8) + (file % 8));
                if ((adjustedRelevantPieceOccupancy & BoardHelpers.RankMasks[1]) != 0) sourceIndex = (ushort)((1 * 8) + (file % 8));
            }
            else //else source square was destination + 8, but we need to make sure a pawn was there
            {
                var supposedIndex = BoardHelpers.RankAndFileToIndex(md.Color == Color.Black ? supposedRank.RankCompliment() : supposedRank, md.DestinationFile.Value);
                if (supposedRank == 0) { throw new MoveException($"{md.MoveText}: Cannot possibly be a pawn at the source square {supposedIndex.IndexToSquareDisplay()} implied by move."); }
                sourceIndex = (ushort)((supposedRank * 8) + md.DestinationFile.Value);
            }

            var idx = md.Color == Color.Black ? sourceIndex.FlipIndexVertically() : sourceIndex;
            ValidatePawnMove(md.Color, idx, md.DestinationIndex.Value, relevantPieceOccupancy.Value, TotalOccupancy, md.MoveText);
            return idx;
        }

        public static void ValidatePawnMove(Color c, ushort sourceIndex, ushort destinationIndex, ulong pawnOccupancy, ulong boardOccupancy, string moveText = "")
        {
            moveText = !string.IsNullOrEmpty(moveText) ? moveText + ": " : "";
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



        public void ValidateMove(MoveExt move)
        {
            var pieceMoving = GetActivePieceByValue(move.SourceValue);
            var isCapture = (OpponentTotalOccupancy & move.DestinationValue) != 0;
            ValidateSourceIsNonVacant(move);
            ValidateDestinationIsNotOccupiedByActiveColor(move);

            switch (move.MoveType)
            {
                case MoveType.Promotion:
                    ValidatePromotion(ActivePlayer, move.SourceIndex, move.DestinationIndex);
                    break;
                default: return;
            }
        }

        private void ValidateDestinationIsNotOccupiedByActiveColor(MoveExt move)
        {
            if ((ActiveTotalOccupancy & move.DestinationValue) != 0)
            {
                //Could be castling move
                var pFrom = PieceOnSquare(move.SourceValue);
                var pTo = PieceOnSquare(move.DestinationValue);
                if (pFrom != Piece.King && pTo != Piece.Rook)
                    throw new MoveException("Move destination is occupied by the active player's color.", move, ActivePlayer);
                else
                    move.MoveType = MoveType.Castle;
            }

        }

        private void ValidateSourceIsNonVacant(MoveExt move)
        {
            if ((ActiveTotalOccupancy & move.SourceValue) == 0) throw new MoveException("Move source square is vacant.", move, ActivePlayer);
        }

        public Piece? PieceOnSquare(ulong squareValue)
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (var p in (Piece[])Enum.GetValues(typeof(Piece)))
                {
                    var nP = (int)p;
                    if ((PiecesOnBoard[i][nP] & squareValue) != 0) return p;
                }
            }
            return null;
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

        public ushort ActivePlayerKingIndex => (ushort)PiecesOnBoard[(int)ActivePlayer][Piece.King.ToInt()].GetSetBits()[0];

        public ulong ActivePlayerKingValue => (ulong)(0x01 << ActivePlayerKingIndex);

        public ushort OpposingPlayerKingIndex => (ushort)PiecesOnBoard[(int)ActivePlayer.Toggle()][Piece.King.ToInt()].GetSetBits()[0];

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

    }
}
