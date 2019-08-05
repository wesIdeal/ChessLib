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
            node.MoveData.SetPostMoveFEN(this.ToFEN());
            CurrentMove = (MoveNode<MoveStorage>)MoveTree.LastMove;
            Debug.WriteLine($"Move {move} was applied.");
            Debug.WriteLine($"After applying move {move}, FEN is:\t{CurrentFEN}");
        }

        public void UnapplyMove()
        {
            Debug.WriteLine($"Before unapply, FEN is:\t{CurrentFEN}");
            var previousBoardState = FindPreviousBoardState();
            if (previousBoardState != null)
            {
                var currentMove = CurrentMove.MoveData;
                var hmClock = previousBoardState.GetHalfmoveClock();
                var castlingAvailability = previousBoardState.GetCastlingAvailability();
                var epSquare = previousBoardState.GetEnPassantSquare();
                var pieces = UnApplyPiecesFromMove(CurrentMove);
                var fullMove = ActivePlayer == Color.White ? FullmoveCounter - 1 : FullmoveCounter;
                var board = new BoardInfo(pieces, ActivePlayer.Toggle(), castlingAvailability, epSquare, hmClock,
                    (ushort)fullMove, false);
                ApplyNewBoard(board);
                CurrentMove = CurrentMove.Previous ?? CurrentMove.Parent;

            }
            Debug.WriteLine($"After unapply, FEN is:\t{CurrentFEN}");
        }

        private ulong[][] UnApplyPiecesFromMove(MoveNode<MoveStorage> currentMoveNode)
        {

            var currentMove = currentMoveNode.MoveData;
            var piece = currentMove.MoveType == MoveType.Promotion ? Piece.Pawn :
                this.GetPieceAtIndex(currentMove.DestinationIndex);

            Debug.Assert(piece.HasValue, "Piece for unapply() has no value.");
            var src = currentMove.DestinationValue;
            var dst = currentMove.SourceValue;
            var board = (IBoard)this.Clone();
            var active = (int)board.ActivePlayer.Toggle();
            var opp = active ^ 1;
            var piecePlacement = board.GetPiecePlacement();
            var capturedPiece = currentMove.BoardState.GetPieceCaptured();


            piecePlacement[active][(int)piece.Value] = piecePlacement[active][(int)piece] | dst;
            piecePlacement[active][(int)piece.Value] = piecePlacement[active][(int)piece] & ~(src);


            if (capturedPiece.HasValue)
            {
                var capturedPieceSrc = src;
                if (currentMove.MoveType == MoveType.EnPassant)
                {
                    capturedPieceSrc = (Color)active == Color.White ?
                        ((ushort)(src.GetSetBits()[0] - 8)).ToBoardValue()
                        : ((ushort)(src.GetSetBits()[0] + 8)).ToBoardValue();
                }
                Debug.WriteLine($"{board.ActivePlayer}'s captured {capturedPiece} is being replaced. ulong={piecePlacement[opp][(int)capturedPiece]}");
                piecePlacement[opp][(int)capturedPiece] ^= capturedPieceSrc;
                Debug.WriteLine($"{board.ActivePlayer}'s captured {capturedPiece} was replaced. ulong={piecePlacement[opp][(int)capturedPiece]}");

            }
            if (currentMove.MoveType == MoveType.Promotion)
            {
                var promotionPiece = (Piece)(currentMove.PromotionPiece + 1);
                Debug.WriteLine($"Uapplying promotion to {promotionPiece}.");
                Debug.WriteLine($"{promotionPiece} ulong is {piecePlacement[active][(int)promotionPiece].ToString()}");
                piecePlacement[active][(int)promotionPiece] &= ~(src);
                Debug.WriteLine($"{promotionPiece} ulong is now {piecePlacement[active][(int)promotionPiece].ToString()}");
            }
            else if (currentMove.MoveType == MoveType.Castle)
            {
                MoveExt rookMove = MoveHelpers.GetRookMoveForCastleMove(currentMove);
                piecePlacement[active][(int)Piece.Rook] = piecePlacement[active][(int)Piece.Rook] ^ (rookMove.SourceValue | rookMove.DestinationValue);
            }

            return piecePlacement;
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
            return GetNextMoveNodes().Select(x => x.MoveData).ToArray();
        }

        private MoveNode<MoveStorage>[] GetNextMoveNodes()
        {
            var lMoves = new List<MoveNode<MoveStorage>>();
            if (CurrentMove.Next == null)
            {
                return lMoves.ToArray();

            }
            else
            {
                lMoves.Add(CurrentMove);
                if (CurrentMove.Next.Variations.Any())
                {
                    lMoves.AddRange(CurrentMove.Next.Variations.Select(x => x.HeadMove));
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
                CurrentMove = foundMove;
                ApplyNewBoard(board);

            }
        }

        public void TraverseBackward()
        {
            UnapplyMove();
        }

        private BoardState FindPreviousBoardState()
        {
            if (CurrentMove.IsNullNode)
            {
                return null;
            }
            if (CurrentMove.Previous.IsNullNode)
            {
                return InitialBoardState;
            }
            else
            {
                return CurrentMove.Previous.MoveData.BoardState;
            }
        }

        /// <summary>
        /// Finds a given move from the next move in the tree or variations
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        private MoveNode<MoveStorage> FindNextNode(MoveStorage move)
        {
            var nextMoves = GetNextMoveNodes();
            return nextMoves.FirstOrDefault(x => x.MoveData == move);
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