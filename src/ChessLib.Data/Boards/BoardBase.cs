using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChessLib.Data.Annotations;
using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Validators.BoardValidation;

namespace ChessLib.Data.Boards
{
    public abstract class BoardBase : IBoard
    {
        protected ulong[][] PiecePlacement;
        private Color _activePlayer;
        protected BoardBase() { }

        protected BoardBase(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability, ushort? enPassantIdx, ushort halfMoveClock, ushort fullMoveCount)
        {
            PiecePlacement = occupancy;
            ActivePlayer = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassantSquare = enPassantIdx;
            HalfmoveClock = halfMoveClock;
            FullmoveCounter = fullMoveCount;
        }

        protected BoardBase(string fen, bool is960)
        {
            InitialFEN = fen;
            PiecePlacement = fen.BoardFromFen(out Color active, out CastlingAvailability ca, out ushort? enPassantSquare, out ushort hmClock, out ushort fmClock);
            ActivePlayer = active;
            CastlingAvailability = ca;
            EnPassantSquare = enPassantSquare;
            HalfmoveClock = hmClock;
            FullmoveCounter = fmClock;
            Chess960 = is960;

        }

        public GameState ValidateBoard()
        {
            var boardValidator = new BoardValidator(this);
            var exceptionType = boardValidator.Validate(false);
            var gameState = GameState.None;
            switch (exceptionType)
            {
                case BoardExceptionType.None:
                    gameState = GameState.None;
                    break;
                case BoardExceptionType.Checkmate:
                    gameState = GameState.Checkmate;
                    break;
                case BoardExceptionType.Stalemate:
                    gameState = GameState.StaleMate;
                    break;
                case BoardExceptionType.MaterialDraw:
                    gameState = GameState.Drawn;
                    break;
                default:
                    throw BoardException.MakeBoardException(exceptionType);
            }
            return gameState;
        }

        public ulong[][] GetPiecePlacement()
        {
            return PiecePlacement;
        }

        public void SetPiecePlacement(ulong[][] piecePlacement)
        {
            PiecePlacement = piecePlacement;
        }

        public virtual Color ActivePlayer
        {
            get => _activePlayer;
            set => _activePlayer = value;
        }
        public bool IsActiveColorInCheck => PiecePlacement.IsPlayerInCheck((int)ActivePlayer);
        public Color OpponentColor => _activePlayer.Toggle();
        public object GetPseudoLegalMoves(ushort sqIndex, out List<MoveExt> legalMoves) => Bitboard.GetPseudoLegalMoves(this, sqIndex, out legalMoves);
        public CastlingAvailability CastlingAvailability { get; set; }
        public ushort? EnPassantSquare { get; set; }
        public ushort HalfmoveClock { get; set; }
        public ushort FullmoveCounter { get; set; }
        public bool Chess960 { get; protected set; }
        public string InitialFEN { get; protected set; }
        private BoardState _initialBoardState = null;
        public BoardState InitialBoardState
        {
            get
            {
                return _initialBoardState ?? (_initialBoardState = BoardState.FromFEN(InitialFEN));

            }
        }
        public ulong ActiveOccupancy => GetPiecePlacement().Occupancy(ActivePlayer);
        public ulong OpponentOccupancy => GetPiecePlacement().Occupancy(this.OpponentColor());
        public ulong TotalOccupancy => GetPiecePlacement().Occupancy();

        public ushort ActivePlayerKingIndex => GetPiecePlacement().Occupancy(ActivePlayer, Piece.King).GetSetBits()[0];
        public ulong ActivePlayerOccupancy(Piece p) => GetPiecePlacement().Occupancy(ActivePlayer, p);



        public abstract object Clone();
    }
}
