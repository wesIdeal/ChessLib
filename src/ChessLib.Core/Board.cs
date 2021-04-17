using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.BoardValidation;
using EnumsNET;

namespace ChessLib.Core
{
    public class Board : BoardState, IEquatable<Board>, IBoard
    {
        protected readonly IBoardValidator BoardValidator;

        
        public Board(IBoardValidator boardValidator = null) : base(FENHelpers.FENInitial)
        {
            BoardValidator = boardValidator ?? new BoardValidator();
            Occupancy = new ulong[2][];
            InitializeOccupancy();
            BoardValidator = boardValidator ?? new BoardValidator();
        }

        /// <summary>
        ///     Construct Board from FEN
        /// </summary>
        /// <param name="fen">fen string </param>
        /// <param name="boardValidator"></param>
        public Board(string fen, IBoardValidator boardValidator = null) : base(fen)
        {
            BoardValidator = boardValidator ?? new BoardValidator();
            Occupancy = fen.BoardFromFen(out _, out _, out _, out _, out _);
            Validate();
        }

        public Board(ulong[][] occupancy, byte halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece,
            CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter,
            IBoardValidator boardValidator = null)
            : this(occupancy, halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, activePlayer,
                fullMoveCounter, true, boardValidator)

        {
        }

        internal Board(ulong[][] occupancy, byte halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece,
            CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter,
            bool validateBoard = true,
            IBoardValidator boardValidator = null)
            : base(halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, activePlayer, fullMoveCounter)
        {
            BoardValidator = boardValidator ?? new BoardValidator();
            Occupancy = CloneOccupancy(occupancy);
            if (validateBoard)
            {
                Validate();
            }
        }

        public ulong[] BlackOccupancy => Occupancy[(int)Color.Black];
        public ulong[] WhiteOccupancy => Occupancy[(int)Color.White];
        public Color OpponentColor => ActivePlayer.Toggle();

        public string CurrentFEN => this.FENFromBoard();

        public object Clone()
        {
            Console.WriteLine($"Total occupancy: {Occupancy.Occupancy()}");
            var clonedOccupancy = CloneOccupancy();
            var activePlayerColor = ActivePlayer;
            return new Board(clonedOccupancy, HalfMoveClock, EnPassantIndex, PieceCaptured, CastlingAvailability,
                activePlayerColor, FullMoveCounter, false, BoardValidator);
        }


        public ulong[][] Occupancy { get; }

        public ulong[][] CloneOccupancy()
        {
            return CloneOccupancy(Occupancy);
        }


        public virtual bool Equals(Board otherBoard)
        {
            if (ReferenceEquals(null, otherBoard) || GetType() != otherBoard.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, otherBoard))
            {
                return true;
            }
            
            if (Occupancy.Occupancy() != otherBoard.Occupancy.Occupancy())
            {
                return false;
            }
            var returnValue = true;
            Parallel.ForEach(BoardConstants.AllColorIndexes, (color, state) =>
            {
                var thisColorOccupancy = Occupancy[color];
                var otherColorOccupancy = otherBoard.Occupancy[color];
                var pieceEquality = CheckPieceEquality(thisColorOccupancy, otherColorOccupancy);
                if (!pieceEquality)
                {
                    state.Stop();
                    returnValue = false;
                }
            });

            return returnValue;
        }

        private static bool CheckPieceEquality(ulong[] thesePieces, ulong[] otherPieces)
        {
            Debug.Assert(thesePieces.Length == otherPieces.Length && otherPieces.Length == 6);
            var returnVal = true;
            Parallel.ForEach(BoardConstants.AllPieceIndexes, (pieceValue, state) =>
            {
                var otherPieceValue = otherPieces[pieceValue];
                var thesePieceValue = thesePieces[pieceValue];
                if (thesePieceValue != otherPieceValue)
                {
                    returnVal = false;
                    state.Break();
                }
            });

            return returnVal;
        }

        private void Validate()
        {
            var validationResult = BoardValidator.Validate(this);
            if (validationResult != BoardExceptionType.None)
            {
                switch (validationResult)
                {
                    case BoardExceptionType.Checkmate:
                        GameState = GameState.Checkmate;
                        break;
                    case BoardExceptionType.MaterialDraw:
                        GameState = GameState.Drawn;
                        break;
                    case BoardExceptionType.Stalemate:
                        GameState = GameState.StaleMate;
                        break;
                    default:
                        throw new BoardException(validationResult, "Invalid board setup.");
                }
            }
        }

        private static ulong[][] CloneOccupancy(in ulong[][] occupancy)
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
            var otherBoard = obj as Board;
            return Equals(otherBoard);
        }

        public override int GetHashCode()
        {
            if (Occupancy != null)
            {
                return Occupancy.GetHashCode();
            }

            return 0;
        }
    }
}