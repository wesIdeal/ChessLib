using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;
using EnumsNET;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.Helpers")]

namespace ChessLib.Core
{
    public class BoardState : ICloneable
    {
        private const string HalfMoveClockKey = "HMCLOCK";
        private const string CastlingAvailabilityKey = "CA";
        private const string EnPassantIndexKey = "EP";
        private const string PieceCapturedKey = "PIECE";
        private const string GameStateKey = "GAMESTATE";
        private const string ActivePlayerKey = "ACTIVEPLAYER";
        private const string FullMoveCounterKey = "FULLMOVECOUNT";

        private static readonly Dictionary<string, BoardStateBitHelpers> Positions =
            new Dictionary<string, BoardStateBitHelpers>();


        private readonly bool _bypassValidation;

        static BoardState()
        {
            InitializeMasks();
        }
        internal BoardState()
        {
            BoardStateStorage = 0;
            
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
        protected BoardState(byte halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece,
            CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter,
            GameState gameState = GameState.None) : this()
        {
            SetBoardState(halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, activePlayer,
                fullMoveCounter, gameState);
        }

        protected BoardState(string strFen, bool bypassValidation) : this()
        {
            _bypassValidation = bypassValidation;
            var fen = new FenTextToBoard() { BypassValidation = bypassValidation }.Translate(strFen);
            SetBoardState(fen.HalfMoveClock, fen.EnPassantIndex, null, fen.CastlingAvailability, fen.ActivePlayer,
                fen.FullMoveCounter);
        }

        public BoardState(uint boardStateStorage) :this()
        {
            BoardStateStorage = boardStateStorage;
            
        }



        public bool IsEndOfGame
        {
            get
            {
                var endOfGameStates = new[] { GameState.Checkmate, GameState.StaleMate };
                return endOfGameStates.Contains(GameState);
            }
        }


        /// <summary>
        ///     FIELD               [bit index, from smallest]
        ///     Half MoveValue Clock     [Bits 0 - 7]
        ///     En Passant Index    [Bits 8 - 12] 0-based index for board ranks 3 and 6, A3-H3, A6-H6
        ///     Castling Rights     [Bits 13 - 15]
        ///     Captured Piece      [Bits 17 - 19] 1-5 (pawn - Queen, 0 for none)
        ///     GameState           [Bits 20 - 21]
        ///     Full MoveValue Count     [Bits 22 - 30]
        ///     Active Color        [Bit  31]
        /// </summary>
        public uint BoardStateStorage { get; set; }

        public GameState GameState
        {
            get
            {
                var gsInfo = Positions[GameStateKey];
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
                var maskedVal = (BoardStateStorage & Positions[CastlingAvailabilityKey].Mask) >>
                                Positions[CastlingAvailabilityKey].Offset;
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
                var activePlayerInfo = Positions[ActivePlayerKey];
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
                var pieceCapturedInfo = Positions[PieceCapturedKey];
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

        public ushort? EnPassantIndex
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


        public byte HalfMoveClock
        {
            get
            {
                var hmInfo = Positions[HalfMoveClockKey];
                return (byte)((BoardStateStorage & hmInfo.Mask) >> hmInfo.Offset);
            }
            set
            {
                var clearValue = GetClearValue(HalfMoveClockKey) & BoardStateStorage;
                var storageValue = value;
                BoardStateStorage = clearValue | storageValue;
            }
        }

        public uint FullMoveCounter
        {
            get
            {
                var maskEntry = Positions[FullMoveCounterKey];
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

        private uint GetEnPassantValueFromStorage()
        {
            var epInfo = Positions[EnPassantIndexKey];
            var unMasked = BoardStateStorage & epInfo.Mask;
            var idx = (ushort)(unMasked >> epInfo.Offset);
            return idx;
        }

        internal virtual void ValidateEnPassantSquare(ushort? value)
        {
            if (_bypassValidation)
            {
                return;
            }
            if (value != null)
            {
                var enPassantIndexValidator = new EnPassantSquareIndexRule();
                var valid = enPassantIndexValidator.Validate(value, ActivePlayer) == BoardExceptionType.None;
                if (!valid)
                {
                    var square = value.Value.IndexToSquareDisplay();
                    var message =
                        $"{square} is not a valid en passant square with {ActivePlayer.AsString()} to play. Found in BoardState.ValidateEnPassant.";
                    throw new BoardException(BoardExceptionType.BadEnPassant,
                        message);
                }
            }
        }

        public static void InitializeMasks()
        {
            Positions.Add(HalfMoveClockKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_0000_0000_0000_1111_1111,
                Offset = 0
            });
            Positions.Add(EnPassantIndexKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_0000_0001_1111_0000_0000,
                Offset = 8
            });
            Positions.Add(CastlingAvailabilityKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_0001_1110_0000_0000_0000,
                Offset = 13
            });
            Positions.Add(PieceCapturedKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0000_1110_0000_0000_0000_0000,
                Offset = 17
            });
            Positions.Add(GameStateKey, new BoardStateBitHelpers
            {
                Mask = 0b0000_0000_0011_0000_0000_0000_0000_0000,
                Offset = 20
            });
            Positions.Add(FullMoveCounterKey, new BoardStateBitHelpers
            {
                Mask = 0b0111_1111_1100_0000_0000_0000_0000_0000,
                Offset = 22
            });
            Positions.Add(ActivePlayerKey, new BoardStateBitHelpers
            {
                Mask = 0b1000_0000_0000_0000_0000_0000_0000_0000,
                Offset = 31
            });
        }

        private uint GetGameStateStorageValue(GameState state)
        {
            return (uint)state << Positions[GameStateKey].Offset;
        }

        private uint GetClearValue(string key)
        {
            return ~Positions[key].Mask;
        }


        private uint GetActivePlayerStorageValue(Color activePlayerColor)
        {
            return (uint)activePlayerColor << Positions[ActivePlayerKey].Offset;
        }

        private uint GetCastlingRightsStorageValue(CastlingAvailability ca)
        {
            return (uint)ca << Positions[CastlingAvailabilityKey].Offset;
        }

        private uint GetPieceCapturedStorageValue(Piece? p)
        {
            var pieceCapturedInfo = Positions[PieceCapturedKey];
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


        private uint GetFullMoveCounterStorageValue(uint fullMoveCounter)
        {
            if (fullMoveCounter > 0b0001_1111_1111)
            {
                throw new FullMoveCountExceededException(fullMoveCounter);
            }

            return fullMoveCounter << Positions[FullMoveCounterKey].Offset;
        }


        private void SetBoardState(byte halfMoveClock, ushort? enPassantIndex, Piece? pieceCaptured,
            CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter,
            GameState gameState = GameState.None)
        {
            ActivePlayer = activePlayer;
            PieceCaptured = pieceCaptured;
            CastlingAvailability = castlingAvailability;
            EnPassantIndex = enPassantIndex;
            HalfMoveClock = halfMoveClock;
            GameState = gameState;
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

        public object Clone()
        {
            return new BoardState(BoardStateStorage);
        }
    }

    public struct BoardStateBitHelpers
    {
        public byte Offset { get; set; }
        public uint Mask { get; set; }
    }
}