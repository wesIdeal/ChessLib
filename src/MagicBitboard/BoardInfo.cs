using ChessLib.Data;
using ChessLib.Data.Boards;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using ChessLib.Validators.MoveValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChessLib.MagicBitboard
{
    public class BoardInfo : BoardInformationService<MoveHashStorage>
    {
        //public readonly MoveTree<MoveHashStorage> MoveTree = new MoveTree<MoveHashStorage>(null);
        public override MoveTree<MoveHashStorage> MoveTree { get; set; }
        public BoardInfo() : base(FENHelpers.FENInitial, false) { }
        public BoardInfo(string fen, bool is960 = false) : base(fen, is960)
        {
            MoveTree = new MoveTree<MoveHashStorage>(null);
        }

        public override void ApplyMove(string moveText)
        {
            var move = GenerateMoveFromText(moveText);
            ApplyMove(move);
        }

        public MoveExceptionType? ApplyMove(MoveExt move)
        {
            GetPiecesAtSourceAndDestination(move, out var pocSource, out var pocDestination);
            if (pocSource == null) throw new ArgumentException("No piece at source.");
            var moveValidator = new MoveValidator((BoardFENInfo)this, move);
            var validationError = moveValidator.Validate();
            if (validationError.HasValue)
                throw new MoveException("Error with move.", validationError.Value, move, ActivePlayer);

            var san = move.MoveToSAN(this, MoveTree.ParentMove == null);
            MoveTree.AddLast(new MoveNode<MoveHashStorage>(new MoveHashStorage(move, pocSource.Value.Piece, ActivePlayer, ToFEN(), san)));
            ApplyValidatedMove(move);
            return null;
        }

        private void ApplyValidatedMove(MoveExt move)
        {
            var newBoard = this.ApplyMoveToBoard(move);
            this.PiecePlacement = newBoard.PiecePlacement;
            this.ActivePlayer = newBoard.ActivePlayer;
            this.CastlingAvailability = newBoard.CastlingAvailability;
            this.EnPassantSquare = newBoard.EnPassantSquare;
            this.HalfmoveClock = newBoard.HalfmoveClock;
            this.FullmoveCounter = newBoard.FullmoveCounter;
        }

        protected bool OpposingPlayerInCheck => IsAttackedBy(ActivePlayer, OpposingPlayerKingIndex);
        protected bool ActivePlayerInCheck => IsAttackedBy(OpponentColor, ActivePlayerKingIndex);
        protected override Check GetChecks(Color activePlayer)
        {
            var rv = Check.None;

            if (ActivePlayerInCheck && OpposingPlayerInCheck)
            {
                rv &= ~Check.None;
                rv |= Check.Double;
            }
            else if (OpposingPlayerInCheck)
            {
                rv &= ~Check.None;
                rv |= Check.Opposite;
            }
            else if (ActivePlayerInCheck)
            {
                rv &= ~Check.None;
                rv |= Check.Normal;
            }

            return rv;
        }

        private void GetPiecesAtSourceAndDestination(MoveExt move, out PieceOfColor? pocSource,
            out PieceOfColor? pocDestination)
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
                    if ((PiecePlacement[WHITE][p] & sVal) != 0)
                        pocSource = new PieceOfColor { Color = Color.White, Piece = piece };
                    if ((PiecePlacement[BLACK][p] & sVal) != 0)
                        pocSource = new PieceOfColor { Color = Color.Black, Piece = piece };
                }
                if (pocDestination == null)
                {
                    if ((PiecePlacement[WHITE][p] & dVal) != 0)
                        pocDestination = new PieceOfColor { Color = Color.White, Piece = piece };
                    if ((PiecePlacement[BLACK][p] & dVal) != 0)
                        pocDestination = new PieceOfColor { Color = Color.Black, Piece = piece };
                }
            }
        }



        public static ulong XRayRookAttacks(ulong occupancy, ulong blockers, ushort squareIndex)
        {
            var rookMovesFromSquare = PieceAttackPatternHelper.RookMoveMask[squareIndex];
            //blockers &= rookMovesFromSquare;
            return rookMovesFromSquare ^ Bitboard.GetAttackedSquares(Piece.Rook, squareIndex, occupancy);
        }

        public static ulong XRayBishopAttacks(ulong occupancy, ulong blockers, ushort squareIndex)
        {
            var bishopMovesFromSquare = PieceAttackPatternHelper.BishopMoveMask[squareIndex];
            //blockers &= bishopMovesFromSquare;
            return bishopMovesFromSquare ^ Bitboard.GetAttackedSquares(Piece.Bishop, squareIndex, occupancy);
        }

        public bool IsPiecePinned(ulong pieceValue)
        {
            return (GetPinnedPieces() & pieceValue) != 0;
        }

        public bool IsPiecePinned(ushort indexOfSquare)
        {
            return IsPiecePinned(indexOfSquare.IndexToValue());
        }

        public ulong GetPinnedPieces()
        {
            ulong pinned = 0;
            var xRayBishopAttacks = XRayBishopAttacks(TotalOccupancy, PiecePlacement.Occupancy(ActivePlayer), ActivePlayerKingIndex);
            var xRayRookAttacks = XRayRookAttacks(TotalOccupancy, PiecePlacement.Occupancy(ActivePlayer), ActivePlayerKingIndex);
            var bishopPinnedPieces = (PiecePlacement[NOpponentColor][BISHOP] | PiecePlacement[NOpponentColor][QUEEN]) &
                                     xRayBishopAttacks;
            var rookPinnedPieces = (PiecePlacement[NOpponentColor][ROOK] | PiecePlacement[NOpponentColor][QUEEN]) &
                                   xRayRookAttacks;
            var allPins = rookPinnedPieces | bishopPinnedPieces;
            while (allPins != 0)
            {
                var square = BitHelpers.BitScanForward(allPins);
                var squaresBetween = BoardHelpers.InBetween(square, ActivePlayerKingIndex);
                var piecesBetween = squaresBetween & PiecePlacement.Occupancy(ActivePlayer);
                if (piecesBetween.CountSetBits() == 1) pinned |= piecesBetween;
                allPins &= allPins - 1;
            }

            return pinned;
        }

        public MoveExt GenerateMoveFromText(string moveText)
        {
            var md = MoveHelpers.GetAvailableMoveDetails(moveText, ActivePlayer);
            if (!md.SourceFile.HasValue || !md.SourceRank.HasValue)
            {
                var sourceIndex = FindPieceSourceIndex(md);
                md.SourceIndex = sourceIndex;
            }

            Debug.Assert(md.SourceIndex != null, "md.SourceIndex != null");
            Debug.Assert(md.DestinationIndex != null, "md.DestinationIndex != null");
            if (md.IsCapture && md.Piece == Piece.Pawn &&
                (OpponentOccupancy & (1ul << md.DestinationIndex)) == 0 &&
                (md.DestinationRank == 2 || md.DestinationRank == 5)) md.MoveType = MoveType.EnPassant;
            var moveExt = MoveHelpers.GenerateMove(md.SourceIndex.Value, md.DestinationIndex.Value, md.MoveType,
                md.PromotionPiece ?? 0);
            return moveExt;
        }


        public override ulong GetAttackedSquares(Piece p, ushort index, ulong occupancy)
        {
            return Bitboard.GetAttackedSquares(p, index, occupancy);
        }

       

       /// <summary>
        ///     Instance method to find if <paramref name="squareIndex" /> is attacked by a piece of <paramref name="color" />
        /// </summary>
        /// <param name="color">Color of possible attacker</param>
        /// <param name="squareIndex">Square which is possibly under attack</param>
        /// <returns>true if <paramref name="squareIndex" /> is attacked by <paramref name="color" />. False if not.</returns>
        public override bool IsAttackedBy(Color color, ushort squareIndex)
        {
            return Bitboard.IsSquareAttackedByColor(squareIndex, color, PiecePlacement);
        }

        public override bool DoesPieceAtSquareAttackSquare(ushort attackerSquare, ushort attackedSquare,
            Piece attackerPiece)
        {
            var attackedSquares = GetAttackedSquares(attackerPiece, attackerSquare, TotalOccupancy);
            var attackedValue = attackedSquare.GetBoardValueOfIndex();
            return (attackedSquares & attackedValue) != 0;
        }

        #region MoveDetail- Finding the Source Square From SAN

        /// <summary>
        ///     Find's a piece's source index, given some textual clues, such as piece type, color, and destination
        /// </summary>
        /// <param name="moveDetail">Details of move, gathered from text description (SAN)</param>
        /// <returns>The index from which the move was made.</returns>
        /// <exception cref="MoveException">
        ///     Thrown when the source can't be determined, piece on square cannot be determined, more
        ///     than one piece of type could reach destination, or piece cannot reach destination.
        /// </exception>
        private ushort FindPieceSourceIndex(MoveDetail moveDetail)
        {
            switch (moveDetail.Piece)
            {
                case Piece.Pawn:
                    if (moveDetail.IsCapture)
                        throw new MoveException("Could not determine source square for pawn capture.");
                    return FindPawnMoveSourceIndex(moveDetail, PiecePlacement[NActiveColor][PAWN], TotalOccupancy);

                case Piece.Knight:
                    return FindKnightMoveSourceIndex(moveDetail, ActiveKnightOccupancy);
                case Piece.Bishop:
                    return FindBishopMoveSourceIndex(moveDetail, ActiveBishopOccupancy, TotalOccupancy);
                case Piece.Rook:
                    return FindRookMoveSourceIndex(moveDetail, ActiveRookOccupancy, TotalOccupancy);
                case Piece.Queen:
                    return FindQueenMoveSourceIndex(moveDetail, ActiveQueenOccupancy, TotalOccupancy);
                case Piece.King:
                    return FindKingMoveSourceIndex(moveDetail, ActivePlayerKingOccupancy, TotalOccupancy);
                default: throw new MoveException("Invalid piece specified for move.");
            }
        }

        /// <summary>
        ///     Used by the Find[piece]MoveSourceIndex to find the source of a piece moving parsed from SAN text.
        /// </summary>
        /// <param name="md">Available Move details</param>
        /// <param name="pieceMoveMask">The move mask for the piece</param>
        /// <param name="pieceOccupancy">The occupancy for the piece in question</param>
        /// <returns></returns>
        private ushort? FindPieceMoveSourceIndex(MoveDetail md, ulong pieceMoveMask, ulong pieceOccupancy)
        {
            ulong sourceSquares;
            if ((sourceSquares = pieceMoveMask & pieceOccupancy) == 0) return null;

            if (md.SourceFile != null)
                sourceSquares &= BoardHelpers.FileMasks[md.SourceFile.Value];
            if (md.SourceRank != null)
                sourceSquares &= BoardHelpers.RankMasks[md.SourceRank.Value];
            var indices = sourceSquares.GetSetBits();

            if (indices.Length == 0) return null;
            if (indices.Length > 1)
            {
                var possibleSources = new List<ushort>();

                foreach (var sourceIndex in indices)
                {
                    if (!md.DestinationIndex.HasValue) throw new MoveException("No destination value provided.");
                    var proposedMove = MoveHelpers.GenerateMove(sourceIndex, md.DestinationIndex.Value, md.MoveType, md.PromotionPiece ?? PromotionPiece.Knight);
                    var moveValidator = new MoveValidator(this, proposedMove);
                    var validationResult = moveValidator.Validate();
                    if (validationResult == null)
                        possibleSources.Add(sourceIndex);
                }
                if (possibleSources.Count > 1) throw new MoveException("More than one piece can reach destination square.");
                if (possibleSources.Count == 0) return null;
                return possibleSources[0];
            }
            return indices[0];
        }

        /// <summary>
        ///     Finds the source square index for a King's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="kingOccupancy">The King's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public ushort FindKingMoveSourceIndex(MoveDetail moveDetail, ulong kingOccupancy, ulong totalOccupancy)
        {
            Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex != null");
            var possibleSquares =
                Bitboard.GetAttackedSquares(Piece.King, moveDetail.DestinationIndex.Value, totalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, kingOccupancy);
            if (!sourceSquare.HasValue)
                throw new MoveException("The King can possibly get to the specified destination.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///     Finds the source square index for a Queen's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="queenOccupancy">The Queen's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public ushort FindQueenMoveSourceIndex(MoveDetail moveDetail, ulong queenOccupancy, ulong totalOccupancy)
        {
            Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex != null");
            var possibleSquares =
                Bitboard.GetAttackedSquares(Piece.Queen, moveDetail.DestinationIndex.Value, totalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, queenOccupancy);
            if (!sourceSquare.HasValue)
                throw new MoveException("No Queen can possibly get to the specified destination.");
            if (sourceSquare == ushort.MaxValue)
                throw new MoveException("More than one Queen can get to the specified square.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///     Finds the source square index for a Rook's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="rookOccupancy">The Rook's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public ushort FindRookMoveSourceIndex(MoveDetail moveDetail, ulong rookOccupancy, ulong totalOccupancy)
        {
            //var possibleSquares = PieceAttackPatternHelper.BishopMoveMask[md.DestRank.Value, md.DestFile.Value];
            Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex != null");
            var possibleSquares =
                Bitboard.GetAttackedSquares(Piece.Rook, moveDetail.DestinationIndex.Value, totalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, rookOccupancy);
            if (!sourceSquare.HasValue)
                throw new MoveException("No Rook can possibly get to the specified destination.");
            if (sourceSquare == ushort.MaxValue)
                throw new MoveException("More than one Rook can get to the specified square.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///     Finds the source square index for a Bishop's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="bishopOccupancy">The Bishop's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public ushort FindBishopMoveSourceIndex(MoveDetail moveDetail, ulong bishopOccupancy,
            ulong totalOccupancy)
        {
            Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex != null");
            var possibleSquares =
                Bitboard.GetAttackedSquares(Piece.Bishop, moveDetail.DestinationIndex.Value, totalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, bishopOccupancy);
            if (!sourceSquare.HasValue)
                throw new MoveException("No Bishop can possibly get to the specified destination.");
            if (sourceSquare == ushort.MaxValue)
                throw new MoveException("More than one Bishop can get to the specified square.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///     Finds the source square index for a Knight's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="relevantPieceOccupancy">The Knight's occupancy board</param>
        /// <returns></returns>
        public ushort FindKnightMoveSourceIndex(MoveDetail moveDetail, ulong relevantPieceOccupancy)
        {
            Debug.Assert(moveDetail.DestinationIndex != null, "moveDetail.DestinationIndex != null");
            var possibleSquares = PieceAttackPatternHelper.KnightAttackMask[moveDetail.DestinationIndex.Value];
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail, possibleSquares, relevantPieceOccupancy);
            if (!sourceSquare.HasValue)
                throw new MoveException("No Knight can possibly get to the specified destination.");
            if (sourceSquare == short.MaxValue)
                throw new MoveException("More than one Knight can get to the specified square.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///     Finds the source square index for a Pawn's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="pawnOccupancy">The pawn's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public ushort FindPawnMoveSourceIndex(MoveDetail moveDetail, ulong pawnOccupancy, ulong totalOccupancy)
        {
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
                moveDetail.Color == Color.Black ? pawnOccupancy.FlipVertically() : pawnOccupancy;
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

        #endregion

        #region SAN Moves
        
        #endregion

    }
}