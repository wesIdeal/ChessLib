using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Validators.BoardValidation.Rules;
using EnumsNET;

namespace ChessLib.Data.Boards
{
    public class BoardState : IBoardState, IEquatable<BoardState>
    {
        /// <summary>
        ///     Makes an archival board state
        /// </summary>
        /// <param name="halfMoveClock">Half move clock. Must be less than 256</param>
        /// <param name="enPassantIndex">Board index (a1 = 0, h8 = 63) of the EP square, if any</param>
        /// <param name="capturedPiece">The piece that was captured, if any</param>
        /// <param name="castlingAvailability">Castling Availability</param>
        /// <param name="gameState">Game state of current board. </param>
        /// <param name="activePlayer">Player to move</param>
        /// <param name="fullMoveCounter">Number of whole moves played</param>
        public BoardState(ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece,
            CastlingAvailability castlingAvailability, GameState gameState, Color activePlayer, ushort fullMoveCounter)
        {
            SetBoardState(halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, gameState, activePlayer,
                fullMoveCounter);
        }

        protected BoardState(string fen)
        {
            var board = fen.BoardFromFen(out var color, out var castingAvailability,
                out var enPassantSquare, out var halfmove, out var fullmove);
            SetBoardState(board, halfmove, enPassantSquare, null, castingAvailability, color, fullmove);
        }

        private BoardState(uint boardStateStorage)
        {
            BoardStateStorage = boardStateStorage;
        }

        protected BoardState(ulong[][] board, ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece, CastlingAvailability castlingAvailability, Color activePlayer, ushort fullMoveCounter)
        {
            SetBoardState(board, halfMoveClock, enPassantIndex, null, castlingAvailability, activePlayer, fullMoveCounter);
        }

        protected virtual GameState GameStateFromBoard(ulong[][] board, Color activeColor, ushort? enPassantIdx,
            CastlingAvailability castlingAvailability)
        {
            var gameStateValidator = new EndOfGameRule();
            var gameState = gameStateValidator.Validate(board, activeColor, enPassantIdx, castlingAvailability);
            switch (gameState)
            {
                case BoardExceptionType.MaterialDraw: return GameState.Drawn;
                case BoardExceptionType.Checkmate: return GameState.Checkmate;
                case BoardExceptionType.Stalemate: return GameState.StaleMate;
                default: return GameState.None;
            }
        }


        public bool Equals(IBoardState other)
        {
            if (other == null) return false;
            return BoardStateStorage == other.BoardStateStorage;
        }


        public uint BoardStateStorage { get; set; }

        public GameState GameState => this.GetGameState();

        /// <summary>
        ///     Enumeration for all castling-moves available on the board.
        /// </summary>
        public CastlingAvailability CastlingAvailability => this.GetCastlingAvailability();

        public Piece? PieceCaptured => this.GetPieceCaptured();

        public ushort? EnPassantSquare => this.GetEnPassantSquare();

        public ushort HalfMoveClock => this.GetHalfmoveClock();

        public ushort FullMoveCounter { get; set; }

        /// <summary>
        ///     Represents the player who is about to move.
        /// </summary>
        public Color ActivePlayer => this.GetActivePlayer();

        public bool Equals(BoardState other)
        {
            if (other == null)
            {
                return false;
            }

            return BoardStateStorage == other.BoardStateStorage;
        }

        private void SetBoardState(ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece,
            CastlingAvailability castlingAvailability, GameState gameState, Color activePlayer, ushort fullMoveCounter)
        {
            var pieceCaptured = BoardStateHelpers.GetPieceCaptured(capturedPiece);
            var castlingAvail = BoardStateHelpers.GetCastlingRights(castlingAvailability);
            var epIdx = enPassantIndex == null ? 0 : BoardStateHelpers.GetEnPassantIndexArchival(enPassantIndex.Value);
            var hmClock = BoardStateHelpers.GetHalfMoveValue(halfMoveClock);
            var gmState = BoardStateHelpers.GetGameState(gameState);
            var color = BoardStateHelpers.GetActivePlayerStorageValue(activePlayer);
            var fullMoveCount = BoardStateHelpers.GetFullMoveCounterValue(fullMoveCounter);
            BoardStateStorage = pieceCaptured | castlingAvail | epIdx | hmClock | gmState | color | fullMoveCount;
        }

        private void SetBoardState(ulong[][] board, ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece, CastlingAvailability castlingAvailability, Color activePlayer, ushort fullMoveCounter)
        {
            GameState gameState = GameStateFromBoard(board, activePlayer, enPassantIndex, castlingAvailability);
            SetBoardState(halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, gameState, activePlayer, fullMoveCounter);
        }

        public static implicit operator uint(BoardState boardState) => boardState.BoardStateStorage;
        public static explicit operator BoardState(uint boardState) => new BoardState(boardState);

        public override bool Equals(object obj)
        {
            var other = obj as BoardState;
            return obj != null &&
                   other != null &&
                   BoardStateStorage == other.BoardStateStorage;
        }

        public override int GetHashCode()
        {
            return BoardStateStorage.GetHashCode();
        }

        public override string ToString()
        {

            var sb = new StringBuilder();
            sb.AppendLine($"BoardState (int): {this.BoardStateStorage}");
            sb.AppendLine($"Active Player: {this.GetActivePlayer().AsString()}");
            sb.AppendLine($"Halfmove Clock: {this.GetHalfmoveClock()}");
            sb.AppendLine($"Castling: {this.GetCastlingAvailability()}");
            sb.AppendLine($"En Passant: {this.GetEnPassantSquare()}");
            sb.AppendLine($"Captured Piece: {this.GetPieceCaptured()}");
            return sb.ToString();
        }

        public object Clone()
        {
            return new BoardState(BoardStateStorage);
        }
    }

    internal static class BoardStateHelpers
    {
        private const string HalfMoveClock = "HMCLOCK";
        private const string CastlingAvail = "CA";
        private const string EPIndex = "EP";
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
            Positions.Add(EPIndex, new BoardStateBitHelpers
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

        public static void ToggleActivePlayer(this BoardState boardState)
        {
            var mask = Positions[ActivePlayer].Mask;
            boardState.BoardStateStorage ^= mask;
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
            var unOffesetted = unmasked >> pieceCapturedInfo.Offset;
            return unOffesetted == 0 ? (Piece?)null : (Piece)(unOffesetted - 1);
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
            ValidateEP(epIndex);
            // if the index is on board rank 3, get offset by subtracting 16, else subtract 32 for offset
            var convertedIndex = epIndex < 23 ? epIndex - 15 : epIndex - 31;
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

        public static uint GetFullMoveCounterValue(ushort fullMoveCounter)
        {
            if (fullMoveCounter > 0b0001_1111_1111)
            {
                throw new FullMoveCountExceededException(fullMoveCounter);
            }

            return (uint)fullMoveCounter << Positions[FullMoveCounter].Offset;
        }

        public struct BoardStateBitHelpers
        {
            public byte Offset { get; set; }
            public uint Mask { get; set; }
        }
    }
}
