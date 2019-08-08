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
            node.MoveData.SetPostMoveFEN(this.ToFEN());
            CurrentMove = (MoveNode<MoveStorage>)MoveTree.LastMove;
            Debug.WriteLine($"Move {move} was applied.");
            Debug.WriteLine($"After applying move {move}, FEN is:\t{CurrentFEN}");
            return CurrentMove;
        }

        public MoveStorage UnapplyMove()
        {
            Debug.WriteLine($"Before unapply, FEN is:\t{CurrentFEN}");
            var previousMoveNode = FindPreviousMove();
            MoveStorage rv;
            if (previousMoveNode != null)
            {
                rv = CurrentMove.MoveData;
                UnapplyMove(previousMoveNode.MoveData.BoardState);
                CurrentMove = previousMoveNode;
            }
            else
            {
                UnapplyMove(InitialBoardState);
                rv = null;
            }
            Debug.WriteLine($"After unapply, FEN is:\t{CurrentFEN}");
            return rv;
        }

        private void UnapplyMove(BoardState previousBoardState)
        {
            var board = GetBoardFromBoardState(previousBoardState);
            ApplyNewBoard(board);
        }

        private IBoard GetBoardFromBoardState(BoardState previousBoardState)
        {
            var hmClock = previousBoardState.GetHalfmoveClock();
            var castlingAvailability = previousBoardState.GetCastlingAvailability();
            var epSquare = previousBoardState.GetEnPassantSquare();
            var pieces = UnApplyPiecesFromMove(CurrentMove);
            var fullMove = ActivePlayer == Color.White ? FullmoveCounter - 1 : FullmoveCounter;
            var board = new BoardInfo(pieces, ActivePlayer.Toggle(), castlingAvailability, epSquare, hmClock,
                (ushort)fullMove, false);
            return board;
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


        /// <summary>
        /// Gets a list of moves to choose from for traversing forward
        /// </summary>
        /// <returns></returns>
        public MoveStorage[] GetNextMoves()
        {
            var nextMoveNodes = GetNextMoveNodes();
            return nextMoveNodes.Select(x => x.MoveData).ToArray();
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
                lMoves.Add(CurrentMove.Next);
                if (CurrentMove.Next.Variations.Any())
                {
                    lMoves.AddRange(CurrentMove.Next.Variations.Select(x => x.HeadMove));
                }
            }

            return lMoves.ToArray();
        }

        public MoveStorage TraverseForward(MoveStorage move)
        {
            var foundMove = FindNextNode(move);
            if (foundMove != null)
            {
                BoardInfo board;
                var newBoard = this.ApplyMoveToBoard(move);
                ApplyNewBoard(newBoard);
                CurrentMove = foundMove;
                return foundMove.MoveData;
            }
            return null;
        }

        public MoveStorage TraverseBackward()
        {
            return UnapplyMove();
        }

        private MoveNode<MoveStorage> FindPreviousMove(MoveNode<MoveStorage> move)
        {
            if (move.IsNullNode)
            {
                return null;
            }

            if (move.Previous == null)
            {
                var moveParent = move.Parent; 
                if(moveParent == null)
                {
                    return null;
                }
                else
                {
                    return FindPreviousMove(moveParent);
                }
            }

            else if (move.Previous.IsNullNode)
            {
                return null;
            }

            else
            {
                return move.Previous;
            }

        }

        private MoveNode<MoveStorage> FindPreviousMove()
        {
            return FindPreviousMove(CurrentMove);
        }

        /// <summary>
        /// Finds a given move from the next move in the tree or variations
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        private MoveNode<MoveStorage> FindNextNode(MoveStorage move)
        {
            var nextMoves = GetNextMoveNodes();
            return nextMoves.FirstOrDefault(x => x.MoveData.Move == move.Move);
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

        public void GoToInitialState()
        {
            var bi = new BoardInfo(InitialFEN);
            ApplyNewBoard(bi);
            CurrentMove = MoveTree.HeadMove;
        }

        public void GoToLastMove()
        {
            MoveNode<MoveStorage> node = CurrentMove.Next;
            while (node != null)
            {
                TraverseForward(node.MoveData);
                node = CurrentMove.Next;
            }
        }
    }
}