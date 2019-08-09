using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using System;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Validators.BoardValidation;
using ChessLib.Data.Validators.MoveValidation;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ChessLib.Data.Annotations;
using ChessLib.Data.Types.Interfaces;
using System.Text;

namespace ChessLib.Data.Boards
{
    public class BoardInfo : BoardBase, INotifyPropertyChanged
    {
        private MoveTree<MoveStorage> _moveTree;
        public MoveNode<MoveStorage> CurrentMove { get; set; } = null;

        public BoardInfo() : this(FENHelpers.FENInitial)
        {
            MoveTree = new MoveTree<MoveStorage>(null);
        }

        public BoardInfo(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability,
            ushort? enPassantIdx, ushort halfMoveClock, ushort fullMoveCount, bool validationException = true)
            : base(occupancy, activePlayer, castlingAvailability, enPassantIdx, halfMoveClock, fullMoveCount)
        {
            MoveTree = new MoveTree<MoveStorage>(null);
        }

        public BoardInfo(string fen, bool is960 = false) : base(fen, is960)
        {
            MoveTree = new MoveTree<MoveStorage>(null);
        }

        public MoveTree<MoveStorage> MoveTree
        {
            get => _moveTree;
            set
            {
                _moveTree = value;
                CurrentMove = _moveTree.HeadMove;
            }
        }


        public MoveNode<MoveStorage> ApplySANMove(string moveText)
        {
            var moveTranslatorService = new MoveTranslatorService(this);
            var move = moveTranslatorService.GetMoveFromSAN(moveText);
            return ApplyMove(move);
        }


        public MoveNode<MoveStorage> ApplyMove(MoveExt move)
        {
            var pocSource = this.GetPieceOfColorAtIndex(move.SourceIndex);
            if (pocSource == null) throw new ArgumentException("No piece at source.");
            var moveValidator = new MoveValidator(this, move);
            var validationError = moveValidator.Validate();
            if (validationError != MoveError.NoneSet)
            {
                throw new MoveException("Error with move.", validationError, move, ActivePlayer);
            }
            return ApplyValidatedMove(move);
        }

        public MoveNode<MoveStorage> ApplyValidatedMove(MoveExt move)
        {

            Debug.WriteLine($"Before applying move {move}, FEN is:\t{CurrentFEN}");
            var pocSource = this.GetPieceOfColorAtIndex(move.SourceIndex);
            var capturedPiece = this.GetPieceOfColorAtIndex(move.DestinationIndex);
            if (capturedPiece == null && move.MoveType == MoveType.EnPassant)
            {
                capturedPiece = new PieceOfColor() { Color = ActivePlayer.Toggle(), Piece = Piece.Pawn };
            }
            var moveDisplayService = new MoveDisplayService(this);
            var san = moveDisplayService.MoveToSAN(move);
            if (pocSource == null)
            {
                throw new ArgumentException("No piece found at source.");
            }
            var newBoard = this.ApplyMoveToBoard(move);
            ApplyNewBoard(newBoard);
            var node = MoveTree.AddMove(new MoveStorage(this, move, capturedPiece?.Piece));
            CurrentMove = (MoveNode<MoveStorage>)MoveTree.LastMove;
            Debug.WriteLine($"Move {move} was applied.");
            Debug.WriteLine($"After applying move {move}, FEN is:\t{CurrentFEN}");
            return CurrentMove;
        }

    




        /// <summary>
        /// Clones a board object from this board
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            var b = new BoardInfo
            {
                ActivePlayer = this.ActivePlayer,
                CastlingAvailability = this.CastlingAvailability,
                EnPassantSquare = this.EnPassantSquare,
                FullmoveCounter = this.FullmoveCounter,
                HalfmoveClock = this.HalfmoveClock,
                PiecePlacement = new ulong[2][]
            };
            for (int i = 0; i < 2; i++)
            {
                b.PiecePlacement[i] = (ulong[])PiecePlacement[i].Clone();
            }
            return b;
        }

        public string CurrentFEN
        {
            get => this.ToFEN();
        }

        /// <summary>
        /// Applies the given board parameter to this board
        /// </summary>
        /// <param name="newBoard"></param>
        protected void ApplyNewBoard(IBoard newBoard)
        {
            PiecePlacement = newBoard.GetPiecePlacement();
            ActivePlayer = newBoard.ActivePlayer;
            CastlingAvailability = newBoard.CastlingAvailability;
            EnPassantSquare = newBoard.EnPassantSquare;
            HalfmoveClock = newBoard.HalfmoveClock;
            FullmoveCounter = newBoard.FullmoveCounter;
        }

      
    }
}