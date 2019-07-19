using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChessLib.Data.Annotations;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Validators.BoardValidation;

namespace ChessLib.Data.Boards
{
    public abstract class BoardBase : INotifyPropertyChanged, IBoard
    {
        protected ulong[][] PiecePlacement;
        private Color _activePlayer;

        protected BoardBase() { }

        public GameState GameState { get; set; } = GameState.None;

        public bool IsGameOver => GameState == GameState.Checkmate || GameState == GameState.StaleMate;

        protected BoardBase(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability, ushort? enPassantIdx, uint? halfMoveClock, uint fullMoveCount)
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
            PiecePlacement = fen.BoardFromFen(out Color active, out CastlingAvailability ca, out ushort? enPassant, out uint hmClock, out uint fmClock);
            ActivePlayer = active;
            CastlingAvailability = ca;
            EnPassantSquare = enPassant;
            HalfmoveClock = hmClock;
            FullmoveCounter = fmClock;
            Chess960 = is960;

        }

        public void ValidateBoard()
        {
            var boardValidator = new BoardValidator(this);
            var exceptionType = boardValidator.Validate(false);
            switch (exceptionType)
            {
                case BoardExceptionType.None:
                    return;
                case BoardExceptionType.Checkmate:
                    GameState = GameState.Checkmate;
                    break;
                case BoardExceptionType.Stalemate:
                    GameState = GameState.StaleMate;
                    break;
                case BoardExceptionType.MaterialDraw:
                    GameState = GameState.Drawn;
                    break;
                default:
                    throw BoardException.MakeBoardException(exceptionType);
            }
        }

        public ulong[][] GetPiecePlacement()
        {
            return PiecePlacement;
        }

        public Color ActivePlayer
        {
            get => _activePlayer;
            set
            {
                _activePlayer = value;
                OnPropertyChanged(nameof(ActivePlayer));
            }
        }

        public CastlingAvailability CastlingAvailability { get; set; }
        public ushort? EnPassantSquare { get; set; }
        public uint? HalfmoveClock { get; set; }
        public uint FullmoveCounter { get; set; }
        public bool Chess960 { get; protected set; }
        public string InitialFEN { get; protected set; }

        public ulong ActiveOccupancy => GetPiecePlacement().Occupancy(ActivePlayer);
        public ulong OpponentOccupancy => GetPiecePlacement().Occupancy(this.OpponentColor());
        public ulong TotalOccupancy => GetPiecePlacement().Occupancy();

        public ushort ActivePlayerKingIndex => GetPiecePlacement().Occupancy(ActivePlayer, Piece.King).GetSetBits()[0];
        public ulong ActivePlayerOccupancy(Piece p) => GetPiecePlacement().Occupancy(ActivePlayer, p);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract object Clone();
    }
}
