using System;
using System.Linq;
using System.Text;
using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;

namespace MagicBitboard
{
    public class BoardInfo
    {
        public readonly bool Chess960 = false;
        public ushort? EnPassantIndex { get; private set; }
        public readonly MoveTree<MoveHashStorage> MoveTree = new MoveTree<MoveHashStorage>(null);

        #region General Board Information
        public ulong[][] PiecesOnBoard = new ulong[2][];
        public string FEN { get; }
        public uint HalfmoveClock { get; set; }
        public uint MoveCounter { get; set; }
        public CastlingAvailability CastlingAvailability { get; set; }
        #region Color Properties and related fields
        public Color ActivePlayer { get; set; }
        public Color OpponentColor => ActivePlayer == Color.Black ? Color.White : Color.Black;
        private int NActiveColor => (int)ActivePlayer;
        private int NOpponentColor => (int)OpponentColor;
        #endregion

        #endregion

        #region Constant Piece Values for Indexing arrays
        // ReSharper disable InconsistentNaming
        const int PAWN = (int)Piece.Pawn;
        const int BISHOP = (int)Piece.Bishop;
        const int KNIGHT = (int)Piece.Knight;
        const int ROOK = (int)Piece.Rook;
        const int QUEEN = (int)Piece.Queen;
        // ReSharper restore InconsistentNaming
        #endregion

        #region Occupancy / Index Properties for shorthand access to occupancy board arrays

        /// <summary>
        /// Occupancy of side-to-move's pawns
        /// </summary>
        public ulong ActivePawnOccupancy => PiecesOnBoard[NActiveColor][PAWN];

        /// <summary>
        /// Occupancy of side-to-move's Knights
        /// </summary>
        public ulong ActiveKnightOccupancy => PiecesOnBoard[NActiveColor][KNIGHT];

        /// <summary>
        /// Occupancy of side-to-move's Bishops
        /// </summary>
        public ulong ActiveBishopOccupancy => PiecesOnBoard[NActiveColor][BISHOP];

        /// <summary>
        /// Occupancy of side-to-move's Rooks
        /// </summary>
        public ulong ActiveRookOccupancy => PiecesOnBoard[NActiveColor][ROOK];

        /// <summary>
        /// Occupancy of side-to-move's Queen(s)
        /// </summary>
        public ulong ActiveQueenOccupancy => PiecesOnBoard[NActiveColor][QUEEN];

        /// <summary>
        /// Occupancy of side-to-move's pieces
        /// </summary>
        public ulong ActiveTotalOccupancy
        {
            get
            {
                ulong rv = 0;
                var activeOcc = PiecesOnBoard[NActiveColor];
                for (int i = 0; i < activeOcc.Length; i++) rv |= activeOcc[i];
                return rv;
            }
        }

        /// <summary>
        /// Occupancy of opponent's pieces
        /// </summary>
        public ulong OpponentTotalOccupancy
        {
            get
            {
                ulong rv = 0;
                for (int i = 0; i < PiecesOnBoard[NOpponentColor].Length; i++) rv |= PiecesOnBoard[NOpponentColor][i];
                return rv;
            }
        }

        /// <summary>
        /// Total board occupancy
        /// </summary>
        public ulong TotalOccupancy => ActiveTotalOccupancy | OpponentTotalOccupancy;

        /// <summary>
        /// Index of side-to-move's King
        /// </summary>
        public ushort ActivePlayerKingIndex => PiecesOnBoard[(int)ActivePlayer][Piece.King.ToInt()].GetSetBits()[0];

        /// <summary>
        /// Value (occupancy board) of side-to-move's King
        /// </summary>
        public ulong ActivePlayerKingOccupancy => (ulong)(0x01 << ActivePlayerKingIndex);

        /// <summary>
        /// Opponent's King's square index
        /// </summary>
        public ushort OpposingPlayerKingIndex => PiecesOnBoard[(int)ActivePlayer.Toggle()][Piece.King.ToInt()].GetSetBits()[0];

        #endregion

        private BoardInfo(bool chess960 = false)
        {
            Chess960 = chess960;
        }

        public BoardInfo(ulong[][] piecesOnBoard, Color activePlayer, CastlingAvailability castlingAvailability, ushort? enPassantIndex, uint halfmoveClock, uint moveCounter, bool chess960 = false)
        {
            PiecesOnBoard = piecesOnBoard;
            ActivePlayer = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassantIndex = enPassantIndex;
            HalfmoveClock = halfmoveClock;
            MoveCounter = moveCounter;
            Chess960 = chess960;

            ValidateFields();
        }

        /// <summary>
        /// Creates a BoardInfo object given FEN notation.
        /// </summary>
        /// <param name="fen">string of FEN notation.</param>
        /// <param name="chess960">[unused right now] Is this a chess960 position (possible non-standard starting position)</param>
        /// <returns>A BoardInfo object representing the board state after parsing the FEN</returns>
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
            ushort? enPassantSquareIndex = fenPieces[(int)FENPieces.EnPassantSquare].SquareTextToIndex();
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
                enPassantSquareIndex, halfmoveClock, fullMoveCount);
        }

        private void ValidateFields()
        {
            var errors = new StringBuilder();
            errors.AppendLine(ValidateNumberOfPiecesOnBoard());
            errors.AppendLine(ValidateEnPassantSquare());
            errors.AppendLine(ValidateCastlingRights());
            errors.AppendLine(ValidateChecks());
        }

        public void ApplyMove(string moveText)
        {
            MoveExt move = GenerateMoveFromText(moveText);
            ApplyMove(move);
        }

        public void ApplyMove(MoveExt move)
        {
            GetPiecesAtSourceAndDestination(move, out PieceOfColor? pocSource, out _);
            var resultBoard = ValidateAndGetResultingBoardFromMove(move);
            var isCapture = (OpponentTotalOccupancy & move.DestinationValue) != 0;
            var isPawnMove = (ActivePawnOccupancy & move.SourceValue) != 0;
            if (isCapture || isPawnMove) HalfmoveClock = 0;
            else HalfmoveClock++;
            if (!pocSource.HasValue) throw new MoveException("No piece found at source.", MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare, move, ActivePlayer);
            UnsetCastlingAvailability(move, pocSource.Value.Piece);
            SetEnPassantFlag(move, pocSource);
            if (ActivePlayer == Color.Black)
            {
                MoveCounter++;
                ActivePlayer = Color.White;
            }
            else { ActivePlayer = Color.Black; }
            PiecesOnBoard = resultBoard;
            MoveTree.AddLast(new MoveNode<MoveHashStorage>(new MoveHashStorage(move, ToFEN())));
        }

        /// <summary>
        /// Clears appropriate castling availability flag when <paramref name="movingPiece">piece moving</paramref> is a <see cref="Piece.Rook">Rook</see> or <see cref="Piece.King">King</see>
        /// </summary>
        /// <param name="move">Move object</param>
        /// <param name="movingPiece">Piece that is moving</param>
        public void UnsetCastlingAvailability(MoveExt move, Piece movingPiece)
        {
            switch (movingPiece)
            {
                case Piece.Rook:
                    if (move.SourceIndex == 56) CastlingAvailability &= ~CastlingAvailability.BlackQueenside;
                    if (move.SourceIndex == 63) CastlingAvailability &= ~CastlingAvailability.BlackKingside;
                    if (move.SourceIndex == 0) CastlingAvailability &= ~CastlingAvailability.WhiteQueenside;
                    if (move.SourceIndex == 7) CastlingAvailability &= ~CastlingAvailability.WhiteKingside;
                    break;
                case Piece.King:
                    if (move.SourceIndex == 60) CastlingAvailability &= ~(CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside);
                    if (move.SourceIndex == 4) CastlingAvailability &= ~(CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside);
                    break;
            }
        }

        /// <summary>
        /// Sets EnPassant flag appropriately, clearing it if no En Passant is available
        /// </summary>
        /// <param name="move"></param>
        /// <param name="pocSource"></param>
        public void SetEnPassantFlag(MoveExt move, PieceOfColor? pocSource)
        {
            if (pocSource.HasValue)
            {
                var startRank = pocSource.Value.Color == Color.White ? 1 : 6;
                var endRank = pocSource.Value.Color == Color.White ? 3 : 4;
                var enPassantIndexOffset = pocSource.Value.Color == Color.White ? 8 : -8;
                if (pocSource.Value.Piece == Piece.Pawn)
                {
                    if (((move.SourceValue & BoardHelpers.RankMasks[startRank]) != 0)
                        && ((move.DestinationValue & BoardHelpers.RankMasks[endRank]) != 0))
                    {
                        EnPassantIndex = (ushort)(move.SourceIndex + enPassantIndexOffset);
                        return;
                    }
                }
            }
            EnPassantIndex = null;
        }

        private void GetPiecesAtSourceAndDestination(MoveExt move, out PieceOfColor? pocSource, out PieceOfColor? pocDestination)
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

        public static ulong XRayRookAttacks(ulong occupancy, ulong blockers, ushort squareIndex)
        {
            var rookMovesFromSquare = PieceAttackPatternHelper.RookMoveMask[squareIndex];
            blockers &= rookMovesFromSquare;
            return rookMovesFromSquare ^ Bitboard.GetAttackedSquares(Piece.Rook, squareIndex, occupancy);
        }

        public static ulong XRayBishopAttacks(ulong occupancy, ulong blockers, ushort squareIndex)
        {
            var bishopMovesFromSquare = PieceAttackPatternHelper.BishopMoveMask[squareIndex];
            blockers &= bishopMovesFromSquare;
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
            ulong xRayBishopAttacks = XRayBishopAttacks(TotalOccupancy, ActiveTotalOccupancy, ActivePlayerKingIndex);
            ulong xRayRookAttacks = XRayRookAttacks(TotalOccupancy, ActiveTotalOccupancy, ActivePlayerKingIndex);
            ulong bishopPinnedPieces = (PiecesOnBoard[NOpponentColor][BISHOP] | PiecesOnBoard[NOpponentColor][QUEEN]) & xRayBishopAttacks;
            ulong rookPinnedPieces = (PiecesOnBoard[NOpponentColor][ROOK] | PiecesOnBoard[NOpponentColor][QUEEN]) & xRayRookAttacks;
            var pinners = rookPinnedPieces | bishopPinnedPieces;
            while (pinners != 0)
            {
                var square = BitHelpers.BitScanForward(pinners);
                var squaresBetween = BoardHelpers.InBetween(square, ActivePlayerKingIndex);
                var piecesBetween = squaresBetween & ActiveTotalOccupancy;
                if (piecesBetween.CountSetBits() == 1)
                {
                    pinned |= piecesBetween;
                }
                pinners &= pinners - 1;

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
            var moveExt = MoveHelpers.GenerateMove(md.SourceIndex.Value, md.DestinationIndex.Value, md.MoveType, md.PromotionPiece ?? 0);
            return moveExt;
        }

        #region MoveDetail- Finding the Source Square From SAN
        /// <summary>
        /// Find's a piece's source index, given some textual clues, such as piece type, color, and destination
        /// </summary>
        /// <param name="moveDetail">Details of move, gathered from text description (SAN)</param>
        /// <returns>The index from which the move was made.</returns>
        /// <exception cref="MoveException">Thrown when the source can't be determined, piece on square cannot be determined, more than one piece of type could reach destination, or piece cannot reach destination.</exception>
        private ushort FindPieceSourceIndex(MoveDetail moveDetail)
        {
            switch (moveDetail.Piece)
            {
                case Piece.Pawn:
                    if (moveDetail.IsCapture)
                    {
                        throw new MoveException("Could not determine source square for pawn capture.");
                    }
                    return FindPawnMoveSourceIndex(moveDetail, PiecesOnBoard[NActiveColor][PAWN], TotalOccupancy);

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
        /// Used by the Find[piece]MoveSourceIndex to find the source of a piece moving parsed from SAN text.
        /// </summary>
        /// <param name="destinationIndex">Index of destination</param>
        /// <param name="pieceMoveMask">The move mask for the piece</param>
        /// <param name="pieceOccupancy">The occupancy for the piece in question</param>
        /// <returns></returns>
        private static ushort? FindPieceMoveSourceIndex(ushort destinationIndex, ulong pieceMoveMask, ulong pieceOccupancy)
        {
            ulong sourceSquares = 0;
            if ((sourceSquares = pieceMoveMask & pieceOccupancy) == 0) return null;
            var indices = sourceSquares.GetSetBits();
            if (indices.Count() != 1) return ushort.MaxValue;
            return indices[0];
        }

        /// <summary>
        /// Finds the source square index for a King's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="kingOccupancy">The King's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public static ushort FindKingMoveSourceIndex(MoveDetail moveDetail, ulong kingOccupancy, ulong totalOccupancy)
        {
            var possibleSquares = Bitboard.GetAttackedSquares(Piece.King, moveDetail.DestinationIndex.Value, totalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail.DestinationIndex.Value, possibleSquares, kingOccupancy);
            if (!sourceSquare.HasValue) throw new MoveException("The King can possibly get to the specified destination.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///  Finds the source square index for a Queen's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="queenOccupancy">The Queen's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public static ushort FindQueenMoveSourceIndex(MoveDetail moveDetail, ulong queenOccupancy, ulong totalOccupancy)
        {
            var possibleSquares = Bitboard.GetAttackedSquares(Piece.Queen, moveDetail.DestinationIndex.Value, totalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail.DestinationIndex.Value, possibleSquares, queenOccupancy);
            if (!sourceSquare.HasValue) throw new MoveException("No Queen can possibly get to the specified destination.");
            if (sourceSquare == ushort.MaxValue) throw new MoveException("More than one Queen can get to the specified square.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///  Finds the source square index for a Rook's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="rookOccupancy">The Rook's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public static ushort FindRookMoveSourceIndex(MoveDetail moveDetail, ulong rookOccupancy, ulong totalOccupancy)
        {
            //var possibleSquares = PieceAttackPatternHelper.BishopMoveMask[md.DestRank.Value, md.DestFile.Value];
            var possibleSquares = Bitboard.GetAttackedSquares(Piece.Rook, moveDetail.DestinationIndex.Value, totalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail.DestinationIndex.Value, possibleSquares, rookOccupancy);
            if (!sourceSquare.HasValue) throw new MoveException("No Rook can possibly get to the specified destination.");
            if (sourceSquare == ushort.MaxValue) throw new MoveException("More than one Rook can get to the specified square.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///  Finds the source square index for a Bishop's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="bishopOccupancy">The Bishop's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public static ushort FindBishopMoveSourceIndex(MoveDetail moveDetail, ulong bishopOccupancy, ulong totalOccupancy)
        {
            var possibleSquares = Bitboard.GetAttackedSquares(Piece.Bishop, moveDetail.DestinationIndex.Value, totalOccupancy);
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail.DestinationIndex.Value, possibleSquares, bishopOccupancy);
            if (!sourceSquare.HasValue) throw new MoveException("No Bishop can possibly get to the specified destination.");
            if (sourceSquare == ushort.MaxValue) throw new MoveException("More than one Bishop can get to the specified square.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///  Finds the source square index for a Knight's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="relevantPieceOccupancy">The Knight's occupancy board</param>
        /// <returns></returns>
        public static ushort FindKnightMoveSourceIndex(MoveDetail moveDetail, ulong relevantPieceOccupancy)
        {
            var possibleSquares = PieceAttackPatternHelper.KnightAttackMask[moveDetail.DestinationIndex.Value];
            var sourceSquare = FindPieceMoveSourceIndex(moveDetail.DestinationIndex.Value, possibleSquares, relevantPieceOccupancy);
            if (!sourceSquare.HasValue) throw new MoveException("No Knight can possibly get to the specified destination.");
            if (sourceSquare == short.MaxValue) throw new MoveException("More than one Knight can get to the specified square.");
            return sourceSquare.Value;
        }

        /// <summary>
        ///  Finds the source square index for a Pawn's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="pawnOccupancy">The pawn's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public static ushort FindPawnMoveSourceIndex(MoveDetail moveDetail, ulong pawnOccupancy, ulong totalOccupancy)
        {
            var file = moveDetail.DestinationFile;
            var rank = moveDetail.Color == Color.Black ? moveDetail.DestinationRank.Value.RankCompliment() : moveDetail.DestinationRank;
            ushort sourceIndex = 0;
            var adjustedRelevantPieceOccupancy = moveDetail.Color == Color.Black ? pawnOccupancy.FlipVertically() : pawnOccupancy;
            ushort supposedRank = (ushort)(rank - 1);
            if (rank == 3) // 2 possible source ranks, 2 & 3 (offsets 1 & 2)
            {
                //Check 3rd rank first, logically if a pawn is there that is the source
                if ((adjustedRelevantPieceOccupancy & BoardHelpers.RankMasks[2]) != 0) sourceIndex = (ushort)((2 * 8) + (file % 8));
                if ((adjustedRelevantPieceOccupancy & BoardHelpers.RankMasks[1]) != 0) sourceIndex = (ushort)((1 * 8) + (file % 8));
            }
            else //else source square was destination + 8, but we need to make sure a pawn was there
            {
                var supposedIndex = BoardHelpers.RankAndFileToIndex(moveDetail.Color == Color.Black ? supposedRank.RankCompliment() : supposedRank, moveDetail.DestinationFile.Value);
                if (supposedRank == 0) { throw new MoveException($"{moveDetail.MoveText}: Cannot possibly be a pawn at the source square {supposedIndex.IndexToSquareDisplay()} implied by move."); }
                sourceIndex = (ushort)((supposedRank * 8) + moveDetail.DestinationFile.Value);
            }

            var idx = moveDetail.Color == Color.Black ? sourceIndex.FlipIndexVertically() : sourceIndex;
            ValidatePawnMove(moveDetail.Color, idx, moveDetail.DestinationIndex.Value, pawnOccupancy, totalOccupancy, moveDetail.MoveText);
            return idx;
        }

        /// <summary>
        ///  Validates a pawn move that has been parsed via SAN, after the source has been determined.
        /// </summary>
        /// <param name="pawnColor"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="pawnOccupancy">Active pawn occupancy board</param>
        /// <param name="opponentOccupancy">Opponent's occupancy board; used to validate captures</param>
        /// <param name="moveText">SAN that is used in the error messages only.</param>
        /// <exception cref="MoveException">Thrown if no pawn exists at source, pawn cannot move from source to destination (blocked, wrong square), destination is occupied, or if move is capture, but no opposing piece is there for capture.</exception>
        public static void ValidatePawnMove(Color pawnColor, ushort sourceIndex, ushort destinationIndex, ulong pawnOccupancy, ulong opponentOccupancy, string moveText = "")
        {
            moveText = !string.IsNullOrEmpty(moveText) ? moveText + ": " : "";
            var sourceValue = sourceIndex.IndexToValue();
            var isCapture = sourceIndex.FileFromIdx() != destinationIndex.FileFromIdx();
            var destValue = destinationIndex.IndexToValue();
            //validate pawn is at supposed source
            var pawnAtSource = sourceValue & pawnOccupancy;
            if (pawnAtSource == 0) throw new MoveException($"There is no pawn on {sourceIndex.IndexToSquareDisplay()} to move to {destinationIndex.IndexToSquareDisplay()}.");

            //validate pawn move to square is valid
            var pawnMoves = isCapture ? PieceAttackPatternHelper.PawnAttackMask[(int)pawnColor][sourceIndex] : PieceAttackPatternHelper.PawnMoveMask[(int)pawnColor][sourceIndex];
            if ((pawnMoves & destValue) == 0)
            {
                throw new MoveException($"{moveText}Pawn from {sourceIndex.IndexToSquareDisplay()} to {destinationIndex.IndexToSquareDisplay()} is illegal.");
            }

            var destinationOccupancy = (destValue & opponentOccupancy);
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

        /// <summary>
        /// Finds a piece belonging to side-to-move based on it's value (not index).
        /// </summary>
        /// <param name="pieceInSquareValue">Value of active/side-to-move's piece</param>
        /// <returns>The <see cref="Piece">piece occupying the <paramref name="pieceInSquareValue">square-value</paramref> parameter.</see>/></returns>
        public Piece GetActivePieceByValue(ulong pieceInSquareValue)
        {
            for (int p = 0; p <= (int)Piece.King; p++)
            {
                if ((PiecesOnBoard[NActiveColor][p] & pieceInSquareValue) != 0) return (Piece)p;
            }
            throw new MoveException("No piece found with the specified value.");
        }

        public ulong[][] ValidateAndGetResultingBoardFromMove(MoveExt move)
        {
            var pieceMoving = GetActivePieceByValue(move.SourceValue);
            var isCapture = (OpponentTotalOccupancy & move.DestinationValue) != 0;
            var resultantBoard = BoardPostMove(move, pieceMoving);
            Validate_PieceIsOfActiveColor(move);
            ValidateSourceIsNonVacant(move);
            ValidateDestinationIsNotOccupiedByActiveColor(move);
            var isKingInCheckAfterMove = IsAttackedBy(OpponentColor, ActivePlayerKingIndex, resultantBoard);
            if (isKingInCheckAfterMove)
            {
                throw new MoveException("Move leaves King in check.", MoveExceptionType.MoveLeavesKingInCheck, move, ActivePlayer);
            }
            switch (move.MoveType)
            {
                case MoveType.EnPassant:
                    if (move.DestinationIndex != EnPassantIndex)
                        throw new MoveException("En Passant not possible.", MoveExceptionType.EnPassantNotAvailalbe, move, ActivePlayer);
                    break;
                case MoveType.Promotion:
                    ValidatePromotion(ActivePlayer, move.SourceIndex, move.DestinationIndex);
                    break;
                case MoveType.Castle:
                    ValidateMove_Castle(move);
                    var castlingAvailabilityToRemove = ActivePlayer == Color.Black ? CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside
                        : CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside;
                    CastlingAvailability &= ~(castlingAvailabilityToRemove);
                    break;
            }
            return resultantBoard;
        }

        private ulong[][] BoardPostMove(MoveExt move, Piece pieceMoving)
        {

            var resultantBoard = new ulong[2][];

            for (int i = 0; i < 2; i++)
            {
                resultantBoard[i] = new ulong[6];
                foreach (var p in Enum.GetValues(typeof(Piece)))
                {
                    resultantBoard[i][(int)p] = PiecesOnBoard[i][(int)p];
                    resultantBoard[i][(int)p] = BitHelpers.ClearBit(resultantBoard[i][(int)p], move.SourceIndex);
                    resultantBoard[i][(int)p] = BitHelpers.ClearBit(resultantBoard[i][(int)p], move.DestinationIndex);
                    if (i == (int)ActivePlayer && (Piece)p == pieceMoving)
                    {
                        resultantBoard[i][(int)p] = resultantBoard[i][(int)p].SetBit(move.DestinationIndex);
                    }
                }
            }
            if (move.MoveType == MoveType.Castle)
            {
                resultantBoard[NActiveColor][ROOK] = GetRookBoardPostCastle(move, resultantBoard[NActiveColor][ROOK]);
            }
            else if (move.MoveType == MoveType.EnPassant)
            {
                var capturedPawnValue = 1ul << (OpponentColor == Color.Black ? move.DestinationIndex - 8 : move.DestinationIndex + 8);
                resultantBoard[NOpponentColor][PAWN] &= ~(capturedPawnValue);
            }
            else if (move.MoveType == MoveType.Promotion)
            {
                resultantBoard[NActiveColor][PAWN] &= ~(move.DestinationValue);
                switch (move.PromotionPiece)
                {
                    case PromotionPiece.Knight:
                        resultantBoard[NActiveColor][KNIGHT] |= move.DestinationValue;
                        break;
                    case PromotionPiece.Bishop:
                        resultantBoard[NActiveColor][BISHOP] |= move.DestinationValue;
                        break;
                    case PromotionPiece.Rook:
                        resultantBoard[NActiveColor][ROOK] |= move.DestinationValue;
                        break;
                    case PromotionPiece.Queen:
                        resultantBoard[NActiveColor][QUEEN] |= move.DestinationValue;
                        break;
                }
            }
            return resultantBoard;
        }

        private static ulong GetRookBoardPostCastle(MoveExt move, ulong rookBoard)
        {
            var rank = move.DestinationIndex.RankFromIdx();
            var file = move.DestinationIndex.FileFromIdx();
            var rookSource = rank == 7      // black castling
                ? file == 2
                    ? 0x100000000000000ul           // BLACK O-O-O
                        : 0x8000000000000000ul      // BLACK O-O
                    : file == 2
                    ? 0x01ul                        // WHITE O-O-O
                        : 0x80ul;                   // WHITE O-O

            var rookDest = rank == 7        // black castling
                ? file == 2
                    ? 0x800000000000000ul           // BLACK O-O-O
                        : 0x2000000000000000ul      // BLACK O-O
                    : file == 2
                    ? 0x08ul                        // WHITE O-O-O
                        : 0x20ul;                   // WHITE O-O

            return (rookBoard & ~(rookSource)) | rookDest;
        }

        public void Validate_PieceIsOfActiveColor(MoveExt move)
        {
            if ((ActiveTotalOccupancy & move.SourceValue) == 0)
            {
                throw new MoveException(
                    $"It is {ActivePlayer.ToString()}'s turn to move and that color has no piece on {move.SourceIndex.IndexToSquareDisplay()}.",
                    MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare,
                    move,
                    ActivePlayer);
            }
        }

        public void ValidateMove_Castle(MoveExt move)
        {
            if (AnySquaresInCheck(move))
            {
                throw new MoveException("Cannot Castle through check.", MoveExceptionType.Castle_ThroughCheck, move, ActivePlayer);
            }
            ValidateMove_CastleOccupancyBetween(move);
            ValidateMove_CastleAvailability(move);
        }

        /// <summary>
        /// Validates castling move with availability flags
        /// </summary>
        /// <param name="move"></param>
        public void ValidateMove_CastleAvailability(MoveExt move)
        {
            CastlingAvailability? castleChar;
            switch (move.DestinationIndex)
            {
                case 58:
                    castleChar = CastlingAvailability.BlackQueenside;
                    break;
                case 62:
                    castleChar = CastlingAvailability.BlackKingside;
                    break;
                case 2:
                    castleChar = CastlingAvailability.WhiteQueenside;
                    break;
                case 6:
                    castleChar = CastlingAvailability.WhiteKingside;
                    break;
                default:
                    throw new MoveException("Castling was applied to move when King's destination wasn't correctly set.", MoveExceptionType.Castle_BadDestinationSquare, move, ActivePlayer);
            }
            if (!CastlingAvailability.HasFlag(castleChar))
                throw new MoveException("Castling in the direction specified is not allowed.", MoveExceptionType.Castle_Unavailable, move, ActivePlayer);
        }

        /// <summary>
        /// Validates that no pieces are between castling King and Rook.
        /// </summary>
        /// <param name="move"></param>
        public void ValidateMove_CastleOccupancyBetween(MoveExt move)
        {
            ulong piecesBetween;
            switch (move.DestinationIndex)
            {
                case 58:
                    piecesBetween = BoardHelpers.InBetween(56, 60);
                    break;
                case 62:
                    piecesBetween = BoardHelpers.InBetween(60, 63);
                    break;
                case 2:
                    piecesBetween = BoardHelpers.InBetween(0, 4);
                    break;
                case 6:
                    piecesBetween = BoardHelpers.InBetween(4, 7);
                    break;
                default:
                    throw new MoveException("Castle's King destination is out of range.", MoveExceptionType.Castle_BadDestinationSquare, move, ActivePlayer);
            }
            if ((TotalOccupancy & piecesBetween) != 0)
            {
                throw new MoveException("Cannot castle with pieces between King and Rook.", MoveExceptionType.Castle_OccupancyBetween, move, ActivePlayer);
            }
        }

        public bool AnySquaresInCheck(MoveExt move)
        {
            var moveToAndFromValues = move.SourceValue | move.DestinationValue;
            var squaresBetween = BoardHelpers.InBetween(move.SourceIndex, move.DestinationIndex) | moveToAndFromValues;
            while (squaresBetween != 0)
            {
                var square = BitHelpers.BitScanForward(squaresBetween);
                if (IsAttackedBy(OpponentColor, square)) return true;
                squaresBetween &= squaresBetween - 1;
            }
            return false;
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

        public void ValidatePromotion(Color color, ushort moveSourceIdx, ushort moveDestIdx)
        {
            var moveSourceVal = 1ul << moveSourceIdx;
            var moveDestVal = 1ul << moveDestIdx;

            if ((PiecesOnBoard[NActiveColor][PAWN] & moveSourceVal) == 0)
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

        public static string ValidateEnPassantSquare(ulong[][] piecesOnBoard, ushort? enPassantSquare, Color activePlayer)
        {
            if (enPassantSquare == null) return "";
            if ((activePlayer == Color.White && (enPassantSquare < 40 || enPassantSquare > 47))
                ||
                (activePlayer == Color.Black && (enPassantSquare < 16 || enPassantSquare > 23)))
            {
                return "Bad En Passant Square detected.";
            }
            return "";
        }

        public static string ValidateCastlingRights(ulong[][] piecesOnBoard, CastlingAvailability castlingAvailability, bool chess960 = false)
        {
            if (castlingAvailability == CastlingAvailability.NoCastlingAvailable) return "";
            var message = new StringBuilder();
            var white = (int)Color.White;
            var black = (int)Color.Black;
            var rook = ROOK;
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
                message.AppendLine("White cannot castle without the King on e1.");
            }
            if (castlingAvailability.HasFlag(CastlingAvailability.BlackQueenside) || castlingAvailability.HasFlag(CastlingAvailability.BlackKingside)
                && !piecesOnBoard[black][king].IsBitSet(60))
            {
                message.AppendLine("Black cannot castle without the King on e1.");
            }
            return message.ToString();
        }


        private string ValidateNumberOfPiecesOnBoard() => ValidateNumberOfPiecesOnBoard(PiecesOnBoard);
        private string ValidateEnPassantSquare() => ValidateEnPassantSquare(PiecesOnBoard, EnPassantIndex, ActivePlayer);
        private string ValidateCastlingRights() => ValidateCastlingRights(PiecesOnBoard, CastlingAvailability, Chess960);

        /// <summary>
        /// Instance method to find if <paramref name="squareIndex"/> is attacked by a piece of <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color of possible attacker</param>
        /// <param name="squareIndex">Square which is possibly under attack</param>
        /// <returns>true if <paramref name="squareIndex"/> is attacked by <paramref name="color"/>. False if not.</returns>
        public bool IsAttackedBy(Color color, ushort squareIndex)
        {
            return IsAttackedBy(color, squareIndex, PiecesOnBoard);
        }

        /// <summary>
        /// Determines if piece on <paramref name="squareIndex"/> is attacked by <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color of attacker</param>
        /// <param name="squareIndex">Index of possible attack target</param>
        /// <param name="piecesOnBoard">Occupancy arrays for both colors, indexed as [color_enum][piece_enum]</param>
        /// <returns>true if <paramref name="squareIndex"/> is attacked by any piece of <paramref name="color"/></returns>
        public static bool IsAttackedBy(Color color, ushort squareIndex, ulong[][] piecesOnBoard)
        {

            var nColor = (int)color;
            var notNColor = nColor ^ 1;
            var r = squareIndex / 8;
            var f = squareIndex % 8;
            var totalOcc = 0ul;
            foreach (var cOcc in piecesOnBoard)
            {
                foreach (var pieceOcc in cOcc)
                    totalOcc |= pieceOcc;
            }
            var bishopAttack = Bitboard.GetAttackedSquares(Piece.Bishop, squareIndex, totalOcc);
            var rookAttack = Bitboard.GetAttackedSquares(Piece.Rook, squareIndex, totalOcc);
            if ((PieceAttackPatternHelper.PawnAttackMask[notNColor][squareIndex] & piecesOnBoard[nColor][Piece.Pawn.ToInt()]) != 0) return true;
            if ((PieceAttackPatternHelper.KnightAttackMask[r, f] & piecesOnBoard[nColor][Piece.Knight.ToInt()]) != 0) return true;
            if ((bishopAttack & (piecesOnBoard[nColor][Piece.Bishop.ToInt()] | piecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((rookAttack & (piecesOnBoard[nColor][Piece.Rook.ToInt()] | piecesOnBoard[nColor][Piece.Queen.ToInt()])) != 0) return true;
            if ((PieceAttackPatternHelper.KingMoveMask[r, f] & piecesOnBoard[nColor][Piece.King.ToInt()]) != 0) return true;
            return false;
        }

        #region FEN String Retrieval

        public string GetPiecePlacement()
        {
            char[] pieceSection = new char[64];
            for (int iColor = 0; iColor < 2; iColor++)
            {
                for (int iPiece = 0; iPiece < 6; iPiece++)
                {
                    var pieceArray = PiecesOnBoard[iColor][iPiece];
                    var charRepForPieceOfColor = PieceHelpers.GetCharRepresentation((Color)iColor, (Piece)iPiece);
                    while (pieceArray != 0)
                    {
                        var squareIndex = BitHelpers.BitScanForward(pieceArray);
                        var fenIndex = FENHelpers.BoardIndexToFENIndex(squareIndex);
                        pieceSection[fenIndex] = charRepForPieceOfColor;
                        pieceArray &= pieceArray - 1;
                    }

                }
            }
            var sb = new StringBuilder();
            for (int rank = 0; rank < 8; rank++) //start at FEN Rank of zero -> 7
            {
                var emptyCount = 0;
                for (int file = 0; file < 8; file++)
                {
                    var paChar = pieceSection[(rank * 8) + file];
                    if (paChar == 0) emptyCount++;
                    else
                    {
                        if (emptyCount != 0)
                        {
                            sb.Append(emptyCount.ToString());
                            emptyCount = 0;
                        }
                        sb.Append(paChar);
                    }

                }
                if (emptyCount != 0)
                {
                    sb.Append(emptyCount);
                }
                if (rank != 7) sb.Append('/');
            }
            return sb.ToString();
        }
        public string GetSideToMoveStrRepresentation() => ActivePlayer == Color.Black ? "b" : "w";

        public string GetCastlingAvailabilityString() => FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability);

        public string GetEnPassantString() => EnPassantIndex == null ? "-" : EnPassantIndex.Value.IndexToSquareDisplay();

        public string GetHalfMoveClockString() => HalfmoveClock.ToString();

        public string GetMoveCounterString() => MoveCounter.ToString();

        public string ToFEN() => $"{GetPiecePlacement()} {GetSideToMoveStrRepresentation()} {GetCastlingAvailabilityString()} {GetEnPassantString()} {GetHalfMoveClockString()} {GetMoveCounterString()}";
        #endregion
    }
}

