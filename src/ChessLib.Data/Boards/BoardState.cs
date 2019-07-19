using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Boards
{
    public interface IBoardState
    {
        /// <summary>
        /// FIELD               [bit index, from smallest]
        /// Half Move Clock     [Bits 0 - 7]
        /// En Passant Index    [Bits 8 - 12] 0-based index for board ranks 3 and 6, A3-H3, A6-H6
        /// Castling Rights     [Bits 13 - 15]
        /// Captured Piece      [Bits 17 - 19] 1-5 (pawn - Queen, 0 for none)
        /// Unused              [Bits 20 - 32]
        /// </summary>
        uint BoardStateStorage { get; }
    }
    public class BoardState : IBoardState
    {
        /// <summary>
        /// Makes an archival board state
        /// </summary>
        /// <param name="halfMoveClock">Half move clock. Must be less than 256</param>
        /// <param name="enPassantIndex">Board index (a1 = 0, h8 = 63) of the EP square, if any</param>
        /// <param name="capturedPiece">The piece that was captured, if any</param>
        /// <param name="castlingAvailability">Castling Availability</param>
        public BoardState(ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece,
            CastlingAvailability castlingAvailability)
        {
            var pieceCaptured = BoardStateHelpers.GetPieceCaptured(capturedPiece);
            var castlingAvail = BoardStateHelpers.GetCastlingRights(castlingAvailability);
            var epIdx = enPassantIndex == null ? 0 : BoardStateHelpers.GetEnPassantIndexArchival(enPassantIndex.Value);
            var hmClock = BoardStateHelpers.GetHalfMoveValue(halfMoveClock);
            BoardStateStorage = pieceCaptured | castlingAvail | epIdx | hmClock;
        }
        public uint BoardStateStorage { get; private set; }
    }

    public static class BoardStateHelpers
    {
        public struct BoardStateBitHelpers
        {
            public byte Offset { get; set; }
            public uint Mask { get; set; }
        }
        const string HalfMoveClock = "HMCLOCK";
        private const string CastlingAvail = "CA";
        private const string EPIndex = "EP";
        private const string CapturedPiece = "PIECE";
        private static Dictionary<string, BoardStateBitHelpers> Positions =
            new Dictionary<string, BoardStateBitHelpers>();

        static BoardStateHelpers()
        {
            Positions.Add(HalfMoveClock, new BoardStateBitHelpers()
            {
                Mask = 0b1111_1111,
                Offset = 0
            });
            Positions.Add(EPIndex, new BoardStateBitHelpers()
            {
                Mask = 0b0001_1111_0000_0000,
                Offset = 8
            });
            Positions.Add(CastlingAvail, new BoardStateBitHelpers()
            {
                Mask = 0b1_1110_0000_0000_0000,
                Offset = 13
            });
            Positions.Add(CapturedPiece, new BoardStateBitHelpers()
            {
                Mask = 0b01110_0000_0000_0000_0000,
                Offset = 17
            });
        }
        internal static uint GetCastlingRights(CastlingAvailability ca)
        {
            return (uint)ca << Positions[CastlingAvail].Offset;
        }

        public static CastlingAvailability GetCastlingAvailability(this BoardState boardState)
        {
            var maskedVal = (boardState.BoardStateStorage & Positions[CastlingAvail].Mask) >>
                            Positions[CastlingAvail].Offset;

            return (CastlingAvailability)maskedVal;
        }

        internal static uint GetPieceCaptured(Piece? p)
        {
            var pieceCapturedInfo = Positions[CapturedPiece];
            if (p == null)
            {
                return 0;
            }
            if (p.Value == Piece.King) { throw new ArgumentException("Error archiving board - the King cannot be captured."); }
            var encodedPiece = ((uint)p.Value + 1) << pieceCapturedInfo.Offset;
            return encodedPiece;
        }

        public static Piece? GetPieceCaptured(this BoardState boardState)
        {
            var pieceCapturedInfo = Positions[CapturedPiece];
            var unmasked = (boardState.BoardStateStorage & pieceCapturedInfo.Mask);
            var unOffesetted = unmasked >> pieceCapturedInfo.Offset ;
            return unOffesetted == 0 ? (Piece?)null : (Piece)(unOffesetted - 1);
        }

        internal static byte GetHalfMoveValue(ushort hmClock)
        {
            if (hmClock >= 256)
            {
                throw new ArgumentException("Half move is logically limited to 255, max. Why higher? Do you even draw, bro?");
            }
            return (byte)hmClock;
        }

        public static ushort GetHalfmoveClock(this BoardState boardState)
        {
            var hmInfo = Positions[HalfMoveClock];
            return (ushort)((boardState.BoardStateStorage & hmInfo.Mask) >> hmInfo.Offset);
        }
        internal static uint GetEnPassantIndexArchival(ushort epIndex)
        {
            ValidateEP(epIndex);
            // if the index is on board rank 3, get offset by subtracting 16, else subtract 32 for offset
            var convertedIndex = epIndex < 23 ? (epIndex - 15) : (epIndex - 31);
            return (uint)(convertedIndex << 8);
        }

        public static ushort? GetEnPassantSquare(this BoardState boardState)
        {
            var epInfo = Positions[EPIndex];
            var unMasked = boardState.BoardStateStorage & epInfo.Mask;
            if (unMasked == 0) return null;
            var idx = (ushort)(unMasked >> epInfo.Offset);
            if (idx <= 8)
            {
                return (ushort?)(idx + 15);
            }

            return (ushort?)(idx + 31);
        }

        private static void ValidateEP(ushort? epIndex)
        {
            if (!epIndex.HasValue)
            {
                return;
            }

            var epVal = epIndex.Value.ToBoardValue();
            var isOnRank3 = (epVal & BoardHelpers.RankMasks[2]) != 0;
            var isOnRank6 = (epVal & BoardHelpers.RankMasks[5]) != 0;
            if (isOnRank6 || isOnRank3)
            {
                return;
            }
            throw new ArgumentException("En Passant error. Must be on board rank 3 or 6.");
        }
    }
}
