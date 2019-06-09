using ChessLib.Data.Boards;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.BoardValidators;
using ChessLib.Validators.MoveValidation;
using System;
using System.Diagnostics;

namespace ChessLib.Data
{
    public class BoardInfo : BoardBase
    {
        public IMoveTree<MoveHashStorage> MoveTree { get; set; }

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
            MoveTree = new MoveTree<MoveHashStorage>(null);
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
            var san = move.MoveToSAN(this, MoveTree.ParentMove == null);
            MoveTree.Add(new MoveNode<MoveHashStorage>(new MoveHashStorage(this.ToFEN(), move, pocSource.Value.Piece, ActivePlayer, san)));
            ApplyValidatedMove(move);
            return null;
        }

        public void ApplyValidatedMove(MoveExt move)
        {
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




        public bool DoesPieceAtSquareAttackSquare(ushort attackerSquare, ushort attackedSquare,
            Piece attackerPiece)
        {
            var attackedSquares = Bitboard.GetAttackedSquares(attackerPiece, attackerSquare, TotalOccupancy);
            var attackedValue = attackedSquare.GetBoardValueOfIndex();
            return (attackedSquares & attackedValue) != 0;
        }


        #region MoveDetail- Finding the Source Square From SAN





        ///// <summary>
        /////     Finds the source square index for a King's move
        ///// </summary>
        ///// <param name="moveDetail">Move details, gathered from text input</param>
        ///// <param name="kingOccupancy">The King's occupancy board</param>
        ///// <param name="totalOccupancy">The board's occupancy</param>
        ///// <returns></returns>
        //public ushort FindKingMoveSourceIndex(MoveDetail moveDetail, ulong kingOccupancy, ulong totalOccupancy)
        //{
        //    Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex cannot equal null");
        //    var possibleSquares =
        //        Bitboard.GetAttackedSquares(Piece.King, moveDetail.DestinationIndex.Value, totalOccupancy);
        //    var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, kingOccupancy);
        //    if (!sourceSquare.HasValue)
        //        throw new MoveException("The King can possibly get to the specified destination.");
        //    return sourceSquare.Value;
        //}

        ///// <summary>
        /////     Finds the source square index for a Queen's move
        ///// </summary>
        ///// <param name="moveDetail">Move details, gathered from text input</param>
        ///// <param name="queenOccupancy">The Queen's occupancy board</param>
        ///// <param name="totalOccupancy">The board's occupancy</param>
        ///// <returns></returns>
        //public ushort FindQueenMoveSourceIndex(MoveDetail moveDetail, ulong queenOccupancy, ulong totalOccupancy)
        //{
        //    Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex != null");
        //    var possibleSquares =
        //        Bitboard.GetAttackedSquares(Piece.Queen, moveDetail.DestinationIndex.Value, totalOccupancy);
        //    var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, queenOccupancy);
        //    if (!sourceSquare.HasValue)
        //        throw new MoveException("No Queen can possibly get to the specified destination.");
        //    if (sourceSquare == ushort.MaxValue)
        //        throw new MoveException("More than one Queen can get to the specified square.");
        //    return sourceSquare.Value;
        //}

        ///// <summary>
        /////     Finds the source square index for a Rook's move
        ///// </summary>
        ///// <param name="moveDetail">Move details, gathered from text input</param>
        ///// <param name="rookOccupancy">The Rook's occupancy board</param>
        ///// <param name="totalOccupancy">The board's occupancy</param>
        ///// <returns></returns>
        //public ushort FindRookMoveSourceIndex(MoveDetail moveDetail, ulong rookOccupancy, ulong totalOccupancy)
        //{
        //    //var possibleSquares = PieceAttackPatternHelper.BishopMoveMask[md.DestRank.Value, md.DestFile.Value];
        //    Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex != null");
        //    var possibleSquares =
        //        Bitboard.GetAttackedSquares(Piece.Rook, moveDetail.DestinationIndex.Value, totalOccupancy);
        //    var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, rookOccupancy);
        //    if (!sourceSquare.HasValue)
        //        throw new MoveException("No Rook can possibly get to the specified destination.");
        //    if (sourceSquare == ushort.MaxValue)
        //        throw new MoveException("More than one Rook can get to the specified square.");
        //    return sourceSquare.Value;
        //}

        ///// <summary>
        /////     Finds the source square index for a Bishop's move
        ///// </summary>
        ///// <param name="moveDetail">Move details, gathered from text input</param>
        ///// <param name="bishopOccupancy">The Bishop's occupancy board</param>
        ///// <param name="totalOccupancy">The board's occupancy</param>
        ///// <returns></returns>
        //public ushort FindBishopMoveSourceIndex(MoveDetail moveDetail, ulong bishopOccupancy,
        //    ulong totalOccupancy)
        //{
        //    Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex != null");
        //    var possibleSquares =
        //        Bitboard.GetAttackedSquares(Piece.Bishop, moveDetail.DestinationIndex.Value, totalOccupancy);
        //    var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, bishopOccupancy);
        //    if (!sourceSquare.HasValue)
        //        throw new MoveException("No Bishop can possibly get to the specified destination.");
        //    if (sourceSquare == ushort.MaxValue)
        //        throw new MoveException("More than one Bishop can get to the specified square.");
        //    return sourceSquare.Value;
        //}

        ///// <summary>
        /////     Finds the source square index for a Knight's move
        ///// </summary>
        ///// <param name="moveDetail">Move details, gathered from text input</param>
        ///// <param name="knightOccupancy">The Knight's occupancy board</param>
        ///// <returns></returns>
        //public static ushort FindKnightMoveSourceIndex(IBoard board, MoveDetail moveDetail)
        //{
        //    ulong knightOccupancy = board.GetPiecePlacement.Occupancy(board.ActivePlayer, Piece.Knight);
        //    Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex != null");
        //    var possibleSquares = PieceAttackPatternHelper.KnightAttackMask[moveDetail.DestinationIndex.Value];
        //    var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, knightOccupancy);
        //    if (!sourceSquare.HasValue)
        //        throw new MoveException("No Knight can possibly get to the specified destination.");
        //    if (sourceSquare == short.MaxValue)
        //        throw new MoveException("More than one Knight can get to the specified square.");
        //    return sourceSquare.Value;
        //}

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
            ValidatePawnMove(moveDetail.Color, idx, moveDetail.DestinationIndex.Value, pawnOccupancy, totalOccupancy,
                moveDetail.MoveText);
            return idx;
        }

        /// <summary>
        ///     Validates a pawn move that has been parsed via SAN, after the source has been determined.
        /// </summary>
        /// <param name="pawnColor"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="pawnOccupancy">Active pawn occupancy board</param>
        /// <param name="opponentOccupancy">Opponent's occupancy board; used to validate captures</param>
        /// <param name="moveText">SAN that is used in the error messages only.</param>
        /// <exception cref="MoveException">
        ///     Thrown if no pawn exists at source, pawn cannot move from source to destination
        ///     (blocked, wrong square), destination is occupied, or if move is capture, but no opposing piece is there for
        ///     capture.
        /// </exception>
        public static void ValidatePawnMove(Color pawnColor, ushort sourceIndex, ushort destinationIndex,
            ulong pawnOccupancy, ulong opponentOccupancy, string moveText = "")
        {
            moveText = !string.IsNullOrEmpty(moveText) ? moveText + ": " : "";
            var sourceValue = sourceIndex.IndexToValue();
            var isCapture = sourceIndex.FileFromIdx() != destinationIndex.FileFromIdx();
            var destValue = destinationIndex.IndexToValue();
            //validate pawn is at supposed source
            var pawnAtSource = sourceValue & pawnOccupancy;
            if (pawnAtSource == 0)
                throw new MoveException(
                    $"There is no pawn on {sourceIndex.IndexToSquareDisplay()} to move to {destinationIndex.IndexToSquareDisplay()}.");

            //validate pawn move to square is valid
            var pawnMoves = isCapture
                ? PieceAttackPatternHelper.PawnAttackMask[(int)pawnColor][sourceIndex]
                : PieceAttackPatternHelper.PawnMoveMask[(int)pawnColor][sourceIndex];
            if ((pawnMoves & destValue) == 0)
                throw new MoveException(
                    $"{moveText}Pawn from {sourceIndex.IndexToSquareDisplay()} to {destinationIndex.IndexToSquareDisplay()} is illegal.");

            var destinationOccupancy = destValue & opponentOccupancy;
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

        public override object Clone()
        {
            return new BoardInfo(this.ToFEN());
        }


        #endregion

        #region SAN Moves

        #endregion

    }
}