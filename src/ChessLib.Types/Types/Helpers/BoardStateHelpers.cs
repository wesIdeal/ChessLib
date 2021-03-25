using System;
using System.Collections.Generic;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;

namespace ChessLib.Core.Types.Helpers
{
    internal static class BoardStateHelpers
    {
        private const string HalfMoveClock = "HMCLOCK";
        private const string CastlingAvail = "CA";
        private const string EnPassantIndex = "EP";
        private const string CapturedPiece = "PIECE";
        private const string GameState = "GAMESTATE";
        private const string ActivePlayer = "ACTIVEPLAYER";
        private const string FullMoveCounter = "FULLMOVECOUNT";


        private static readonly Dictionary<string, BoardStateBitHelpers> Positions =
            new Dictionary<string, BoardStateBitHelpers>();

        static BoardStateHelpers()
        {
            Positions.Add(HalfMoveClock, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_0000_0000_0000_1111_1111,
                Offset = 0
            });
            Positions.Add(EnPassantIndex, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_0000_0001_1111_0000_0000,
                Offset = 8
            });
            Positions.Add(CastlingAvail, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_0001_1110_0000_0000_0000,
                Offset = 13
            });
            Positions.Add(CapturedPiece, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_1110_0000_0000_0000_0000,
                Offset = 17
            });
            Positions.Add(GameState, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0011_0000_0000_0000_0000_0000,
                Offset = 20
            });
            Positions.Add(FullMoveCounter, new BoardStateBitHelpers
            {
                Mask = 0b0111_1111_1100_0000_0000_0000_0000_0000,
                Offset = 22
            });
            Positions.Add(ActivePlayer, new BoardStateBitHelpers
            {
                Mask = 0b1000_0000_0000_0000_0000_0000_0000_0000,
                Offset = 31
            });
        }

        public static Color GetActivePlayer(this BoardState boardState)
        {
            var activePlayerInfo = Positions[ActivePlayer];
            var unmasked = (boardState.BoardStateStorage & activePlayerInfo.Mask) >> activePlayerInfo.Offset;
            return (Color)unmasked;
        }

        public static uint GetActivePlayerStorageValue(Color activePlayerColor)
        {
            return (uint)activePlayerColor << Positions[ActivePlayer].Offset;
        }

        internal static uint GetGameState(GameState state)
        {
            return (uint)state << Positions[GameState].Offset;
        }


        public static GameState GetGameState(this BoardState boardState)
        {
            var gsInfo = Positions[GameState];
            var unmasked = (boardState.BoardStateStorage & gsInfo.Mask) >> gsInfo.Offset;
            return (GameState)unmasked;
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

            if (p.Value == Piece.King)
            {
                throw new ArgumentException("Error archiving board - the King cannot be captured.");
            }

            var encodedPiece = ((uint)p.Value + 1) << pieceCapturedInfo.Offset;
            return encodedPiece;
        }

        public static Piece? GetPieceCaptured(this BoardState boardState)
        {
            var pieceCapturedInfo = Positions[CapturedPiece];
            var unmasked = boardState.BoardStateStorage & pieceCapturedInfo.Mask;
            var notOffset = unmasked >> pieceCapturedInfo.Offset;
            return notOffset == 0 ? (Piece?)null : (Piece)(notOffset - 1);
        }

        internal static byte GetHalfMoveValue(ushort hmClock)
        {
            if (hmClock >= 256)
            {
                throw new ArgumentException(
                    "Half move is logically limited to 255, max. Why higher? Do you even draw, bro?");
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
            ValidateEnPassant(epIndex);
            // if the index is on board rank 3, get offset by subtracting 16, else subtract 32 for offset
            var convertedIndex = epIndex <= 23 ? epIndex - 15 : epIndex - 31;
            return (uint)(convertedIndex << 8);
        }

        public static ushort? GetEnPassantSquare(this BoardState boardState)
        {
            var epInfo = Positions[EnPassantIndex];
            var unMasked = boardState.BoardStateStorage & epInfo.Mask;
            if (unMasked == 0) return null;
            var idx = (ushort)(unMasked >> epInfo.Offset);
            if (idx <= 8)
            {
                return (ushort?)(idx + 15);
            }

            return (ushort?)(idx + 31);
        }

        private static void ValidateEnPassant(ushort? epIndex)
        {
            if (!epIndex.HasValue)
            {
                return;
            }

            var epVal = epIndex.Value.GetBoardValueOfIndex();
            var isOnRank3 = (epVal & BoardConstants.RankMasks[2]) != 0;
            var isOnRank6 = (epVal & BoardConstants.RankMasks[5]) != 0;
            if (isOnRank6 || isOnRank3)
            {
                return;
            }

            throw new ArgumentException("En Passant error. Must be on board rank 3 or 6.");
        }

        internal static uint GetFullMoveCounterValue(uint fullMoveCounter)
        {
            if (fullMoveCounter > 0b0001_1111_1111)
            {
                throw new FullMoveCountExceededException(fullMoveCounter);
            }

            return fullMoveCounter << Positions[FullMoveCounter].Offset;
        }

        public static uint GetFullMoveCounter(this BoardState boardState)
        {
            var maskEntry = Positions[FullMoveCounter];
            var maskedValue = boardState.BoardStateStorage & maskEntry.Mask;
            var value = maskedValue >> maskEntry.Offset;
            return value;
        }

        public struct BoardStateBitHelpers
        {
            public byte Offset { get; set; }
            public uint Mask { get; set; }
        }
    }
}
