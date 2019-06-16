using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ChessLib.Data.Helpers
{
    public static class BoardHelpers
    {
        static BoardHelpers()
        {
            InitializeInBetween();
        }
        #region Initialization

        private static void InitializeInBetween()
        {
            for (var f = 0; f < 64; f++)
            {
                for (var t = f; t < 64; t++)
                {
                    const long m1 = (-1);
                    const long aFileBorder = (0x0001010101010100);
                    const long b2DiagonalBorder = (0x0040201008040200);
                    const long hFileBorder = (0x0002040810204080);

                    var between = (m1 << f) ^ (m1 << t);
                    long file = (t & 7) - (f & 7);
                    long rank = ((t | 7) - f) >> 3;
                    var line = ((file & 7) - 1) & aFileBorder;
                    line += 2 * (((rank & 7) - 1) >> 58); /* b1g1 if same rank */
                    line += (((rank - file) & 15) - 1) & b2DiagonalBorder; /* b2g7 if same diagonal */
                    line += (((rank + file) & 15) - 1) & hFileBorder; /* h1b7 if same anti-diagonal */
                    line *= between & -between; /* mul acts like shift by smaller square */
                    ArrInBetween[f, t] = (ulong)(line & between);   /* return the bits on that line in-between */
                }
            }
        }

        #endregion


        #region Constant Piece and Color Values for Indexing arrays
        // ReSharper disable InconsistentNaming
        public const int PAWN = (int)Piece.Pawn;
        public const int BISHOP = (int)Piece.Bishop;
        public const int KNIGHT = (int)Piece.Knight;
        public const int ROOK = (int)Piece.Rook;
        public const int QUEEN = (int)Piece.Queen;
        public const int KING = (int)Piece.King;
        public const int WHITE = (int)Color.White;
        public const int BLACK = (int)Color.Black;
        // ReSharper restore InconsistentNaming
        #endregion

        /// <summary>
        /// Gets the opoposite color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Color Toggle(this Color c) => c == Color.White ? Color.Black : Color.White;

        /// <summary>
        /// Contains square values for a square index
        /// </summary>
        public static readonly ulong[] IndividualSquares =
       new ulong[]{
            0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80,
            0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000,
            0x8000, 0x10000, 0x20000, 0x40000, 0x80000, 0x100000, 0x200000,
            0x400000, 0x800000, 0x1000000, 0x2000000, 0x4000000, 0x8000000, 0x10000000,
            0x20000000, 0x40000000, 0x80000000, 0x100000000, 0x200000000, 0x400000000, 0x800000000,
            0x1000000000, 0x2000000000, 0x4000000000, 0x8000000000, 0x10000000000, 0x20000000000, 0x40000000000,
            0x80000000000, 0x100000000000, 0x200000000000, 0x400000000000, 0x800000000000, 0x1000000000000, 0x2000000000000,
            0x4000000000000, 0x8000000000000, 0x10000000000000, 0x20000000000000, 0x40000000000000, 0x80000000000000, 0x100000000000000,
            0x200000000000000, 0x400000000000000, 0x800000000000000, 0x1000000000000000, 0x2000000000000000, 0x4000000000000000, 0x8000000000000000,

        };

        /// <summary>
        /// Contains set bits for each rank
        /// </summary>
        public static ulong[] RankMasks = {
            0xff,               //R1
            0xff00,             //R2
            0xff0000,           //R3
            0xff000000,         //R4
            0xff00000000,       //R5
            0xff0000000000,     //R6
            0xff000000000000,   //R7
            0xff00000000000000  //R8
        };

        /// <summary>
        /// Contains set bits for each file
        /// </summary>
        public static ulong[] FileMasks = {
            0x101010101010101,  //A
            0x202020202020202,  //B
            0x404040404040404,  //C
            0x808080808080808,  //D
            0x1010101010101010, //E
            0x2020202020202020, //F
            0x4040404040404040, //G
            0x8080808080808080  //H
        };

        private static readonly ulong[,] ArrInBetween = new ulong[64, 64];

        /// <summary>
        /// Gets the squares in between two squares, returns 0 for squares not linked diagonally or by file or rank
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static ulong InBetween(int from, int to)
        {
            var square1 = Math.Min(from, to);
            var square2 = Math.Max(from, to);
            return ArrInBetween[square1, square2];
        }

        /// <summary>
        /// Gets the OR'ed version of all bitboard representations
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static ulong TotalOccupancy(this IBoard board)
        {
            return board.GetPiecePlacement().Occupancy();
        }

        /// <summary>
        /// Gets the OR'ed version of all active-color pieces
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static ulong ActiveOccupancy(this IBoard board)
        {
            return board.GetPiecePlacement().Occupancy(board.ActivePlayer);
        }
        /// <summary>
        /// Gets the OR'ed version of all non-active-color (opponent) pieces
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static ulong OpponentOccupancy(this IBoard board)
        {
            return board.GetPiecePlacement().Occupancy(board.OpponentColor());
        }

        /// <summary>
        /// Gets the occupancy of a board by color and/or piece (or neither = TotalOccupancy)
        /// </summary>
        /// <param name="board"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Occupancy(this ulong[][] board, Color? c = null, Piece? p = null)
        {
            if (c == null && p == null)
                return board.Select(x => x.Aggregate((acc, val) => acc |= val)).Aggregate((acc, val) => acc |= val);
            if (c == null)
                return board[(int)Color.White][(int)p] | board[(int)Color.Black][(int)p];
            if (p == null)
            {
                return board[(int)c].Aggregate((curr, val) => curr |= val);
            }

            return board[(int)c][(int)p];
        }


        #region Enum ToInt() methods
        /// <summary>
        /// Extension to get the int of a color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Color c) => (int)c;

        /// <summary>
        /// Extension to get the int of a piece
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Piece p) => (int)p;

        /// <summary>
        /// Extension to get the int of a file
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this File f) => (int)f;

        /// <summary>
        /// Extension to get the int of a rank
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Rank r) => (int)r;

        /// <summary>
        /// Gets the hex display of a long (for debugging, mainly)
        /// </summary>
        /// <param name="u">long to get display from</param>
        /// <param name="appendHexNotation">append '0x' to the representation</param>
        /// <param name="pad">pad length to a certain size</param>
        /// <param name="padSize">size to pad</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexDisplay(this ulong u, bool appendHexNotation = true, bool pad = false, int padSize = 64)
        {
            var str = Convert.ToString((long)u, 16);
            if (pad)
            {
                str = str.PadLeft(padSize, '0');
            }
            if (appendHexNotation)
            {
                str = "0x" + str;
            }
            return str;
        }
        #endregion

        #region Array Position to Friendly Position Helpers

        /// <summary>
        /// Gets the index of a square
        /// </summary>
        /// <param name="square">SAN square representation (A1, H5, E4, etc). Must be either '-' (FEN En Passant) or 2 characters</param>
        /// <returns>Square index</returns>
        /// <exception cref="ArgumentException">Thrown if square length, File, or Rank is invalid.</exception>
        public static ushort? SquareTextToIndex(this string square)
        {
            if (square.Trim() == "-")
            {
                return null;
            }
            if (square.Length != 2)
            {
                throw new ArgumentException($"Square passed to SquareTextToIndex(), {square} has an invalid length.");
            }
            var file = Char.ToLower(square[0]);
            var rank = UInt16.Parse(square[1].ToString());
            if (!Char.IsLetter(file) || file < 'a' || file > 'h')
            {
                throw new ArgumentException("File portion of square-text should be a letter, between 'a' and 'h'.");
            }
            if (rank < 1 || rank > 8)
            {
                throw new ArgumentException("Rank portion of square-text should be a digit with a value between 1 and 8.");
            }
            var rankMultiplier = rank - 1;
            return (ushort)((rankMultiplier * 8) + file - 'a');
        }

        /// <summary>
        /// Converts rank and file to a board index
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="file"></param>
        /// <returns>Corresponding board index specified by rank and file</returns>
        public static ushort RankAndFileToIndex(ushort rank, ushort file)
        {
            Debug.Assert((rank >= 0 && rank <= 7) && (file >= 0 && file <= 7));
            return (ushort)((rank * 8) + file);
        }
        /// <summary>
        /// Gets a File basked on square index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static File GetFile(this int square) => (File)(square % 8);

        /// <summary>
        /// Gets a File basked on square index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetFile(this ushort square) => (ushort)(square % 8);

        /// <summary>
        /// Gets a Rank basked on square index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rank GetRank(this int square)
        {
            Debug.Assert(square >= 0 && square < 64);
            return (Rank)((ushort)square).GetRank();
        }

        /// <summary>
        /// Gets a Rank basked on square index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetRank(this ushort square)
        {
            return (ushort)(square / 8);
        }
        /// <summary>
        /// Gets a rank index from square indes
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankFromIdx(this ushort idx) => (ushort)(idx / 8);

        /// <summary>
        /// Gets a file index from a square index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort FileFromIdx(this ushort idx) => (ushort)(idx % 8);

        /// <summary>
        /// Gets 
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankCompliment(this ushort rank) => (ushort)Math.Abs(rank - 7);

        #endregion

        /// <summary>
        /// Returns type of piece at the given index
        /// </summary>
        /// <param name="idx">board index</param>
        /// <param name="occupancy">occupancy arrays</param>
        /// <returns>Type of Piece, if found, otherwise null</returns>
        public static Piece? GetTypeOfPieceAtIndex(ushort idx, in ulong[][] occupancy)
        {
            var val = 1ul << idx;
            foreach (var color in Enum.GetValues(typeof(Color)))
            {
                foreach (var piece in (Piece[])Enum.GetValues(typeof(Piece)))
                {
                    if ((val & occupancy[(int)color][(int)piece]) != 0)
                        return piece;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a piece of color object for the index
        /// </summary>
        /// <param name="occupancy"></param>
        /// <param name="idx"></param>
        /// <returns>The object representing the piece at an index, or null of the piece isn't there.</returns>
        public static PieceOfColor? GetPieceOfColorAtIndex(this ulong[][] occupancy, ushort idx)
        {
            var val = 1ul << idx;
            foreach (var color in Enum.GetValues(typeof(Color)))
            {
                foreach (var piece in (Piece[])Enum.GetValues(typeof(Piece)))
                {
                    if ((val & occupancy[(int)color][(int)piece]) != 0)
                        return new PieceOfColor() { Color = (Color)color, Piece = piece };
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a piece of color object for the index
        /// </summary>
        /// <param name="occupancy"></param>
        /// <param name="idx"></param>
        /// <returns>The object representing the piece at an index, or null of the piece isn't there.</returns>
        public static PieceOfColor? GetPieceOfColorAtIndex(this IBoard board, ushort index) => GetPieceOfColorAtIndex(board.GetPiecePlacement(), index);

        /// <summary>
        /// Returns a boolean to represent if the specified player is in check
        /// </summary>
        /// <param name="board"></param>
        /// <param name="playerInCheckColor"></param>
        /// <returns></returns>
        public static bool IsPlayerInCheck(this ulong[][] board, int playerInCheckColor)
        {
            var kingIndex = board[playerInCheckColor][BoardHelpers.KING].GetSetBits()[0];
            return Bitboard.IsSquareAttackedByColor(kingIndex, (Color)(1 - playerInCheckColor), board);
        }

        /// <summary>
        /// Generate array of potential moves, disregarding legality of move
        /// </summary>
        /// <param name="board"></param>
        /// <param name="c"></param>
        /// <param name="enPassentSq"></param>
        /// <param name="ca"></param>
        /// <returns></returns>
        public static IMoveExt[] GenerateAllPseudoLegalMoves(this ulong[][] board, Color c, ushort? enPassentSq, CastlingAvailability ca)
        {
            var rv = new List<MoveExt>();
            var nColor = (int)c;

            for (int i = 0; i < 6; i++)
            {
                var p = (Piece)i;
                var pieceLocations = board[nColor][i].GetSetBits();
                foreach (var sq in pieceLocations)
                {
                    var pseudoLegalMoves = Bitboard.GetPseudoLegalMoves(p, sq, BoardHelpers.Occupancy(board, c), BoardHelpers.Occupancy(board, c.Toggle()), c, enPassentSq, ca, out List<MoveExt> plm);
                    rv.AddRange(plm);
                }
            }
            return rv.ToArray();
        }

        /// <summary>
        ///     Clears appropriate castling availability flag when <paramref name="movingPiece">piece moving</paramref> is a
        ///     <see cref="Piece.Rook">Rook</see> or <see cref="Piece.King">King</see>
        /// </summary>
        /// <param name="move">Move object</param>
        /// <param name="movingPiece">Piece that is moving</param>
        public static CastlingAvailability GetCastlingAvailabilityPostMove(IBoard board, MoveExt move, Piece movingPiece)
        {
            var ca = board.CastlingAvailability;
            switch (movingPiece)
            {
                case Piece.Rook:
                    if (move.SourceIndex == 56) ca &= ~CastlingAvailability.BlackQueenside;
                    if (move.SourceIndex == 63) ca &= ~CastlingAvailability.BlackKingside;
                    if (move.SourceIndex == 0) ca &= ~CastlingAvailability.WhiteQueenside;
                    if (move.SourceIndex == 7) ca &= ~CastlingAvailability.WhiteKingside;
                    break;
                case Piece.King:
                    if (move.SourceIndex == 60)
                        ca &=
                            ~(CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside);
                    if (move.SourceIndex == 4)
                        ca &=
                            ~(CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside);
                    break;
            }

            return ca;
        }

        /// <summary>
        ///     Sets EnPassant flag appropriately, clearing it if no En Passant is available
        /// </summary>
        /// <param name="move"></param>
        /// <param name="pocSource"></param>
        public static ushort? GetEnPassentIndex(MoveExt move, PieceOfColor? pocSource)
        {
            ushort? rv = null;
            if (pocSource.HasValue)
            {
                var startRank = pocSource.Value.Color == Color.White ? 1 : 6;
                var endRank = pocSource.Value.Color == Color.White ? 3 : 4;
                var enPassantIndexOffset = pocSource.Value.Color == Color.White ? 8 : -8;
                if (pocSource.Value.Piece == Piece.Pawn)
                    if ((move.SourceValue & BoardHelpers.RankMasks[startRank]) != 0
                        && (move.DestinationValue & BoardHelpers.RankMasks[endRank]) != 0)
                    {
                        rv = (ushort)(move.SourceIndex + enPassantIndexOffset);
                    }
            }

            return rv;
        }

        /// <summary>
        /// Applies a move to a board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public static IBoard ApplyMoveToBoard(this IBoard board, in MoveExt move)
        {
            var pieceMoving = GetPieceOfColorAtIndex(board.GetPiecePlacement(), move.SourceIndex);
            var isCapture = (board.GetPiecePlacement().Occupancy(board.ActivePlayer.Toggle()) & move.DestinationValue) != 0;
            var isPawnMove = GetTypeOfPieceAtIndex(move.SourceIndex, board.GetPiecePlacement()).Equals(Piece.Pawn);
            var halfMoveClock = (isCapture || isPawnMove) ? 0 : (board.HalfmoveClock + 1);
            var fullMoveCounter =
                (board.ActivePlayer == Color.Black) ? board.FullmoveCounter + 1 : board.FullmoveCounter;

            var piecePlacement = board.GetPiecePlacement().GetBoardPostMove(board.ActivePlayer, move);
            var castlingAvailability = GetCastlingAvailabilityPostMove(board, move, pieceMoving.Value.Piece);
            var enPassantSquare = GetEnPassentIndex(move, pieceMoving.Value);
            var activePlayer = board.ActivePlayer.Toggle();

            return new BoardInfo(piecePlacement, activePlayer, castlingAvailability, enPassantSquare, halfMoveClock, fullMoveCounter, false);
        }

        /// <summary>
        /// Gets the piece setup post-move
        /// </summary>
        /// <param name="board"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public static ulong[][] GetBoardPostMove(this IBoard board, in MoveExt move)
        {
            return GetBoardPostMove(board.GetPiecePlacement(), board.ActivePlayer, move);
        }
        /// <summary>
        /// Gets a board's piece setup after the specified player makes the specified move
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="activePlayerColor"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public static ulong[][] GetBoardPostMove(this ulong[][] currentBoard, in Color activePlayerColor, in MoveExt move)
        {
            var nActiveColor = (int)activePlayerColor;
            var opponentColor = activePlayerColor.Toggle();
            var nOppColor = (int)opponentColor;
            var resultantBoard = new ulong[2][];
            var pieceMoving = GetTypeOfPieceAtIndex(move.SourceIndex, currentBoard);
            for (var i = 0; i < 2; i++)
            {
                resultantBoard[i] = new ulong[6];
                foreach (var p in Enum.GetValues(typeof(Piece)))
                {
                    resultantBoard[i][(int)p] = currentBoard[i][(int)p];
                    if (i == nActiveColor && (Piece)p == pieceMoving)
                    {
                        resultantBoard[i][(int)p] = BitHelpers.ClearBit(resultantBoard[i][(int)p], move.SourceIndex);
                        resultantBoard[i][(int)p] = resultantBoard[i][(int)p].SetBit(move.DestinationIndex);
                    }
                    else if (i == (int)opponentColor)
                    {
                        resultantBoard[i][(int)p] = BitHelpers.ClearBit(resultantBoard[i][(int)p], move.DestinationIndex);
                    }
                }
            }
            if (move.MoveType == MoveType.Castle)
            {
                resultantBoard[nActiveColor][ROOK] = GetRookBoardPostCastle(move, resultantBoard[nActiveColor][ROOK]);
            }
            else if (move.MoveType == MoveType.EnPassant)
            {
                var capturedPawnValue = 1ul << (opponentColor == Color.Black ? move.DestinationIndex - 8 : move.DestinationIndex + 8);
                resultantBoard[nOppColor][PAWN] &= ~(capturedPawnValue);
            }
            else if (move.MoveType == MoveType.Promotion)
            {
                resultantBoard[nActiveColor][PAWN] &= ~(move.DestinationValue);
                switch (move.PromotionPiece)
                {
                    case PromotionPiece.Knight:
                        resultantBoard[nActiveColor][KNIGHT] |= move.DestinationValue;
                        break;
                    case PromotionPiece.Bishop:
                        resultantBoard[nActiveColor][BISHOP] |= move.DestinationValue;
                        break;
                    case PromotionPiece.Rook:
                        resultantBoard[nActiveColor][ROOK] |= move.DestinationValue;
                        break;
                    case PromotionPiece.Queen:
                        resultantBoard[nActiveColor][QUEEN] |= move.DestinationValue;
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


        public static Color OpponentColor(this IBoard board) => board.ActivePlayer.Toggle();

        public static ushort ActiveKingIndex(this IBoard board)
        {
            return board.GetPiecePlacement()[board.ActivePlayer.ToInt()][KING].GetSetBits()[0];
        }

        public static ushort OpponentKingIndex(this IBoard board) => board.GetPiecePlacement()[board.OpponentColor().ToInt()][KING].GetSetBits()[0];


        public static bool IsActivePlayerInCheck(this IBoard board) =>
            IsColorInCheck(board.GetPiecePlacement(), board.ActivePlayer, board.ActiveKingIndex());

        public static bool IsOpponentInCheck(this IBoard board) =>
            IsColorInCheck(board.GetPiecePlacement(), board.OpponentColor(), board.OpponentKingIndex());

        private static bool IsColorInCheck(ulong[][] board, Color checkedColor, ushort? checkedColorKingIdx)
        {
            checkedColorKingIdx = checkedColorKingIdx ?? board[checkedColor.ToInt()][KING].GetSetBits()[0];
            Debug.Assert(checkedColorKingIdx.HasValue);
            return Bitboard.IsSquareAttackedByColor(checkedColorKingIdx.Value, checkedColor.Toggle(), board);
        }

        /// <summary>
        /// Determines if active player is stalemated
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool IsStalemate(this IBoard board)
        {
            if (board.IsActivePlayerInCheck())
            {
                return false;
            }
            var canAnyPieceMove = false;
            var myPieceLocations = board.GetPiecePlacement().Occupancy(board.ActivePlayer).GetSetBits();
            foreach (var square in myPieceLocations)
            {
                if (canAnyPieceMove == false)
                {
                    var pieceType = BoardHelpers.GetTypeOfPieceAtIndex(square, board.GetPiecePlacement());
                    Debug.Assert(pieceType.HasValue);
                    if (board.CanPieceMove(square))
                    {
                        canAnyPieceMove = true;
                        break;
                    }

                }
            }
            return !canAnyPieceMove;
        }

        /// <summary>
        /// Determines if active player has been mated
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool IsCheckmate(this IBoard board)
        {
            return board.IsActivePlayerInCheck() && !CanEvadeThroughBlockOrCapture(board, board.ActivePlayer);
        }

        /// <summary>
        /// Can the specified color's king evade an attack through a block or a capture
        /// </summary>
        /// <param name="board"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CanEvadeThroughBlockOrCapture(in IBoard board, Color? c = null)
        {
            if (c.HasValue)
                board.ActivePlayer = c.Value;
            return GetEvasions(board).Any();
        }

        /// <summary>
        /// Gets a list of possible evasions for the active player's King
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static MoveExt[] GetEvasions(this IBoard board)
        {
            var rv = new List<MoveExt>();
            var nColor = board.ActivePlayer.ToInt();
            var nOppColor = board.OpponentColor().ToInt();
            var kingIndex = board.ActiveKingIndex();
            var activeOccupancy = board.GetPiecePlacement().Occupancy(board.ActivePlayer);
            var oppOccupancy = board.GetPiecePlacement().Occupancy(board.OpponentColor());
            var piecesAttacking = board.PiecesAttackingSquare(kingIndex) & board.GetPiecePlacement().Occupancy(board.ActivePlayer.Toggle());
            var attackerIndexes = piecesAttacking.GetSetBits();


            //find if attacker can be blocked. If double check (more than one attacker), the king must move
            if (attackerIndexes.Length == 1)
            {
                var attackerIdx = attackerIndexes[0];
                var activeOccupancyNotKing = activeOccupancy & ~(kingIndex.GetBoardValueOfIndex());
                foreach (var occupiedSquare in activeOccupancyNotKing.GetSetBits())
                {
                    var piece = GetTypeOfPieceAtIndex(occupiedSquare, board.GetPiecePlacement());
                    var attackedSquares =
                        Bitboard.GetPseudoLegalMoves(piece.Value, occupiedSquare, activeOccupancy, oppOccupancy, board.ActivePlayer, board.EnPassantSquare, board.CastlingAvailability, out _);
                    var squaresBetween = InBetween(kingIndex, attackerIdx);
                    ulong destination;
                    if ((destination = (attackedSquares & squaresBetween)) != 0)
                    {
                        var destIdx = destination.GetSetBits();
                        foreach (var potentialBlockingPiece in destIdx)
                        {
                            var move = MoveHelpers.GenerateMove(occupiedSquare, potentialBlockingPiece);
                            rv.Add(move);
                        }
                    }
                }

            }
            Bitboard.GetPseudoLegalMoves(Piece.King, kingIndex, activeOccupancy, oppOccupancy, board.ActivePlayer,
                board.EnPassantSquare, CastlingAvailability.NoCastlingAvailable, out List<MoveExt> plMoves);
            foreach (var mv in plMoves)
            {
                var boardPostMove = GetBoardPostMove(board.GetPiecePlacement(), board.ActivePlayer, mv);
                if (!Bitboard.IsSquareAttackedByColor(mv.DestinationIndex, (Color)nOppColor, boardPostMove))
                {
                    rv.Add(mv);
                }
            }

            return rv.ToArray();
        }

        /// <summary>
        /// Gets piece value of pieces that attack the square index.
        /// </summary>
        /// <param name="board">The IBoard from which to derive board information.</param>
        /// <param name="squareIndex">index of square being attacked.</param>
        /// <returns></returns>
        public static ulong PiecesAttackingSquare(this IBoard board, in ushort squareIndex)
        {
            var piecesOnBoard = board.GetPiecePlacement();
            var total = piecesOnBoard
                .Select(color => color.Aggregate((current, x) => current |= x))
                .Aggregate((current, x) => current |= x);
            var pawnWhite = piecesOnBoard[WHITE][PAWN];
            var pawnBlack = piecesOnBoard[BLACK][PAWN];
            var knight = piecesOnBoard[BLACK][KNIGHT] | piecesOnBoard[WHITE][KNIGHT];
            var bishop = piecesOnBoard[BLACK][BISHOP] | piecesOnBoard[WHITE][BISHOP];
            var rook = piecesOnBoard[BLACK][ROOK] | piecesOnBoard[WHITE][ROOK];
            var queen = piecesOnBoard[BLACK][QUEEN] | piecesOnBoard[WHITE][QUEEN];
            var king = piecesOnBoard[BLACK][KING] | piecesOnBoard[WHITE][KING];
            return (Bitboard.GetAttackedSquares(Piece.Pawn, squareIndex, total, Color.Black) & pawnWhite)
                   | (Bitboard.GetAttackedSquares(Piece.Pawn, squareIndex, total, Color.White) & pawnBlack)
                   | (Bitboard.GetAttackedSquares(Piece.Knight, squareIndex, total) & knight)
                   | (Bitboard.GetAttackedSquares(Piece.Bishop, squareIndex, total) & bishop)
                   | (Bitboard.GetAttackedSquares(Piece.Rook, squareIndex, total) & rook)
                   | (Bitboard.GetAttackedSquares(Piece.Queen, squareIndex, total) & (queen))
                   | (Bitboard.GetAttackedSquares(Piece.King, squareIndex, total) & king);

        }



        #region FEN String Retrieval

        public static string GetFENPiecePlacement(this IBoard board)
        {
            return board.GetPiecePlacement().GetPiecePlacement();
        }

        public static string GetFENSideToMoveStrRepresentation(this IBoard board)
        {
            return board.ActivePlayer == Color.Black ? "b" : "w";
        }

        public static string GetFENCastlingAvailabilityString(this IBoard board)
        {
            return FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(board.CastlingAvailability);
        }

        public static string GetFENEnPassantString(this IBoard board)
        {
            return board.EnPassantSquare == null ? "-" : board.EnPassantSquare.Value.IndexToSquareDisplay();
        }

        public static string GetFENHalfMoveClockString(this IBoard board)
        {
            return board.HalfmoveClock.ToString();
        }

        public static string GetFENMoveCounterString(this IBoard board)
        {
            return board.FullmoveCounter.ToString();
        }

        public static string ToFEN(this IBoard b)
        {
            return
                $"{b.GetFENPiecePlacement()} {b.GetFENSideToMoveStrRepresentation()} {b.GetFENCastlingAvailabilityString()} {b.GetFENEnPassantString()} {b.GetFENHalfMoveClockString()} {b.GetFENMoveCounterString()}";
        }

        #endregion
    }
}
