using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using ChessLib.MagicBitboard.MoveValidation;
using MagicBitboard;

namespace ChessLib.MagicBitboard
{
    public class BoardInfo
    {
        public readonly bool Chess960;
        public readonly MoveTree<MoveHashStorage> MoveTree = new MoveTree<MoveHashStorage>(null);


        //public BoardInfo(ulong[][] piecesOnBoard, Color activePlayerColor, CastlingAvailability castlingAvailability, ushort? enPassantIndex, uint halfmoveClock, uint moveCounter, bool chess960 = false)
        //{
        //    PiecesOnBoard = piecesOnBoard;
        //    ActivePlayerColor = activePlayerColor;
        //    CastlingAvailability = castlingAvailability;
        //    EnPassantIndex = enPassantIndex;
        //    HalfmoveClock = halfmoveClock;
        //    MoveCounter = moveCounter;
        //    Chess960 = chess960;

        //    ValidateFields();
        //}
        public BoardInfo() : this(FENHelpers.FENInitial)
        {
        }

        public BoardInfo(string fen, bool chess960 = false)
        {
            FENHelpers.ValidateFENStructure(fen);

            var fenPieces = fen.Split(' ');
            var piecePlacement = fenPieces[(int)FENPieces.PiecePlacement];
            FENHelpers.ValidateFENString(fen);
            var pieces = new ulong[2][];
            pieces[(int)Color.White] = new ulong[6];
            pieces[(int)Color.Black] = new ulong[6];
            var ranks = piecePlacement.Split('/').Reverse();

            var activePlayer = FENHelpers.GetActiveColor(fenPieces[(int)FENPieces.ActiveColor]);
            var enPassantSquareIndex = fenPieces[(int)FENPieces.EnPassantSquare].SquareTextToIndex();
            var halfmoveClock = FENHelpers.GetMoveNumberFromString(fenPieces[(int)FENPieces.HalfmoveClock]);
            var fullMoveCount = FENHelpers.GetMoveNumberFromString(fenPieces[(int)FENPieces.FullMoveCounter]);
            uint pieceIndex = 0;

            foreach (var rank in ranks)
                foreach (var f in rank)
                    switch (char.IsDigit(f))
                    {
                        case true:
                            var emptySquares = uint.Parse(f.ToString());
                            pieceIndex += emptySquares;
                            break;
                        case false:
                            var pieceOfColor = PieceHelpers.GetPieceOfColor(f);
                            pieces[(int)pieceOfColor.Color][(int)pieceOfColor.Piece] |= 1ul << (int)pieceIndex;
                            pieceIndex++;
                            break;
                    }
            PiecesOnBoard = pieces;
            ActivePlayerColor = activePlayer;
            CastlingAvailability = FENHelpers.GetCastlingFromString(fen.GetFENPiece(FENPieces.CastlingAvailability));
            EnPassantIndex = enPassantSquareIndex;
            HalfmoveClock = halfmoveClock;
            MoveCounter = fullMoveCount;
            Chess960 = chess960;
            FEN = fen;
            MoveTree.FENStart = fen;
            ValidateFields();
        }

        public ushort? EnPassantIndex { get; private set; }

        /// <summary>
        ///     Creates a BoardInfo object given FEN notation.
        /// </summary>
        /// <param name="fen">string of FEN notation.</param>
        /// <param name="chess960">[unused right now] Is this a chess960 position (possible non-standard starting position)</param>
        /// <returns>A BoardInfo object representing the board state after parsing the FEN</returns>
        public static BoardInfo BoardInfoFromFen(string fen, bool chess960 = false)
        {
            return new BoardInfo(fen, chess960);
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
            var move = GenerateMoveFromText(moveText);

            ApplyMove(move);
        }

        public MoveExceptionType? ApplyMove(MoveExt move)
        {
            GetPiecesAtSourceAndDestination(move, out var pocSource, out _);
            if (pocSource == null) throw new ArgumentException("No piece at source.");
            var moveValidator = new MoveValidator(this, move);
            var validationError = moveValidator.Validate();
            if (validationError.HasValue)
                throw new MoveException("Error with move.", validationError.Value, move, ActivePlayerColor);
            var isCapture = (OpponentTotalOccupancy & move.DestinationValue) != 0;
            var isPawnMove = (ActivePawnOccupancy & move.SourceValue) != 0;
            if (isCapture || isPawnMove) HalfmoveClock = 0;
            else HalfmoveClock++;

            UnsetCastlingAvailability(move, pocSource.Value.Piece);
            SetEnPassantFlag(move, pocSource);


            PiecesOnBoard = moveValidator.PostMoveBoard;
            MoveTree.AddLast(new MoveNode<MoveHashStorage>(new MoveHashStorage(move, pocSource.Value.Piece, ActivePlayerColor, ToFEN())));

            if (ActivePlayerColor == Color.Black)
            {
                MoveCounter++;
                ActivePlayerColor = Color.White;
            }
            else
            {
                ActivePlayerColor = Color.Black;
            }
            return null;
        }

        /// <summary>
        ///     Clears appropriate castling availability flag when <paramref name="movingPiece">piece moving</paramref> is a
        ///     <see cref="Piece.Rook">Rook</see> or <see cref="Piece.King">King</see>
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
                    if (move.SourceIndex == 60)
                        CastlingAvailability &=
                            ~(CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside);
                    if (move.SourceIndex == 4)
                        CastlingAvailability &=
                            ~(CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside);
                    break;
            }
        }

        /// <summary>
        ///     Sets EnPassant flag appropriately, clearing it if no En Passant is available
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
                    if ((move.SourceValue & BoardHelpers.RankMasks[startRank]) != 0
                        && (move.DestinationValue & BoardHelpers.RankMasks[endRank]) != 0)
                    {
                        EnPassantIndex = (ushort)(move.SourceIndex + enPassantIndexOffset);
                        return;
                    }
            }

            EnPassantIndex = null;
        }

        private void GetPiecesAtSourceAndDestination(MoveExt move, out PieceOfColor? pocSource,
            out PieceOfColor? pocDestination)
        {
            var sVal = move.SourceValue;
            pocSource = null;
            pocDestination = null;
            foreach (Piece piece in Enum.GetValues(typeof(Piece)))
            {
                var p = (int)piece;
                if (pocSource == null)
                {
                    if ((PiecesOnBoard[(int)Color.White][p] & sVal) != 0)
                        pocSource = new PieceOfColor { Color = Color.White, Piece = piece };
                    if ((PiecesOnBoard[(int)Color.Black][p] & sVal) != 0)
                        pocSource = new PieceOfColor { Color = Color.Black, Piece = piece };
                }

                if (pocDestination == null)
                {
                    if ((PiecesOnBoard[(int)Color.White][p] & sVal) != 0)
                        pocDestination = new PieceOfColor { Color = Color.White, Piece = piece };
                    if ((PiecesOnBoard[(int)Color.Black][p] & sVal) != 0)
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
            var xRayBishopAttacks = XRayBishopAttacks(TotalOccupancy, ActiveTotalOccupancy, ActivePlayerKingIndex);
            var xRayRookAttacks = XRayRookAttacks(TotalOccupancy, ActiveTotalOccupancy, ActivePlayerKingIndex);
            var bishopPinnedPieces = (PiecesOnBoard[NOpponentColor][BISHOP] | PiecesOnBoard[NOpponentColor][QUEEN]) &
                                     xRayBishopAttacks;
            var rookPinnedPieces = (PiecesOnBoard[NOpponentColor][ROOK] | PiecesOnBoard[NOpponentColor][QUEEN]) &
                                   xRayRookAttacks;
            var allPins = rookPinnedPieces | bishopPinnedPieces;
            while (allPins != 0)
            {
                var square = BitHelpers.BitScanForward(allPins);
                var squaresBetween = BoardHelpers.InBetween(square, ActivePlayerKingIndex);
                var piecesBetween = squaresBetween & ActiveTotalOccupancy;
                if (piecesBetween.CountSetBits() == 1) pinned |= piecesBetween;
                allPins &= allPins - 1;
            }

            return pinned;
        }

        public MoveExt GenerateMoveFromText(string moveText)
        {
            var md = MoveHelpers.GetAvailableMoveDetails(moveText, ActivePlayerColor);
            if (!md.SourceFile.HasValue || !md.SourceRank.HasValue)
            {
                var sourceIndex = FindPieceSourceIndex(md);
                md.SourceIndex = sourceIndex;
            }

            Debug.Assert(md.SourceIndex != null, "md.SourceIndex != null");
            Debug.Assert(md.DestinationIndex != null, "md.DestinationIndex != null");
            if (md.IsCapture && md.Piece == Piece.Pawn &&
                (OpponentTotalOccupancy & (1ul << md.DestinationIndex)) == 0 &&
                (md.DestinationRank == 2 || md.DestinationRank == 5)) md.MoveType = MoveType.EnPassant;
            var moveExt = MoveHelpers.GenerateMove(md.SourceIndex.Value, md.DestinationIndex.Value, md.MoveType,
                md.PromotionPiece ?? 0);
            return moveExt;
        }


        public string ValidateChecks()
        {
            var c = GetChecks(ActivePlayerColor);
            if (c.HasFlag(Check.Double))
                return "Both Kings are in check.";
            if (c.HasFlag(Check.Opposite)) return "Active side is in check.";
            return "";
        }


        private Check GetChecks(Color activePlayer)
        {
            var rv = Check.None;

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
            return ValidateNumberOfPiecesOnBoard(PiecesOnBoard);
        }

        private string ValidateEnPassantSquare()
        {
            return ValidateEnPassantSquare(PiecesOnBoard, EnPassantIndex, ActivePlayerColor);
        }

        private string ValidateCastlingRights()
        {
            return ValidateCastlingRights(PiecesOnBoard, CastlingAvailability, Chess960);
        }

        /// <summary>
        ///     Instance method to find if <paramref name="squareIndex" /> is attacked by a piece of <paramref name="color" />
        /// </summary>
        /// <param name="color">Color of possible attacker</param>
        /// <param name="squareIndex">Square which is possibly under attack</param>
        /// <returns>true if <paramref name="squareIndex" /> is attacked by <paramref name="color" />. False if not.</returns>
        public bool IsAttackedBy(Color color, ushort squareIndex)
        {
            return Bitboard.IsAttackedBy(color, squareIndex, PiecesOnBoard);
        }

        #region General Board Information

        public ulong[][] PiecesOnBoard;
        public string FEN { get; }
        public uint HalfmoveClock { get; set; }
        public uint MoveCounter { get; set; }
        public CastlingAvailability CastlingAvailability { get; set; }

        #region Color Properties and related fields

        public Color ActivePlayerColor { get; set; }
        public Color OpponentColor => ActivePlayerColor == Color.Black ? Color.White : Color.Black;
        private int NActiveColor => (int)ActivePlayerColor;
        private int NOpponentColor => (int)OpponentColor;

        #endregion

        #endregion

        #region Constant Piece Values for Indexing arrays

        // ReSharper disable InconsistentNaming
        private const int PAWN = (int)Piece.Pawn;
        private const int BISHOP = (int)Piece.Bishop;
        private const int KNIGHT = (int)Piece.Knight;
        private const int ROOK = (int)Piece.Rook;
        private const int QUEEN = (int)Piece.Queen;

        private const int KING = (int)Piece.King;
        // ReSharper restore InconsistentNaming

        #endregion

        #region Occupancy / Index Properties for shorthand access to occupancy board arrays

        /// <summary>
        ///     Occupancy of side-to-move's pawns
        /// </summary>
        public ulong ActivePawnOccupancy => PiecesOnBoard[NActiveColor][PAWN];

        /// <summary>
        ///     Occupancy of side-to-move's Knights
        /// </summary>
        public ulong ActiveKnightOccupancy => PiecesOnBoard[NActiveColor][KNIGHT];

        /// <summary>
        ///     Occupancy of side-to-move's Bishops
        /// </summary>
        public ulong ActiveBishopOccupancy => PiecesOnBoard[NActiveColor][BISHOP];

        /// <summary>
        ///     Occupancy of side-to-move's Rooks
        /// </summary>
        public ulong ActiveRookOccupancy => PiecesOnBoard[NActiveColor][ROOK];

        /// <summary>
        ///     Occupancy of side-to-move's Queen(s)
        /// </summary>
        public ulong ActiveQueenOccupancy => PiecesOnBoard[NActiveColor][QUEEN];

        /// <summary>
        ///     Occupancy of side-to-move's pieces
        /// </summary>
        public ulong ActiveTotalOccupancy
        {
            get
            {
                var activeOcc = PiecesOnBoard[NActiveColor];
                return activeOcc.Aggregate<ulong, ulong>(0, (current, t) => current | t);
            }
        }

        /// <summary>
        ///     Occupancy of opponent's pieces
        /// </summary>
        public ulong OpponentTotalOccupancy
        {
            get
            {
                ulong rv = 0;
                for (var i = 0; i < PiecesOnBoard[NOpponentColor].Length; i++) rv |= PiecesOnBoard[NOpponentColor][i];
                return rv;
            }
        }

        /// <summary>
        ///     Total board occupancy
        /// </summary>
        public ulong TotalOccupancy => ActiveTotalOccupancy | OpponentTotalOccupancy;

        /// <summary>
        ///     Index of side-to-move's King
        /// </summary>
        public ushort ActivePlayerKingIndex =>
            PiecesOnBoard[(int)ActivePlayerColor][Piece.King.ToInt()].GetSetBits()[0];

        /// <summary>
        ///     Value (occupancy board) of side-to-move's King
        /// </summary>
        public ulong ActivePlayerKingOccupancy => PiecesOnBoard[NActiveColor][KING];

        /// <summary>
        ///     Opponent's King's square index
        /// </summary>
        public ushort OpposingPlayerKingIndex =>
            PiecesOnBoard[(int)ActivePlayerColor.Toggle()][Piece.King.ToInt()].GetSetBits()[0];

        #endregion

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
        ///     Used by the Find[piece]MoveSourceIndex to find the source of a piece moving parsed from SAN text.
        /// </summary>
        /// <param name="md">Available Move details</param>
        /// <param name="pieceMoveMask">The move mask for the piece</param>
        /// <param name="pieceOccupancy">The occupancy for the piece in question</param>
        /// <returns></returns>
        private static ushort? FindPieceMoveSourceIndex(MoveDetail md, ulong pieceMoveMask, ulong pieceOccupancy)
        {
            ulong sourceSquares;
            if ((sourceSquares = pieceMoveMask & pieceOccupancy) == 0) return null;

            if (md.SourceFile != null)
                sourceSquares &= BoardHelpers.FileMasks[md.SourceFile.Value];
            if (md.SourceRank != null)
                sourceSquares &= BoardHelpers.RankMasks[md.SourceRank.Value];
            var indices = sourceSquares.GetSetBits();

            if (indices.Length != 1) return ushort.MaxValue;
            return indices[0];
        }

        /// <summary>
        ///     Finds the source square index for a King's move
        /// </summary>
        /// <param name="moveDetail">Move details, gathered from text input</param>
        /// <param name="kingOccupancy">The King's occupancy board</param>
        /// <param name="totalOccupancy">The board's occupancy</param>
        /// <returns></returns>
        public static ushort FindKingMoveSourceIndex(MoveDetail moveDetail, ulong kingOccupancy, ulong totalOccupancy)
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
        public static ushort FindQueenMoveSourceIndex(MoveDetail moveDetail, ulong queenOccupancy, ulong totalOccupancy)
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
        public static ushort FindRookMoveSourceIndex(MoveDetail moveDetail, ulong rookOccupancy, ulong totalOccupancy)
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
        public static ushort FindBishopMoveSourceIndex(MoveDetail moveDetail, ulong bishopOccupancy,
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
        public static ushort FindKnightMoveSourceIndex(MoveDetail moveDetail, ulong relevantPieceOccupancy)
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
        public static ushort FindPawnMoveSourceIndex(MoveDetail moveDetail, ulong pawnOccupancy, ulong totalOccupancy)
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
                    sourceIndex = (ushort)(8 * 2 + file % 8);
                if ((adjustedRelevantPieceOccupancy & BoardHelpers.RankMasks[1] & BoardHelpers.FileMasks[file]) != 0)
                    sourceIndex = (ushort)(1 * 8 + file % 8);
            }
            else //else source square was destination + 8 (a move one rank ahead), but we need to make sure a pawn was there
            {
                var supposedIndex = BoardHelpers.RankAndFileToIndex(
                    moveDetail.Color == Color.Black ? supposedRank.RankCompliment() : supposedRank,
                    moveDetail.DestinationFile.Value);
                if (supposedRank == 0)
                    throw new MoveException(
                        $"{moveDetail.MoveText}: Cannot possibly be a pawn at the source square {supposedIndex.IndexToSquareDisplay()} implied by move.");
                sourceIndex = (ushort)(supposedRank * 8 + moveDetail.DestinationFile.Value);
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


        #region FEN String Retrieval

        public string GetPiecePlacement()
        {
            var pieceSection = new char[64];
            for (var iColor = 0; iColor < 2; iColor++)
                for (var iPiece = 0; iPiece < 6; iPiece++)
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

            var sb = new StringBuilder();
            for (var rank = 0; rank < 8; rank++) //start at FEN Rank of zero -> 7
            {
                var emptyCount = 0;
                for (var file = 0; file < 8; file++)
                {
                    var paChar = pieceSection[rank * 8 + file];
                    if (paChar == 0)
                    {
                        emptyCount++;
                    }
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

                if (emptyCount != 0) sb.Append(emptyCount);
                if (rank != 7) sb.Append('/');
            }

            return sb.ToString();
        }

        public string GetSideToMoveStrRepresentation()
        {
            return ActivePlayerColor == Color.Black ? "b" : "w";
        }

        public string GetCastlingAvailabilityString()
        {
            return FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability);
        }

        public string GetEnPassantString()
        {
            return EnPassantIndex == null ? "-" : EnPassantIndex.Value.IndexToSquareDisplay();
        }

        public string GetHalfMoveClockString()
        {
            return HalfmoveClock.ToString();
        }

        public string GetMoveCounterString()
        {
            return MoveCounter.ToString();
        }

        public string ToFEN()
        {
            return
                $"{GetPiecePlacement()} {GetSideToMoveStrRepresentation()} {GetCastlingAvailabilityString()} {GetEnPassantString()} {GetHalfMoveClockString()} {GetMoveCounterString()}";
        }

        #endregion
    }
}