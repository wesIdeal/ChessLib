using System;
using System.Text;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Helpers;
using ChessLib.Core.Types.Interfaces;
using EnumsNET;

namespace ChessLib.Core
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
            CastlingAvailability castlingAvailability, GameState gameState, Color activePlayer, uint fullMoveCounter)
        {
            SetBoardState(halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, gameState, activePlayer,
                fullMoveCounter);
        }

        protected BoardState(string fen)
        {
            var board = fen.BoardFromFen(out var color, out var castingAvailability,
                out var enPassantSquare, out var halfmove, out var fullmove, false);
            SetBoardState(board, halfmove, enPassantSquare, null, castingAvailability, color, fullmove);
        }

        private BoardState(uint boardStateStorage)
        {
            BoardStateStorage = boardStateStorage;
        }

        protected BoardState(ulong[][] board, ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece, CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter)
        {
            SetBoardState(board, halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, activePlayer, fullMoveCounter);
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

        public uint FullMoveCounter => this.GetFullMoveCounter();

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
            CastlingAvailability castlingAvailability, GameState gameState, Color activePlayer, uint fullMoveCounter)
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

        private void SetBoardState(ulong[][] board, ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece, CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter)
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
}