using ChessLib.Data.Boards;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.BoardValidators;
using ChessLib.Validators.MoveValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ChessLib.Data
{
    public class BoardInfo : BoardBase
    {
        public MoveTree<MoveStorage> MoveTree { get; set; }

        public BoardInfo() : this(FENHelpers.FENInitial, false) { }

        public BoardInfo(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability,
            ushort? enPassantIdx, uint? halfMoveClock, uint fullMoveCount, bool validationException = true) : base(occupancy, activePlayer,
            castlingAvailability, enPassantIdx, halfMoveClock, fullMoveCount)
        {
            BoardValidator validator = new BoardValidator(this);
            validator.Validate(validationException);
        }

        public BoardInfo(string fen, bool is960 = false)
        {
            MoveTree = new MoveTree<MoveStorage>(null);
            _piecePlacement = FENHelpers.BoardFromFen(fen, out Color active, out CastlingAvailability ca, out ushort? enPassant, out uint hmClock, out uint fmClock);
            ActivePlayer = active;
            CastlingAvailability = ca;
            EnPassantSquare = enPassant;
            HalfmoveClock = hmClock;
            FullmoveCounter = fmClock;
            Chess960 = is960;
            InitialFEN = this.ToFEN();
        }

        public ulong ActiveRookOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Rook);
        public ulong ActiveKnightOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Knight);
        public ulong ActivePawnOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Pawn);
        public ulong ActiveQueenOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Queen);
        public ulong ActiveBishopOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Bishop);

        public void ApplyMove(string moveText)
        {
            var move = this.GenerateMoveFromText(moveText);
            ApplyMove(move);
        }

        public MoveExceptionType? ApplyMove(MoveExt move)
        {
            var pocSource = this.GetPieceOfColorAtIndex(move.SourceIndex);
            var pocDestination = this.GetPieceOfColorAtIndex(move.DestinationIndex);
            if (pocSource == null) throw new ArgumentException("No piece at source.");
            var moveValidator = new MoveValidator(this, move);
            var validationError = moveValidator.Validate();
            if (validationError.HasValue)
                throw new MoveException("Error with move.", validationError.Value, move, ActivePlayer);
            ApplyValidatedMove(move);
            return null;
        }

        public void ApplyValidatedMove(MoveExt move)
        {
            var pocSource = this.GetPieceOfColorAtIndex(move.SourceIndex);
            var san = move.MoveToSAN(this, MoveTree.FirstMove == null);
            MoveTree.AddVariation(new MoveNode<MoveStorage>(new MoveStorage(this.ToFEN(), move, pocSource.Value.Piece, ActivePlayer, san)));
            var newBoard = this.ApplyMoveToBoard(move);
            this._piecePlacement = newBoard.GetPiecePlacement();
            this.ActivePlayer = newBoard.ActivePlayer;
            this.CastlingAvailability = newBoard.CastlingAvailability;
            this.EnPassantSquare = newBoard.EnPassantSquare;
            this.HalfmoveClock = newBoard.HalfmoveClock;
            this.FullmoveCounter = newBoard.FullmoveCounter;

        }

        public ulong GetPinnedPieces()
        {
            ulong pinned = 0;
            var xRayBishopAttacks = this.XRayBishopAttacks(ActivePlayerKingIndex);
            var xRayRookAttacks = this.XRayRookAttacks(ActivePlayerKingIndex);
            var oppQueenLocations = GetPiecePlacement().Occupancy(this.OpponentColor(), Piece.Queen);
            var oppBishopLocations = GetPiecePlacement().Occupancy(this.OpponentColor(), Piece.Bishop);
            var oppRookLocations = GetPiecePlacement().Occupancy(this.OpponentColor(), Piece.Rook);
            var bishopPinnedPieces = (oppBishopLocations | oppQueenLocations) & xRayBishopAttacks;
            var rookPinnedPieces = (oppRookLocations | oppQueenLocations) & xRayRookAttacks;
            var allPins = rookPinnedPieces | bishopPinnedPieces;
            while (allPins != 0)
            {
                var square = BitHelpers.BitScanForward(allPins);
                var squaresBetween = BoardHelpers.InBetween(square, ActivePlayerKingIndex);
                var piecesBetween = squaresBetween & GetPiecePlacement().Occupancy(ActivePlayer);
                if (piecesBetween.CountSetBits() == 1) pinned |= piecesBetween;
                allPins &= allPins - 1;
            }

            return pinned;
        }


        public string GetLANStringToCurrent(bool startPositionToFEN = false)
        {

            Stack<string> lanStack = new Stack<string>();

            var sb = new StringBuilder();

            return sb.ToString();
        }


        public bool DoesPieceAtSquareAttackSquare(ushort attackerSquare, ushort attackedSquare,
            Piece attackerPiece)
        {
            var attackedSquares = Bitboard.GetAttackedSquares(attackerPiece, attackerSquare, TotalOccupancy);
            var attackedValue = attackedSquare.GetBoardValueOfIndex();
            return (attackedSquares & attackedValue) != 0;
        }


        #region MoveDetail- Finding the Source Square From SAN





        /// <summary>
        ///     Finds the source square index for a Pawn's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="pawnOccupancy">The pawn's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public static ushort FindPawnMoveSourceIndex(IBoard board, MoveDetail moveDetail)
        {
            ulong pawnOccupancy = board.GetPiecePlacement().Occupancy(board.ActivePlayer, Piece.Pawn);
            ulong totalOccupancy = board.TotalOccupancy();
            if (moveDetail.DestinationIndex == null)
                throw new ArgumentException("moveDetail.DestinationIndex cannot be null");
            if (moveDetail.DestinationRank == null)
                throw new ArgumentException("moveDetail.DestinationRank cannot be null");
            if (moveDetail.DestinationFile == null)
                throw new ArgumentException("moveDetail.DestinationFile cannot be null");
            var rank = moveDetail.Color == Color.Black
                ? moveDetail.DestinationRank.Value.RankCompliment()
                : moveDetail.DestinationRank.Value;
            var file = moveDetail.DestinationFile.Value;
            ushort sourceIndex = 0;
            var adjustedRelevantPieceOccupancy =
                moveDetail.Color == Color.Black ? BitHelpers.FlipVertically(pawnOccupancy) : pawnOccupancy;
            Debug.Assert(rank < 8);
            var supposedRank = (ushort)(rank - 1);
            if (rank == 3) // 2 possible source ranks, 2 & 3 (offsets 1 & 2)
            {
                //Check 3rd rank first, logically if a pawn is there that is the source
                if ((adjustedRelevantPieceOccupancy & BoardHelpers.RankMasks[2] & BoardHelpers.FileMasks[file]) != 0)
                    sourceIndex = (ushort)((8 * 2) + (file % 8));
                else if ((adjustedRelevantPieceOccupancy & BoardHelpers.RankMasks[1] & BoardHelpers.FileMasks[file]) != 0)
                    sourceIndex = (ushort)((1 * 8) + (file % 8));
            }
            else //else source square was destination + 8 (a move one rank ahead), but we need to make sure a pawn was there
            {
                var supposedIndex = BoardHelpers.RankAndFileToIndex(
                    moveDetail.Color == Color.Black ? supposedRank.RankCompliment() : supposedRank,
                    moveDetail.DestinationFile.Value);
                if (supposedRank == 0)
                    throw new MoveException(
                        $"{moveDetail.MoveText}: Cannot possibly be a pawn at the source square {supposedIndex.IndexToSquareDisplay()} implied by move.");
                sourceIndex = (ushort)((supposedRank * 8) + moveDetail.DestinationFile.Value);
            }

            var idx = moveDetail.Color == Color.Black ? sourceIndex.FlipIndexVertically() : sourceIndex;
            
            return idx;
        }



        public override object Clone()
        {
            return new BoardInfo(this.ToFEN());
        }


        #endregion

        #region SAN Moves

        #endregion

    }
}