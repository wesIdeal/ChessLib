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
using System.Runtime.CompilerServices;
using ChessLib.Data.Annotations;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data
{
    public class BoardInfo : BoardBase, INotifyPropertyChanged
    {
        private MoveTree<MoveStorage> _moveTree;

        public BoardInfo() : this(FENHelpers.FENInitial)
        {
            MoveTree = new MoveTree<MoveStorage>(null);
        }

        public BoardInfo(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability,
            ushort? enPassantIdx, uint? halfMoveClock, uint fullMoveCount, bool validationException = true)
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

        public new GameState GameState
        {
            get => base.GameState;
            internal set => base.GameState = value;
        }

        public void ApplySANMove(string moveText)
        {
            var moveTranslatorService = new MoveTranslatorService(this);
            var move = moveTranslatorService.GetMoveFromSAN(moveText);
            ApplyMove(move);
        }

        /// <summary>
        /// Applies a move from source and destination indexes
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="promotionPiece"></param>
        /// <returns></returns>
        public MoveError ApplyMove(ushort sourceIndex, ushort destinationIndex, PromotionPiece? promotionPiece)
        {
            var moveTranslatorService = new MoveTranslatorService(this);
            var move = moveTranslatorService.GenerateMoveFromIndexes(sourceIndex, destinationIndex, promotionPiece);
            return ApplyMove(move);
        }

        public MoveError ApplyMove(MoveExt move)
        {
            var pocSource = this.GetPieceOfColorAtIndex(move.SourceIndex);
            if (pocSource == null) throw new ArgumentException("No piece at source.");
            var moveValidator = new MoveValidator(this, move);
            var validationError = moveValidator.Validate();
            if (validationError != MoveError.NoneSet)
            {
                throw new MoveException("Error with move.", validationError, move, ActivePlayer);
            }

            ApplyValidatedMove(move);
            return MoveError.NoneSet;
        }

        public void ApplyValidatedMove(MoveExt move)
        {
            var pocSource = this.GetPieceOfColorAtIndex(move.SourceIndex);
            var moveDisplayService = new MoveDisplayService(this);
            var san = moveDisplayService.MoveToSAN(move);
            if (pocSource == null)
            {
                throw new ArgumentException("No piece found at source.");
            }
            var node = MoveTree.AddMove(new MoveStorage(this.ToFEN(), move, pocSource.Value.Piece, pocSource.Value.Color, san));
            var newBoard = this.ApplyMoveToBoard(move);
            ApplyNewBoard(newBoard);
            node.MoveData.SetPostMoveFEN(this.ToFEN());
            CurrentMove = (MoveNode<MoveStorage>)MoveTree.LastMove;
        }

        public MoveNode<MoveStorage> CurrentMove { get; set; } = null;

        public MoveStorage GetPreviousMove()
        {
            return CurrentMove.Previous.MoveData ?? CurrentMove.Parent.MoveData ?? null;
        }

        /// <summary>
        /// Gets a list of moves to choose from for traversing forward
        /// </summary>
        /// <returns></returns>
        public MoveStorage[] GetNextMoves()
        {
            var lMoves = new List<MoveStorage>();
            if (CurrentMove == null)
            {
                return lMoves.ToArray();

            }
            else
            {
                lMoves.Add(CurrentMove.MoveData);
                if (CurrentMove.Variations.Any())
                {
                    lMoves.AddRange(CurrentMove.Variations.Select(x => x.HeadMove.MoveData));
                }
            }

            return lMoves.ToArray();
        }

        public void TraverseForward(MoveStorage move)
        {
            var foundMove = FindNextNode(move);
            if (foundMove != null)
            {
                BoardInfo board;
                board = new BoardInfo(CurrentMove.MoveData.PostmoveFEN);
                ApplyNewBoard(board);
                CurrentMove = foundMove.Next;
                ApplyNewBoard(board);

            }
        }

        public void TraverseBackward()
        {
            var previousMove = FindPreviousNode();
            if (previousMove != null)
            {
                CurrentMove = (MoveNode<MoveStorage>)previousMove;
                var fen = CurrentMove?.MoveData.PremoveFEN ?? InitialFEN;
                var board = new BoardInfo(fen);
                ApplyNewBoard(board);
            }
        }

        private IMoveNode<MoveStorage> FindPreviousNode()
        {
            if (CurrentMove == null)
            {
                return MoveTree.LastMove;
            }

            if (CurrentMove == MoveTree.HeadMove)
            {
                return null;
            }
            return CurrentMove.Previous ?? CurrentMove.Parent ?? null;
        }

        /// <summary>
        /// Finds a given move from the next move in the tree or variations
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        private MoveNode<MoveStorage> FindNextNode(MoveStorage move)
        {
            if (CurrentMove.MoveData == move)
            {
                return CurrentMove;
            }

            foreach (var variation in CurrentMove.Variations)
            {
                if (variation.HeadMove.MoveData == move)
                {
                    return variation.HeadMove;
                }
            }

            return null;
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
                GameState = this.GameState,
                HalfmoveClock = this.HalfmoveClock,
                PiecePlacement = new ulong[2][]
            };
            for (int i = 0; i < 2; i++)
            {
                b.PiecePlacement[i] = (ulong[])PiecePlacement[i].Clone();
            }
            return b;
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