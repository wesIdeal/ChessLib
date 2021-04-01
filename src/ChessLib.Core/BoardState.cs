using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;
using EnumsNET;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Helpers")]

namespace ChessLib.Core
{
    public class BoardState : IBoardState, IEquatable<BoardState>
    {
        private const string HalfMoveClockKey = "HMCLOCK";
        private const string CastlingAvailabilityKey = "CA";
        private const string EnPassantIndexKey = "EP";
        private const string PieceCapturedKey = "PIECE";
        private const string GameStateKey = "GAMESTATE";
        private const string ActivePlayerKey = "ACTIVEPLAYER";
        private const string FullMoveCounterKey = "FULLMOVECOUNT";

        private readonly Dictionary<string, BoardStateBitHelpers> _positions =
            new Dictionary<string, BoardStateBitHelpers>();

        protected EndOfGameRule EndOfGameRule = new EndOfGameRule();
        public IEnPassantSquareRule EnPassantSquareRule;

        public BoardState()
        {
            BoardStateStorage = 0;
            InitializeMasks();
        }
        /// <summary>
        ///     Makes an archival board state
        /// </summary>
        /// <param name="halfMoveClock">Half move clock. Must be less than 256</param>
        /// <param name="enPassantIndex">Board index (a1 = 0, h8 = 63) of the EP square, if any</param>
        /// <param name="capturedPiece">The piece that was captured, if any</param>
        /// <param name="castlingAvailability">Castling Availability</param>
        /// <param name="activePlayer">Player to move</param>
        /// <param name="fullMoveCounter">Number of whole moves played</param>
        /// <param name="gameState">Game state of current board. </param>
        public BoardState(ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece,
            CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter,
            GameState gameState = GameState.None) : this()
        {
            SetBoardState(halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, activePlayer,
                fullMoveCounter, gameState);
        }

        protected BoardState(string fen) : this()
        {
            fen.BoardFromFen(out var color, out var castingAvailability,
                out var enPassantSquare, out var halfmove, out var fullmove, false);
            SetBoardState(halfmove, enPassantSquare, null, castingAvailability, color, fullmove);
        }

        private BoardState(uint boardStateStorage)
        {
            BoardStateStorage = boardStateStorage;
        }


        public bool Equals(IBoardState other)
        {
            if (other == null) return false;
            return BoardStateStorage == other.BoardStateStorage;
        }


        public uint BoardStateStorage { get; set; }

        public GameState GameState
        {
            get
            {
                var gsInfo = _positions[GameStateKey];
                var unmasked = (BoardStateStorage & gsInfo.Mask) >> gsInfo.Offset;
                return (GameState)unmasked;
            }
            set
            {
                var clearValue = GetClearValue(GameStateKey) & BoardStateStorage;
                var storageValue = GetGameStateStorageValue(value);
                BoardStateStorage = clearValue | storageValue;
            }
        }

        /// <summary>
        ///     Enumeration for all castling-moves available on the board.
        /// </summary>
        public CastlingAvailability CastlingAvailability
        {
            get
            {
                var maskedVal = (BoardStateStorage & _positions[CastlingAvailabilityKey].Mask) >>
                                _positions[CastlingAvailabilityKey].Offset;
                return (CastlingAvailability)maskedVal;
            }
            set
            {
                var clearValue = GetClearValue(CastlingAvailabilityKey) & BoardStateStorage;
                var storageValue = GetCastlingRightsStorageValue(value);
                BoardStateStorage = clearValue | storageValue;
            }
        }

        public Color ActivePlayer
        {
            get
            {
                var activePlayerInfo = _positions[ActivePlayerKey];
                var unmasked = (BoardStateStorage & activePlayerInfo.Mask) >> activePlayerInfo.Offset;
                return (Color)unmasked;
            }
            set
            {
                var clearValue = GetClearValue(ActivePlayerKey) & BoardStateStorage;
                var storageValue = GetActivePlayerStorageValue(value);
                BoardStateStorage = clearValue | storageValue;
            }
        }

        public Piece? PieceCaptured
        {
            get
            {
                var pieceCapturedInfo = _positions[PieceCapturedKey];
                var unmasked = BoardStateStorage & pieceCapturedInfo.Mask;
                var notOffset = unmasked >> pieceCapturedInfo.Offset;
                return notOffset == 0 ? (Piece?)null : (Piece)(notOffset - 1);
            }
            set
            {
                var clearValue = GetClearValue(PieceCapturedKey) & BoardStateStorage;
                var storageValue = GetPieceCapturedStorageValue(value);
                BoardStateStorage = clearValue | storageValue;
            }
        }

        public virtual ushort? EnPassantIndex
        {
            get
            {
                var idx = GetEnPassantValueFromStorage();
                if (idx == 0)
                {
                    return null;
                }

                if (idx <= 8)
                {
                    return (ushort?)(idx + 15);
                }

                return (ushort?)(idx + 31);
            }
            set
            {
                ValidateEnPassantSquare(value);
                var clearValue = GetClearValue(EnPassantIndexKey);
                var clearedEnPassantValue = clearValue & BoardStateStorage;
                var storageValue = GetEnPassantStorageValue(value);
                BoardStateStorage = clearedEnPassantValue | storageValue;
            }
        }

        private uint GetEnPassantValueFromStorage()
        {
            var epInfo = _positions[EnPassantIndexKey];
            var unMasked = BoardStateStorage & epInfo.Mask;
            var idx = (ushort)(unMasked >> epInfo.Offset);
            return idx;
        }

        private void ValidateEnPassantSquare(ushort? value)
        {
            var enPassantValidator = EnPassantSquareRule ?? new EnPassantSquareRule();
            if (!enPassantValidator.IsValidEnPassantSquare(value, ActivePlayer))
            {
                var square = value.HasValue ? value.Value.IndexToSquareDisplay() : "[null]";
                var message = $"{square} is not a valid en passant square. Found in BoardState.ValidateEnPassant.";
                throw new BoardException(BoardExceptionType.BadEnPassant,
                    message);
            }
        }


        public ushort HalfMoveClock
        {
            get
            {
                var hmInfo = _positions[HalfMoveClockKey];
                return (ushort)((BoardStateStorage & hmInfo.Mask) >> hmInfo.Offset);
            }
            set
            {
                var clearValue = GetClearValue(HalfMoveClockKey) & BoardStateStorage;
                var storageValue = GetHalfMoveStorageValue(value);
                BoardStateStorage = clearValue | storageValue;
            }
        }

        public uint FullMoveCounter
        {
            get
            {
                var maskEntry = _positions[FullMoveCounterKey];
                var maskedValue = BoardStateStorage & maskEntry.Mask;
                var value = maskedValue >> maskEntry.Offset;
                return value;
            }
            set
            {
                var clearValue = GetClearValue(FullMoveCounterKey) & BoardStateStorage;
                var storageValue = GetFullMoveCounterStorageValue(value);
                BoardStateStorage = clearValue | storageValue;
            }
        }


        /// <summary>
        ///     Represents the player who is about to move.
        /// </summary>
        public bool Equals(BoardState other)
        {
            if (other == null)
            {
                return false;
            }

            return BoardStateStorage == other.BoardStateStorage;
        }

        public void InitializeMasks()
        {
            _positions.Add(HalfMoveClockKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_0000_0000_0000_1111_1111,
                Offset = 0
            });
            _positions.Add(EnPassantIndexKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_0000_0001_1111_0000_0000,
                Offset = 8
            });
            _positions.Add(CastlingAvailabilityKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_0001_1110_0000_0000_0000,
                Offset = 13
            });
            _positions.Add(PieceCapturedKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_1110_0000_0000_0000_0000,
                Offset = 17
            });
            _positions.Add(GameStateKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0011_0000_0000_0000_0000_0000,
                Offset = 20
            });
            _positions.Add(FullMoveCounterKey, new BoardStateBitHelpers
            {
                Mask = 0b0111_1111_1100_0000_0000_0000_0000_0000,
                Offset = 22
            });
            _positions.Add(ActivePlayerKey, new BoardStateBitHelpers
            {
                Mask = 0b1000_0000_0000_0000_0000_0000_0000_0000,
                Offset = 31
            });
        }

        private uint GetGameStateStorageValue(GameState state)
        {
            return (uint)state << _positions[GameStateKey].Offset;
        }

        private uint GetClearValue(string key)
        {
            return ~_positions[key].Mask;
        }


        private uint GetActivePlayerStorageValue(Color activePlayerColor)
        {
            return (uint)activePlayerColor << _positions[ActivePlayerKey].Offset;
        }

        private uint GetCastlingRightsStorageValue(CastlingAvailability ca)
        {
            return (uint)ca << _positions[CastlingAvailabilityKey].Offset;
        }

        private uint GetPieceCapturedStorageValue(Piece? p)
        {
            var pieceCapturedInfo = _positions[PieceCapturedKey];
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


        internal uint GetEnPassantStorageValue(ushort? epIndex)
        {
            if (!epIndex.HasValue)
            {
                return 0;
            }
            // if the index is on board rank 3, get offset by subtracting 16, else subtract 32 for offset
            var convertedIndex = epIndex <= 23 ? epIndex - 15 : epIndex - 31;
            var value = (uint)(convertedIndex << 8);
            return value;
        }

        private byte GetHalfMoveStorageValue(ushort hmClock)
        {
            if (hmClock >= 256)
            {
                throw new ArgumentException(
                    "Half move is logically limited to 255, max. Time to draw.");
            }

            return (byte)hmClock;
        }

        private uint GetFullMoveCounterStorageValue(uint fullMoveCounter)
        {
            if (fullMoveCounter > 0b0001_1111_1111)
            {
                throw new FullMoveCountExceededException(fullMoveCounter);
            }

            return fullMoveCounter << _positions[FullMoveCounterKey].Offset;
        }


        private void SetBoardState(ushort halfMoveClock, ushort? enPassantIndex, Piece? pieceCaptured,
            CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter,
            GameState gameState = GameState.None)
        {
            PieceCaptured = pieceCaptured;
            CastlingAvailability = castlingAvailability;
            EnPassantIndex = null;
            HalfMoveClock = halfMoveClock;
            GameState = gameState;
            ActivePlayer = activePlayer;
            FullMoveCounter = fullMoveCounter;
        }

        public static implicit operator uint(BoardState boardState)
        {
            return boardState.BoardStateStorage;
        }

        public static explicit operator BoardState(uint boardState)
        {
            return new BoardState(boardState);
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"BoardState (int): {BoardStateStorage}");
            sb.AppendLine($"Active Player: {ActivePlayer.AsString()}");
            sb.AppendLine($"Halfmove Clock: {HalfMoveClock}");
            sb.AppendLine($"Castling: {CastlingAvailability}");
            sb.AppendLine($"En Passant: {EnPassantIndex}");
            sb.AppendLine($"Captured Piece: {PieceCaptured}");
            return sb.ToString();
        }
    }

    public struct BoardStateBitHelpers
    {
        public byte Offset { get; set; }
        public uint Mask { get; set; }
    }
}