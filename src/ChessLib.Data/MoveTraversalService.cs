using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Validators.MoveValidation;

namespace ChessLib.Data
{
    public class MoveTraversalService
    {
        private string _initialFen;
        private bool _pauseMoveEvents;
        public MoveTraversalService(string fen)
        {
            Board = new BoardInfo(fen);
            MainMoveTree = new MoveTree(null, fen);
            CurrentMoveNode = MainMoveTree.First;
        }

        public string InitialFen
        {
            get => _initialFen;
            protected set
            {
                _initialFen = value;
                MainMoveTree = new MoveTree(null, _initialFen);
                InitialBoard = new BoardInfo(_initialFen);
                Board = (BoardInfo)InitialBoard.Clone();
            }
        }

        public IBoard InitialBoard { get; private set; }
        public BoardInfo Board { get; private set; }
        public string CurrentFEN => Board.CurrentFEN;
        public MoveTree MainMoveTree { get; protected set; }
        public LinkedListNode<MoveStorage> CurrentMoveNode { get; set; }

        public bool HasNextMove => NextMoveNode != null;
        public MoveTree CurrentTree => (MoveTree)CurrentMoveNode.List;
        public LinkedListNode<MoveStorage> NextMoveNode => CurrentMoveNode.Next;

        public LinkedListNode<MoveStorage> PreviousMoveNode
        {
            get
            {
                var currentList = CurrentMoveNode.List as MoveTree;
                return CurrentMoveNode.Previous ??
                       currentList?.VariationParentNode;
            }
        }

        /// <summary>
        /// Sets the board, in case the user would like to pass in an inherited version to add functionality.
        /// </summary>
        /// <param name="board"></param>
        public void SetBoardInterface(BoardInfo board)
        {
            Board = board;
            GoToInitialState();
        }


        public event EventHandler MoveMade;

        private void OnMoveMade()
        {
            if (!_pauseMoveEvents)
            {
                Volatile.Read(ref MoveMade)?.Invoke(this, EventArgs.Empty);
            }
        }

        //public static ushort[] GetSquaresUpdated(IMoveExt move)
        //{
        //    if (move.MoveType == MoveType.Castle)
        //    {
        //        if (move == MoveHelpers.WhiteCastleKingSide)
        //        {
        //            return new ushort[] { 4, 5, 6, 7 };
        //        }

        //        if (move == MoveHelpers.WhiteCastleQueenSide)
        //        {
        //            return new ushort[] { 2, 3, 4 };
        //        }

        //        if (move == MoveHelpers.BlackCastleKingSide)
        //        {
        //            return new ushort[] { 60, 61, 62, 63 };
        //        }

        //        return new ushort[] { 56, 58, 59, 60 };
        //    }

        //    var rv = new[] { move.SourceIndex, move.DestinationIndex };

        //    if (move.MoveType == MoveType.EnPassant)
        //    {
        //        ushort extra;
        //        if (move.DestinationIndex.GetRank() == 5)
        //        {
        //            extra = (ushort)(move.DestinationIndex - 8);
        //        }
        //        else
        //        {
        //            extra = (ushort)(move.DestinationIndex + 8);
        //        }

        //        var nRv = rv.ToList();
        //        nRv.Add(extra);
        //        rv = nRv.ToArray();
        //    }

        //    return rv;
        //}

        public MoveStorage[] GetNextMoves()
        {
            var nextMoveNodes = GetNextMoveNodes();
            return nextMoveNodes.Select(x => x.Value).ToArray();
        }

        protected LinkedListNode<MoveStorage>[] GetNextMoveNodes()
        {
            if (!HasNextMove)
            {
                return new LinkedListNode<MoveStorage>[] { };
            }
            Debug.Assert(CurrentMoveNode.Next != null, "Next move was null. Execution should not get to this point.");
            var lMoves = new List<LinkedListNode<MoveStorage>> { CurrentMoveNode.Next };
            var nextMove = CurrentMoveNode.Next.Value;
            if (nextMove.Variations.Any())
            {
                lMoves.AddRange(nextMove.Variations.Select(x => x.First));
            }

            return lMoves.ToArray();
        }

        public IBoard TraverseForward()
        {
            if (!HasNextMove) return Board;
            ApplyMoveToBoard(NextMoveNode.Value);
            CurrentMoveNode = NextMoveNode;

            return Board;
        }

        public IBoard TraverseBackward()
        {
            TraverseBackToNode(PreviousMoveNode);
            return Board;
        }

        protected LinkedListNode<MoveStorage> TraverseBackToNode(LinkedListNode<MoveStorage> node)
        {
            _pauseMoveEvents = true;
            if (node != null)
            {
                while (!CurrentMoveNode.Equals(node) && CurrentMoveNode != null && !CurrentMoveNode.Value.IsNullMove)
                {
                    UnApplyMove();
                }
            }
            else
            {
                GoToInitialState();
            }

            _pauseMoveEvents = false;
            OnMoveMade();
            return CurrentMoveNode;
        }


        protected LinkedListNode<MoveStorage> UnApplyMove()
        {
            if (!PreviousMoveNode.Value.IsNullMove)
            {
                UnApplyMove(PreviousMoveNode);
                CurrentMoveNode = PreviousMoveNode;
            }
            else
            {
                GoToInitialState();
            }

            Debug.WriteLine($"It is now {Board.ActivePlayer}'s move.");
            Debug.WriteLine($"FEN is now:\t{CurrentFEN}\r\n{new string('*', 20)}");
            return CurrentMoveNode;
        }


        /// <summary>
        ///     Traverse the tree to the next move.
        /// </summary>
        /// <param name="move">The move that should be set to current.</param>
        /// <returns>
        ///     Board state for the resulting board. null if the end of the tree was reached.
        /// </returns>
        ///     <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>
        public IBoard TraverseForward(MoveStorage move)
        {
            LinkedListNode<MoveStorage> foundMove;
            try
            {
                foundMove = FindNextMoveNodeFromMove(move);
            }
            catch (Exception exc)
            {
                throw new MoveException($"TraverseForward({move}) error. Move not found.", exc);
            }

            ApplyMoveToBoard(move);
            CurrentMoveNode = foundMove;
            return Board;
        }

        private LinkedListNode<MoveStorage> FindNextMoveNodeFromMove(MoveStorage move)
        {
            return GetNextMoveNodes().First(x => x.Value.Equals(move));
        }


        #region Go To Specific Moves

        public void GoToInitialState()
        {
            var bi = new BoardInfo(InitialFen);
            ApplyNewBoard(bi);
            while (CurrentTree.VariationParentNode != null)
            {
                CurrentMoveNode = CurrentTree.VariationParentNode;
            }

            CurrentMoveNode = CurrentTree.First;
            MainMoveTree = CurrentTree;
        }

        public void GoToLastMove()
        {
            var node = NextMoveNode;
            _pauseMoveEvents = true;
            while (node != null)
            {
                TraverseForward();
                node = NextMoveNode;
            }
            _pauseMoveEvents = false;
            OnMoveMade();
        }

        #endregion

        #region Move Application

        #region Variations

        protected LinkedListNode<MoveStorage> ApplySanVariationMove(string moveText)
        {
            TraverseBackward();
            var move = TranslateSanMove(moveText);
            move.SAN = moveText;
            ValidateMove(move);
            return ApplyValidatedMoveVariation(move);
        }

        protected LinkedListNode<MoveStorage> ApplyMoveVariation(MoveExt move)
        {
            TraverseBackward();
            ValidateMove(move);
            return ApplyValidatedMoveVariation(move);
        }

        private LinkedListNode<MoveStorage> ApplyValidatedMoveVariation(MoveExt move)
        {
            try
            {
                var capturedPiece = GetCapturedPiece(move);
                if (string.IsNullOrEmpty(move.SAN))
                {
                    move.SAN = GetMoveText(move);
                }

                var variationParentFen = CurrentFEN;
                ApplyMoveToBoard(move);
                var moveStorageObj = new MoveStorage(Board, move, capturedPiece) { Validated = true };
                Debug.Assert(CurrentMoveNode.Next != null, "Cannot apply a variation if there is not a move to apply it to.");
                CurrentMoveNode =
                    CurrentMoveNode.Next.Value.AddVariation(CurrentMoveNode, moveStorageObj, variationParentFen);
               
                return CurrentMoveNode;
            }
            catch (Exception e)
            {
                throw new MoveException($"Issue while applying move {move}.", e);
            }
        }

        #endregion

        #region Regular Moves

        public LinkedListNode<MoveStorage> ApplySanMove(string moveText,
            MoveApplicationStrategy moveApplicationStrategy)
        {
            Debug.WriteLine($"Applying move {moveText}");
            if (moveApplicationStrategy == MoveApplicationStrategy.Variation)
            {
                return ApplySanVariationMove(moveText);
            }

            var move = TranslateSanMove(moveText);
            move.SAN = moveText;
            return ApplyMove(move);
        }

        public LinkedListNode<MoveStorage> ApplyMove(MoveExt move,
            MoveApplicationStrategy moveApplicationStrategy = MoveApplicationStrategy.ContinueMainLine)
        {
            if (moveApplicationStrategy == MoveApplicationStrategy.Variation)
            {
                return ApplyMoveVariation(move);
            }
            ValidateMove(move);
            return ApplyValidatedMove(move);
        }

        protected LinkedListNode<MoveStorage> ApplyValidatedMove(MoveExt move,
            MoveApplicationStrategy moveApplicationStrategy = MoveApplicationStrategy.ContinueMainLine)
        {
            Debug.WriteLine($"Before applying move {move}, FEN is:\t{Board.CurrentFEN}");
            var capturedPiece = GetCapturedPiece(move);
            if (string.IsNullOrEmpty(move.SAN))
            {
                move.SAN = GetMoveText(move);
            }

            ApplyMoveToBoard(move);
            var moveStorageObject = new MoveStorage(Board, move, capturedPiece) { Validated = true };
            CurrentMoveNode = CurrentTree.AddMove(moveStorageObject);
            return CurrentMoveNode;
        }

        #endregion

        protected void ValidateMove(MoveExt move)
        {
            if (move is MoveStorage storage)
            {
                if (storage.Validated)
                {
                    return;
                }
            }

            var moveValidator = new MoveValidator(Board, move);
            var validationError = moveValidator.Validate();
            if (validationError != MoveError.NoneSet)
            {
                throw new MoveException("Error with move.", validationError, move, Board.ActivePlayer);
            }
        }

        private string GetMoveText(MoveExt move)
        {
            var moveDisplay = new MoveDisplayService(Board);
            return moveDisplay.MoveToSAN(move);
        }

        private Piece? GetCapturedPiece(MoveExt move)
        {
            var capturedPiece = Board.GetPieceOfColorAtIndex(move.DestinationIndex);
            if (capturedPiece == null && move.MoveType == MoveType.EnPassant)
            {
                capturedPiece = new PieceOfColor { Color = Board.ActivePlayer.Toggle(), Piece = Piece.Pawn };
            }

            return capturedPiece?.Piece;
        }


        protected void ApplyMoveToBoard(MoveExt move)
        {
            var newBoard = Board.ApplyMoveToBoard(move);
            ApplyNewBoard(newBoard);
        }

        /// <summary>
        ///     Applies the given board parameter to this board
        /// </summary>
        /// <param name="newBoard"></param>
        protected void ApplyNewBoard(IBoard newBoard)
        {
            Board.SetPiecePlacement(newBoard.GetPiecePlacement());
            Board.ActivePlayer = newBoard.ActivePlayer;
            Board.CastlingAvailability = newBoard.CastlingAvailability;
            Board.EnPassantSquare = newBoard.EnPassantSquare;
            Board.HalfmoveClock = newBoard.HalfmoveClock;
            Board.FullmoveCounter = newBoard.FullmoveCounter;
            OnMoveMade();
        }

        private void UnApplyMove(LinkedListNode<MoveStorage> previousNode)
        {
            var board = GetBoardFromBoardState(previousNode);
            ApplyNewBoard(board);
        }

        private IBoard GetBoardFromBoardState(LinkedListNode<MoveStorage> previousNode)
        {
            if (previousNode.Value.IsNullMove)
            {
                return InitialBoard;
            }

            var previousState = previousNode.Value.BoardState;
            var hmClock = previousState.GetHalfmoveClock();
            var castlingAvailability = previousState.GetCastlingAvailability();
            var epSquare = previousState.GetEnPassantSquare();
            var pieces = UnApplyPiecesFromMove(CurrentMoveNode.Value);
            var fullMove = Board.ActivePlayer == Color.White ? Board.FullmoveCounter - 1 : Board.FullmoveCounter;
            var board = new BoardInfo(pieces, Board.ActivePlayer.Toggle(), castlingAvailability, epSquare, hmClock,
                (ushort)fullMove, false);
            return board;
        }

        private ulong[][] UnApplyPiecesFromMove(MoveStorage currentMove)
        {
            var piece = currentMove.MoveType == MoveType.Promotion
                ? Piece.Pawn
                : Board.GetPieceAtIndex(currentMove.DestinationIndex);

            Debug.Assert(piece.HasValue, "Piece for un-apply() has no value.");
            var src = currentMove.DestinationValue;
            var dst = currentMove.SourceValue;
            var board = (IBoard)Board.Clone();
            var active = (int)board.ActivePlayer.Toggle();
            var opp = active ^ 1;
            var piecePlacement = board.GetPiecePlacement();
            var capturedPiece = currentMove.BoardState.GetPieceCaptured();


            piecePlacement[active][(int)piece.Value] = piecePlacement[active][(int)piece] | dst;
            piecePlacement[active][(int)piece.Value] = piecePlacement[active][(int)piece] & ~src;


            if (capturedPiece.HasValue)
            {
                var capturedPieceSrc = src;
                if (currentMove.MoveType == MoveType.EnPassant)
                {
                    capturedPieceSrc = (Color)active == Color.White
                        ? ((ushort)(src.GetSetBits()[0] - 8)).ToBoardValue()
                        : ((ushort)(src.GetSetBits()[0] + 8)).ToBoardValue();
                }

                Debug.WriteLine(
                    $"{board.ActivePlayer}'s captured {capturedPiece} is being replaced. ulong={piecePlacement[opp][(int)capturedPiece]}");
                piecePlacement[opp][(int)capturedPiece] ^= capturedPieceSrc;
                Debug.WriteLine(
                    $"{board.ActivePlayer}'s captured {capturedPiece} was replaced. ulong={piecePlacement[opp][(int)capturedPiece]}");
            }

            if (currentMove.MoveType == MoveType.Promotion)
            {
                var promotionPiece = (Piece)(currentMove.PromotionPiece + 1);
                Debug.WriteLine($"Un-applying promotion to {promotionPiece}.");
                Debug.WriteLine($"{promotionPiece} ulong is {piecePlacement[active][(int)promotionPiece].ToString()}");
                piecePlacement[active][(int)promotionPiece] &= ~src;
                Debug.WriteLine(
                    $"{promotionPiece} ulong is now {piecePlacement[active][(int)promotionPiece].ToString()}");
            }
            else if (currentMove.MoveType == MoveType.Castle)
            {
                var rookMove = MoveHelpers.GetRookMoveForCastleMove(currentMove);
                piecePlacement[active][(int)Piece.Rook] = piecePlacement[active][(int)Piece.Rook] ^
                                                           (rookMove.SourceValue | rookMove.DestinationValue);
            }

            return piecePlacement;
        }

        private MoveExt TranslateSanMove(string moveText)
        {
            var moveTranslatorService = new MoveTranslatorService(Board);
            var move = moveTranslatorService.GetMoveFromSAN(moveText);
            return move;
        }

        public LinkedListNode<MoveStorage> ExitVariation()
        {
            var currentTree = (MoveTree)CurrentMoveNode.List;
            var parentMove = currentTree.VariationParentNode;
            var parentFen = currentTree.StartingFEN;
            Board = new BoardInfo(parentFen);
            CurrentMoveNode = parentMove;
            TraverseForward();
            return parentMove;
        }

        #endregion
    }
}