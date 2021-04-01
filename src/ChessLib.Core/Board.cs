using System;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;

namespace ChessLib.Core
{
    public class Board : BoardState, IEquatable<Board>, IBoard
    {
        public Board() : base(FENHelpers.FENInitial)
        {
            Occupancy = new ulong[2][];
            InitializeOccupancy();
        }

        /// <summary>
        ///     Construct Board from FEN
        /// </summary>
        /// <param name="fen">fen string </param>
        public Board(string fen) :  base(fen)
        {
            Occupancy = new ulong[2][];
            Occupancy = FENHelpers.BoardFromFen(fen, out _, out _, out ushort? enPassantIndex, out _, out _);
            EnPassantIndex = enPassantIndex;

        }

        public Board(ulong[][] occupancy, ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece,
            CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter)
            : base(halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, activePlayer, fullMoveCounter)
        {
            Occupancy = new ulong[2][];
            Occupancy = CloneOccupancy(occupancy);
            GameState = GameStateFromBoard();
        }

        public ulong[] BlackOccupancy => Occupancy[(int)Color.Black];
        public ulong[] WhiteOccupancy => Occupancy[(int)Color.White];
        public Color OpponentColor => ActivePlayer.Toggle();

        public string CurrentFEN => this.FENFromBoard();

        public new object Clone()
        {
            Console.WriteLine($"Total occupancy: {Occupancy.Occupancy()}");
            var clonedOccupancy = CloneOccupancy();
            var activePlayerColor = ActivePlayer;
            return new Board(clonedOccupancy, HalfMoveClock, EnPassantIndex, PieceCaptured, CastlingAvailability,
                activePlayerColor, FullMoveCounter);
        }

        public override ushort? EnPassantIndex
        {
            get => base.EnPassantIndex;
            set
            {
                ValidateEnPassant(value);
                base.EnPassantIndex = value;
            }
        }

        public ulong[][] Occupancy { get; }


        public bool Equals(Board other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var boardStateEquality = base.Equals(other);
            var whitePiecesEquality = Occupancy[(int)Color.White].Select((b, i) => new { Occ = b, Idx = i })
                .All(x => x.Occ == other.Occupancy[(int)Color.White][x.Idx]);
            var blackPiecesEquality = Occupancy[(int)Color.Black].Select((b, i) => new { Occ = b, Idx = i })
                .All(x => x.Occ == other.Occupancy[(int)Color.Black][x.Idx]);
            return boardStateEquality && whitePiecesEquality && blackPiecesEquality;
        }

        public ulong[][] CloneOccupancy()
        {
            return CloneOccupancy(Occupancy);
        }

        private static ulong[][] CloneOccupancy(ulong[][] occupancy)
        {
            var rv = new ulong[2][];
            rv[0] = new ulong[6];
            rv[1] = new ulong[6];
            Array.Copy(occupancy[0], rv[0], 6);
            Array.Copy(occupancy[1], rv[1], 6);
            return rv;
        }

        public override string ToString()
        {
            return CurrentFEN + Environment.NewLine + base.ToString();
        }

        protected GameState GameStateFromBoard()
        {
            var gameState = EndOfGameRule.Validate(this);
            switch (gameState)
            {
                case BoardExceptionType.MaterialDraw: return GameState.Drawn;
                case BoardExceptionType.Checkmate: return GameState.Checkmate;
                case BoardExceptionType.Stalemate: return GameState.StaleMate;
                default: return GameState.None;
            }
        }

        private void ValidateEnPassant(ushort? epIndex)
        {
            var validate = new EnPassantSquareRule();
            var validationResult = validate.ValidateEnPassantSquare(Occupancy, epIndex, ActivePlayer);
            if (validationResult != BoardExceptionType.None)
            {
                throw new BoardException(validationResult,
                    "Bad en passant square passed to Board.cs ValidateEnPassant.");
            }
        }

        private void InitializeOccupancy()
        {
            Occupancy[(int)Color.Black] = new ulong[6];
            Occupancy[(int)Color.White] = new ulong[6];
            InitializeBlackOccupancy();
            InitializeWhiteOccupancy();
        }

        private void InitializeBlackOccupancy()
        {
            BlackOccupancy[(int)Piece.Pawn] = 0xff000000000000;
            BlackOccupancy[(int)Piece.Knight] = 0x4200000000000000;
            BlackOccupancy[(int)Piece.Bishop] = 0x2400000000000000;
            BlackOccupancy[(int)Piece.Rook] = 0x8100000000000000;
            BlackOccupancy[(int)Piece.Queen] = 0x800000000000000;
            BlackOccupancy[(int)Piece.King] = 0x1000000000000000;
        }

        private void InitializeWhiteOccupancy()
        {
            WhiteOccupancy[(int)Piece.Pawn] = 0xff00;
            WhiteOccupancy[(int)Piece.Knight] = 0x42;
            WhiteOccupancy[(int)Piece.Bishop] = 0x24;
            WhiteOccupancy[(int)Piece.Rook] = 0x81;
            WhiteOccupancy[(int)Piece.Queen] = 0x8;
            WhiteOccupancy[(int)Piece.King] = 0x10;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Board)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (Occupancy != null ? Occupancy.GetHashCode() : 0);
            }
        }
    }
}