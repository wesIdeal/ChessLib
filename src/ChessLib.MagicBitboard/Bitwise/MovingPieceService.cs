using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("ChessLib.MagicBitboard.Bitwise.Tests")]
namespace ChessLib.MagicBitboard.Bitwise
{
    public class MovingPieceService : IMovingPieceService
    {
        private readonly ulong[,] ArrInBetween = new ulong[64, 64];

        private readonly ushort[] Index64 = {
                                    0, 47,  1, 56, 48, 27,  2, 60,
                                   57, 49, 41, 37, 28, 16,  3, 61,
                                   54, 58, 35, 52, 50, 42, 21, 44,
                                   38, 32, 29, 23, 17, 11,  4, 62,
                                   46, 55, 26, 59, 40, 36, 15, 53,
                                   34, 51, 20, 43, 31, 22, 10, 45,
                                   25, 39, 14, 33, 19, 30,  9, 24,
                                   13, 18,  8, 12,  7,  6,  5, 63
                                };

        public MovingPieceService()
        {
            Initialize();
        }

        /// <summary>
        /// Gets an array of bit indexes set to '1'
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public ushort[] GetSetBits(ulong u)
        {
            var rv = new List<ushort>(64); //Set max capacity to 64, since our 'array of bits' will be no larger.
            while (u != 0)
            {
                rv.Add(BitScanForward(u));
                u &= (u - 1);
            }
            return rv.ToArray();
        }

        /// <summary>
        ///     Gets the squares in between two squares, returns 0 for squares not linked diagonally or by file or rank
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>ulong value of in-between squares</returns>
        public ulong GetInBetweenSquares(ushort from, ushort to)
        {
            var square1 = Math.Min(from, to);
            var square2 = Math.Max(from, to);
            return this.ArrInBetween[square1, square2];
        }


        public ulong GetFileFromIndex(ushort idx)
        {
            var val = (ulong)1 << idx;
            foreach (var file in BoardConstants.Files)
            {
                if ((val & file) != 0)
                {
                    return file;
                }
            }
            throw new ArgumentException($"Cannot find file for index {idx}, board value of {val}.");
        }


        public static ulong ShiftN(ulong u) { return (u & BoardConstants.NotRank8Mask) << (8); }
        public static ulong ShiftE(ulong u) { return (u & BoardConstants.NotHFileMask) << 1; }
        public static ulong ShiftS(ulong u) { return (u & BoardConstants.NotRank1Mask) >> 8; }
        public static ulong ShiftW(ulong u) { return (u & BoardConstants.NotAFileMask) >> 1; }

        public static ulong Shift2N(ulong u) { return (u & (BoardConstants.NotRank7Mask & BoardConstants.NotRank8Mask)) << 16; }
        public static ulong Shift2E(ulong u) { return (u & (BoardConstants.NotGFileMask & BoardConstants.NotHFileMask)) << 2; }
        public static ulong Shift2S(ulong u) { return (u & (BoardConstants.NotRank1Mask & BoardConstants.NotRank2Mask)) >> 16; }
        public static ulong Shift2W(ulong u) { return (u & BoardConstants.NotBFileMask & BoardConstants.NotAFileMask) >> 2; }

        public static ulong ShiftNE(ulong u) { return ShiftE(ShiftN(u)); }
        public static ulong ShiftSE(ulong u) { return ShiftE(ShiftS(u)); }
        public static ulong ShiftSW(ulong u) { return ShiftW(ShiftS(u)); }
        public static ulong ShiftNW(ulong u) { return ShiftW(ShiftN(u)); }

        public static ulong ShiftNNE(ulong u) { return ShiftE(Shift2N(u)); }
        public static ulong ShiftENE(ulong u) { return ShiftN(Shift2E(u)); }
        public static ulong ShiftESE(ulong u) { return Shift2E(ShiftS(u)); }
        public static ulong ShiftSSE(ulong u) { return ShiftE(Shift2S(u)); }
        public static ulong ShiftSSW(ulong u) { return ShiftW(Shift2S(u)); }
        public static ulong ShiftWSW(ulong u) { return Shift2W(ShiftS(u)); }
        public static ulong ShiftWNW(ulong u) { return Shift2W(ShiftN(u)); }
        public static ulong ShiftNNW(ulong u) { return ShiftW(Shift2N(u)); }

        /// <summary>
        /// Uses de Bruijn Sequences to find the least-significant index for the first '1' in a ulong
        /// </summary>
        /// <param name="boardRep">A ulong 'array of bits' representing a chess board.</param>
        /// <returns>index of the least-significant set bit in ulong</returns>
        public ushort BitScanForward(ulong boardRep)
        {
            const ulong deBruijn = (0x03f79d71b4cb0a89ul);
            return Index64[((boardRep ^ (boardRep - 1)) * deBruijn) >> 58];
        }

        /// <summary>
        /// Gets the board value of the index in order to compare boards with bitwise math.
        /// </summary>
        /// <param name="idx">The board index, 0(A1)->63(H8)</param>
        /// <returns>ulong representing a square's value on the board</returns>
        public ulong GetBoardValueOfIndex(ushort idx) => ((ulong)1) << idx;

        /// <summary>
        /// Gets the permutations of blockers for a given attack mask. 
        /// </summary>
        /// <param name="mask">The relevant attack mask</param>
        /// <returns>All relevant occupancy boards for the given mask</returns>
        public ulong[] GetAllPermutations(ulong mask)
        {
            var setBitIndices = GetSetBits(mask);
            return GetAllPermutations(setBitIndices, 0, 0).Distinct().ToArray();
        }

        enum PieceDirection
        {
            South = -8, West = -1, East = (-West), North = (-South),
            NorthEast = North + East, SouthEast = South + East,
            NorthWest = North + West, SouthWest = South + West
        };

        private readonly PieceDirection[] RookDirections = new PieceDirection[] { PieceDirection.South, PieceDirection.North, PieceDirection.East, PieceDirection.West };
        private readonly PieceDirection[] BishopDirections = new PieceDirection[] { PieceDirection.SouthEast, PieceDirection.SouthWest, PieceDirection.NorthEast, PieceDirection.NorthWest };

        public ulong GetAttacks(Piece piece, ushort square, ulong occupancy)
        {
            PieceDirection[] directions = GetPieceDirections(piece);
            ulong result = 0;
            var squareValue = GetBoardValueOfIndex(square);
            foreach (var direction in directions)
            {
                Func<ulong, ulong> Shift = GetShiftMethodFromDirection(direction);
                if (Shift != null)
                {
                    ulong shiftedValue = squareValue;
                    while ((shiftedValue = Shift(shiftedValue)) != 0 &&
                        (shiftedValue & occupancy) == 0)
                    {
                        result |= shiftedValue;
                    }
                }
            }
            return result;
        }

        private PieceDirection[] GetPieceDirections(Piece piece)
        {
            return piece == Piece.Bishop ? BishopDirections : RookDirections;
        }

        private static Func<ulong, ulong> GetShiftMethodFromDirection(PieceDirection direction)
        {
            Func<ulong, ulong> Shift;
            switch (direction)
            {
                case PieceDirection.North:
                    Shift = ShiftN;
                    break;
                case PieceDirection.South:
                    Shift = ShiftS;
                    break;
                case PieceDirection.West:
                    Shift = ShiftW;
                    break;
                case PieceDirection.East:
                    Shift = ShiftE;
                    break;
                case PieceDirection.NorthEast:
                    Shift = ShiftNE;
                    break;
                case PieceDirection.SouthEast:
                    Shift = ShiftSE;
                    break;
                case PieceDirection.NorthWest:
                    Shift = ShiftNW;
                    break;
                case PieceDirection.SouthWest:
                    Shift = ShiftSW;
                    break;
                default:
                    Shift = null;
                    break;
            }

            return Shift;
        }

        /// <summary>
        /// Gets the permutations of Occupancy/Move boards from a given position
        /// </summary>
        /// <param name="pieceLocationIndex">The index of the piece</param>
        /// <param name="attackMask">The piece's associated attack mask from the position index</param>
        /// <param name="occupancyBoards">The associated occupancy boards</param>
        /// <returns>An array of blocker boards and corresponding moves based on blocker placement.</returns>
        public IEnumerable<MoveObstructionBoard> GetAllPermutationsForAttackMask(ushort pieceLocationIndex, ulong moveMask,
            ulong attackMask, IEnumerable<ulong> occupancyBoards)
        {
            foreach (var board in occupancyBoards)
            {
                var validMoves = CalculateValidMoves(pieceLocationIndex, moveMask, board);
                yield return new MoveObstructionBoard(board, validMoves);
            }
        }

        private ulong CalculateValidMoves(ushort pieceLocationIndex, ulong moveMask, ulong occupancy)
        {
            ulong moves = SearchNorth(pieceLocationIndex, moveMask, occupancy);
            moves |= SearchSouth(pieceLocationIndex, moveMask, occupancy);
            return moves;

        }

        private ulong SearchNorth(ushort pieceLocationIndex, ulong moveMask, ulong occupancy, bool attackSearch = false)
        {

            return 0;
        }

        private ulong SearchSouth(int pieceLocationIndex, ulong moveMask, ulong occupancy)
        {
            ulong movingPiece = 0;
            ulong validMoves = 0;
            const int lastSquare = 0;
            pieceLocationIndex += 8;
            for (movingPiece = (ulong)(1 << pieceLocationIndex);
                (long)movingPiece > lastSquare
                    && IsValidMove(movingPiece, moveMask)
                    && !IsBlocked(occupancy, movingPiece);
                pieceLocationIndex -= 8)
            {
                validMoves |= movingPiece;
            }
            return validMoves;
        }

        private ulong SearchWest(ushort pieceLocationIndex, ulong moveMask, ulong occupancy)
        {
            ulong validMoves = 0;
            var lastSquare = GetWestExtent(pieceLocationIndex);
            pieceLocationIndex += 8;
            for (var movingPiece = (ulong)(1 << pieceLocationIndex);
                movingPiece > lastSquare
                    && IsValidMove(movingPiece, moveMask)
                    && !IsBlocked(occupancy, movingPiece);
                pieceLocationIndex -= 1)
            {
                validMoves |= movingPiece;
            }
            return validMoves;
        }


        /// <summary>
        /// Gets the rank of a square
        /// </summary>
        /// <param name="squareIndex">Square index; 0 = a1, 63 = h8</param>
        /// <returns>A value indicating a zero-based rank [0...7]</returns>
        protected ushort GetRank(ushort squareIndex)
        {
            Debug.Assert(squareIndex >= 0 && squareIndex < 64, $"Rank from square index {squareIndex} cannot be determined.");
            var rank = squareIndex / 8;
            return (ushort)rank;
        }

        /// <summary>
        /// Gets the file of a square
        /// </summary>
        /// <param name="squareIndex">Square index; 0 = a1, 63 = h8</param>
        /// <returns>A value indicating a zero-based file [0...7]</returns>
        protected ushort GetFile(ushort squareIndex)
        {
            Debug.Assert(squareIndex >= 0 && squareIndex < 64, $"File from square index {squareIndex} cannot be determined.");
            var file = squareIndex % 8;
            return (ushort)file;
        }

        protected ulong GetWestExtent(ushort index)
        {
            return (ulong)(1 << (index - (index % 8)));
        }

        protected ulong GetEastExtent(ushort index)
        {
            var rank = GetRank(index) * 8;
            var extentSquare = rank + 7;
            return (ulong)1 << extentSquare;
        }

        protected IEnumerable<ushort> GetNorthEastDiagonal(ushort index)
        {
            var indexFile = GetFile(index);
            var indexRank = GetRank(index);
            var check = indexRank - indexFile;
            foreach (ushort sq in Enumerable.Range(0, 63))
            {
                var file = GetFile(sq);
                var rank = GetRank(sq);
                if (rank - file == check)
                {
                    if (file > 7 || rank > 7) { break; }
                    yield return sq;
                }
            }
        }

        protected IEnumerable<ushort> GetNorthWestDiagonal(ushort sq1)
        {
            var check = ((sq1 % 8) + (sq1 / 8)) ^ 7;
            foreach (ushort sq2 in Enumerable.Range(0, 63))
            {
                var rank = GetRank(sq2);
                var file = GetFile(sq2);
                var sq2Check = (rank + file) ^ 7;
                if (file < 0 || rank > 7) { break; }
                if (sq2Check == check)
                {
                    yield return sq2;
                }
            }
        }

        private static bool IsBlocked(ulong occupancy, ulong movingPiece)
        {
            bool isBlocked = (occupancy ^ movingPiece) == 0;
            return isBlocked;
        }

        private static bool IsValidMove(ulong movingPiece, ulong moveMask)
        {
            bool isValidMove = (moveMask & movingPiece) != 0;
            return isValidMove;
        }




        /// <summary>
        /// Sets a bit (specified by <paramref name="boardIndex">bitIndex</paramref>) on a ulong by ORing the value with 1 SHL bitIndex 
        /// </summary>
        /// <param name="u">ulong board representation</param>
        /// <param name="boardIndex">index of square to set</param>
        public ulong SetBit(ulong u, ushort boardIndex) => u | (1ul << boardIndex);

        /// <summary>
        /// Bitwise 'and' operation with the 'not' of the bit's index to clear.
        /// </summary>
        /// <remarks>Ex. boardRep = 7, clear bit index of 3: boardRep = 0111b; return boardRep ANDed with NOT((1 shl 3)= 0100b) or 0111b AND 1011b returns 0011b</remarks>
        /// <param name="boardRep">A ulong, in this case representing a chessboard</param>
        /// <param name="bitIndex">Index of bit to clear</param>
        /// <returns>ulong, <paramref name="boardRep">boardRep</paramref>, with the index of <paramref name="bitIndex">bitIndex</paramref> cleared.</returns>
        public ulong ClearBit(ulong boardRep, int bitIndex) => boardRep & ~(1ul << bitIndex);


        private IEnumerable<ulong> GetAllPermutations(ushort[] setBits, int idx, ulong value)
        {
            value = SetBit(value, setBits[idx]);
            yield return value;
            int index = idx + 1;
            if (index < setBits.Length)
            {
                using IEnumerator<ulong> occupancyPermutations = GetAllPermutations(setBits, index, value).GetEnumerator();
                while (occupancyPermutations.MoveNext())
                {
                    yield return occupancyPermutations.Current;
                }
            }
            value = ClearBit(value, setBits[idx]);
            yield return value;
            if (index < setBits.Length)
            {
                using (var occupancyPermutations = GetAllPermutations(setBits, index, value).GetEnumerator())
                {
                    while (occupancyPermutations.MoveNext())
                    {
                        yield return occupancyPermutations.Current;
                    }
                }
            }
        }

        private void InitializeInBetween()
        {
            for (var f = 0; f < 64; f++)
                for (var t = f; t < 64; t++)
                {
                    const long m1 = -1;
                    const long aFileBorder = 0x0001010101010100;
                    const long b2DiagonalBorder = 0x0040201008040200;
                    const long hFileBorder = 0x0002040810204080;

                    var between = (m1 << f) ^ (m1 << t);
                    long file = (t & 7) - (f & 7);
                    long rank = ((t | 7) - f) >> 3;
                    var line = ((file & 7) - 1) & aFileBorder;
                    line += 2 * (((rank & 7) - 1) >> 58); /* b1g1 if same rank */
                    line += (((rank - file) & 15) - 1) & b2DiagonalBorder; /* b2g7 if same diagonal */
                    line += (((rank + file) & 15) - 1) & hFileBorder; /* h1b7 if same anti-diagonal */
                    line *= between & -between; /* mul acts like shift by smaller boardIndex */
                    ArrInBetween[f, t] = (ulong)(line & between); /* return the bits on that line in-between */
                }
        }



        private void Initialize()
        {
            InitializeInBetween();
        }
    }
}
