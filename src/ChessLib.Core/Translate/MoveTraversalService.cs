using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ChessLib.Core.Helpers;
using ChessLib.Core.IO;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.EventArgs;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.MoveValidation;

namespace ChessLib.Core.Translate
{
    public abstract class MoveTraversalService
    {
        private LinkedListNode<BoardSnapshot> _currentMoveNode;

        private Board _initialBoard;
        private bool _pauseMoveEvents;


        protected MoveTraversalService(string fen)
        {
            SetupBoard(fen);
            MainMoveTree = new MoveTree(null, InitialFen);
            CurrentMoveNode = MainMoveTree.First;
            GoToInitialState();
        }

        protected MoveTraversalService(MoveTraversalService moveTraversalService) : this(
            moveTraversalService.InitialFen)
        {
            MainMoveTree = new MoveTree(moveTraversalService.MainMoveTree);
        }

        public Board CurrentBoard { get; private set; }

        /// <summary>
        ///     Gets a value that determines if a game is being loaded. Used to determine if MoveMade event is fired.
        /// </summary>
        public bool IsLoaded { get; private set; }

        public bool ShouldSendMoveEvents => MoveMade != null && !_pauseMoveEvents && IsLoaded;

        public string InitialFen => InitialBoard.CurrentFEN;

        public Board InitialBoard
        {
            get => _initialBoard;
            private set => _initialBoard = new Board(value);
        }

        public string CurrentFEN => CurrentBoard.CurrentFEN;
        public MoveTree MainMoveTree { get; protected set; }
        public int PlyCount => MainMoveTree.Skip(1).Count();

        public LinkedListNode<BoardSnapshot> CurrentMoveNode
        {
            get => _currentMoveNode;
            set
            {
                _currentMoveNode = value;
                OnMoveMade();
            }
        }

        public bool HasNextMove => NextMoveNode != null;
        public MoveTree CurrentTree => (MoveTree) CurrentMoveNode.List;
        public LinkedListNode<BoardSnapshot> NextMoveNode => CurrentMoveNode.Next;

        public LinkedListNode<BoardSnapshot> PreviousMoveNode => GetPreviousNode(CurrentMoveNode);

        private void SetupBoard(string fen)
        {
            if (string.IsNullOrEmpty(fen))
            {
                fen = BoardConstants.FenStartingPosition;
            }

            var ftb = new FenTextToBoard();
            var board = ftb.Translate(fen);
            InitialBoard = board;
            CurrentBoard = new Board(board);
        }

        public virtual event EventHandler<MoveMadeEventArgs> MoveMade;

        public void AddComment(string comment)
        {
            //if the current tree has any moves that aren't null
            // (otherwise the comment belongs at the beginning of game or variation)
            if (CurrentTree.Any(x => !x.IsNullMove))
            {
                if (!string.IsNullOrEmpty(CurrentMoveNode.Value.Comment))
                {
                    CurrentMoveNode.Value.Comment += $" {comment}";
                }
                else
                {
                    CurrentMoveNode.Value.Comment = comment;
                }
            }
            else
            {
                CurrentTree.GameComment = comment;
            }
        }

        public void BeginGameInitialization()
        {
            IsLoaded = false;
        }

        public void EndGameInitialization()
        {
            GoToInitialState();
            IsLoaded = true;
        }

        public BoardSnapshot[] GetNextMoves()
        {
            var nextMoveNodes = GetNextMoveNodes();
            return nextMoveNodes.Select(x => x.Value).ToArray();
        }

        public IBoard TraverseBackward()
        {
            TraverseBackToNode(PreviousMoveNode);
            return CurrentBoard;
        }

        public IBoard TraverseForward()
        {
            if (!HasNextMove)
            {
                return CurrentBoard;
            }

            ApplyMoveToBoard(NextMoveNode.Value);
            CurrentMoveNode = NextMoveNode;
            return CurrentBoard;
        }


        /// <summary>
        ///     Traverse the tree to the next move.
        /// </summary>
        /// <param name="move">The move that should be set to current.</param>
        /// <returns>
        ///     Board state for the resulting board. null if the end of the tree was reached.
        /// </returns>
        /// <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>
        public IBoard TraverseForward(BoardSnapshot move)
        {
            LinkedListNode<BoardSnapshot> foundMove;
            try
            {
                foundMove = FindNextMoveNodeFromMove(move);
            }
            catch (Exception exc)
            {
                throw new MoveException($"TraverseForward({move}) error. MoveValue not found.", exc);
            }

            ApplyMoveToBoard(move);
            CurrentMoveNode = foundMove;
            return CurrentBoard;
        }

        protected LinkedListNode<BoardSnapshot>[] GetNextMoveNodes()
        {
            if (!HasNextMove)
            {
                return new LinkedListNode<BoardSnapshot>[] { };
            }

            Debug.Assert(CurrentMoveNode.Next != null, "Next move was null. Execution should not get to this point.");
            var lMoves = new List<LinkedListNode<BoardSnapshot>> {CurrentMoveNode.Next};
            var nextMove = CurrentMoveNode.Next.Value;
            if (nextMove.Variations.Any())
            {
                lMoves.AddRange(nextMove.Variations.Select(x => x.First));
            }

            return lMoves.ToArray();
        }

        protected LinkedListNode<BoardSnapshot> TraverseBackToNode(LinkedListNode<BoardSnapshot> node)
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


        protected LinkedListNode<BoardSnapshot> UnApplyMove()
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

            return CurrentMoveNode;
        }

        internal LinkedListNode<BoardSnapshot> GetPreviousNode(LinkedListNode<BoardSnapshot> current)
        {
            var currentList = current.List as MoveTree;
            return current.Previous ??
                   currentList?.VariationParentNode;
        }

        private LinkedListNode<BoardSnapshot> FindNextMoveNodeFromMove(BoardSnapshot move)
        {
            return GetNextMoveNodes().First(x => x.Value.Equals(move));
        }

        private void OnMoveMade()
        {
            if (!ShouldSendMoveEvents)
            {
                return;
            }

            var moves = new List<Move>();
            var currentMoveNode = CurrentMoveNode;
            while (!currentMoveNode.Value.IsNullMove && currentMoveNode.Value != null)
            {
                moves.Add(currentMoveNode.Value);
                currentMoveNode = GetPreviousNode(currentMoveNode);
            }

            moves.Reverse();
            Volatile.Read(ref MoveMade)?.Invoke(this, new MoveMadeEventArgs(moves.ToArray(), CurrentFEN));
        }


        #region Go To Specific Moves

        public void GoToInitialState()
        {
            ApplyNewBoard(InitialBoard);
            while (CurrentTree.VariationParentNode != null)
            {
                CurrentMoveNode = CurrentTree.VariationParentNode;
            }

            CurrentMoveNode = CurrentTree.First;
           
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

        #region MoveValue Application

        #region Variations

        protected LinkedListNode<BoardSnapshot> ApplyMoveVariation(Move move)
        {
            TraverseBackward();
            ValidateMove(move);
            return ApplyValidatedMoveVariation(move);
        }

        protected LinkedListNode<BoardSnapshot> ApplyValidatedMoveVariation(Move move)
        {
            try
            {
                

                var variationParentFen = CurrentFEN;
                var preMove = new Board(CurrentBoard);
                ApplyMoveToBoard(move);
                var postMove = new Board(CurrentBoard);
                if (string.IsNullOrEmpty(move.SAN))
                {
                    move.SAN = _moveToSan.Translate(move, preMove, postMove);
                }
                return AddVariationToCurrentNode(CurrentMoveNode, move, variationParentFen);
            }
            catch (Exception e)
            {
                throw new MoveException($"Issue while applying move {move}.", e);
            }

        }

        public LinkedListNode<BoardSnapshot> AddVariationToCurrentNode(LinkedListNode<BoardSnapshot> currentMoveNode,
            Move move, string initialFen)
        {
            var boardSnapshot = new BoardSnapshot(CurrentBoard, move) {Validated = true};

            Debug.Assert(CurrentMoveNode.Next != null,
                "Cannot apply a variation if there is not a move to apply it to.");
            var variationList = CurrentMoveNode.Next.Value.Variations;
            var variation = new MoveTree(currentMoveNode, initialFen);
            variation.AddFirst(boardSnapshot);
            variationList.Add(variation);
            var addedVariationIdx = variationList.IndexOf(variation);
            return CurrentMoveNode = variationList.ElementAt(addedVariationIdx).First;
        }

        #endregion

        #region Regular Moves

        public LinkedListNode<BoardSnapshot> ApplyMove(Move move,
            MoveApplicationStrategy moveApplicationStrategy = MoveApplicationStrategy.ContinueMainLine)
        {
            if (moveApplicationStrategy == MoveApplicationStrategy.Variation)
            {
                return ApplyMoveVariation(move);
            }

            ValidateMove(move);
           return ApplyValidatedMove(move);
        }

        private readonly MoveToSan _moveToSan = new MoveToSan();

        internal LinkedListNode<BoardSnapshot> ApplyValidatedMove(Move move,
            MoveApplicationStrategy moveApplicationStrategy = MoveApplicationStrategy.ContinueMainLine)
        {
            //Uncomment to debug move application
            //Debug.WriteLine($"Before applying move {move}, FEN is:\t{Board.CurrentFEN}");

            var variationParentFen = CurrentFEN;
            var preMove = new Board(CurrentBoard);
            ApplyMoveToBoard(move);
            var postMove = new Board(CurrentBoard);
            if (string.IsNullOrEmpty(move.SAN))
            {
                move.SAN = _moveToSan.Translate(move, preMove, postMove);
            }
            var moveStorageObject = new BoardSnapshot(CurrentBoard, move) {Validated = true};
            CurrentMoveNode = CurrentTree.AddMove(moveStorageObject);
            return CurrentMoveNode;
        }

        #endregion

        protected void ValidateMove(Move move)
        {
            if (move is BoardSnapshot {Validated: true})
            {
                return;
            }

            var moveValidator = new MoveValidator(CurrentBoard, move);
            var validationError = moveValidator.Validate();
            if (validationError != MoveError.NoneSet)
            {
                throw new MoveException("Error with move.", validationError, move, CurrentBoard.ActivePlayer);
            }
        }

        private string GetMoveText(Move move, Board preMoveBoard, Board postMoveBoard)
        {
            return _moveToSan.Translate(move, preMoveBoard, postMoveBoard);
        }


        protected void ApplyMoveToBoard(Move move)
        {
            var newBoard = CurrentBoard.ApplyMoveToBoard(move);
            ApplyNewBoard(newBoard);
        }

        /// <summary>
        ///     Applies the given board parameter to this board
        /// </summary>
        /// <param name="newBoard"></param>
        protected void ApplyNewBoard(Board newBoard)
        {
            CurrentBoard = new Board(newBoard);
        }

        private void UnApplyMove(LinkedListNode<BoardSnapshot> previousNode)
        {
            var board = GetBoardFromBoardState(previousNode);
            ApplyNewBoard(board);
        }

        private Board GetBoardFromBoardState(LinkedListNode<BoardSnapshot> previousNode)
        {
            if (previousNode.Value.IsNullMove)
            {
                return InitialBoard;
            }

            var previousState = previousNode.Value.BoardState;
            var hmClock = previousState.HalfMoveClock;
            var epSquare = previousState.EnPassantIndex;
            var pieces = UnApplyPiecesFromMove(CurrentMoveNode.Value);
            var fullMove = CurrentBoard.ActivePlayer == Color.White
                ? CurrentBoard.FullMoveCounter - 1
                : CurrentBoard.FullMoveCounter;
            var board = new Board(pieces, hmClock, epSquare, previousState.PieceCaptured,
                previousState.CastlingAvailability,
                previousState.ActivePlayer, (ushort) fullMove);
            return board;
        }

        private ulong[][] UnApplyPiecesFromMove(BoardSnapshot currentMove)
        {
            var piece = currentMove.MoveType == MoveType.Promotion
                ? Piece.Pawn
                : BoardHelpers.GetPieceAtIndex(CurrentBoard.Occupancy, currentMove.DestinationIndex);

            Debug.Assert(piece.HasValue, "Piece for un-apply() has no value.");
            var src = currentMove.DestinationValue;
            var dst = currentMove.SourceValue;
            var board = new Board(CurrentBoard);
            var active = (int) board.ActivePlayer.Toggle();
            var opp = active ^ 1;
            var piecePlacement = board.Occupancy;
            var capturedPiece = currentMove.BoardState.PieceCaptured;


            piecePlacement[active][(int) piece.Value] = piecePlacement[active][(int) piece] | dst;
            piecePlacement[active][(int) piece.Value] = piecePlacement[active][(int) piece] & ~src;


            if (capturedPiece.HasValue)
            {
                var capturedPieceSrc = src;
                if (currentMove.MoveType == MoveType.EnPassant)
                {
                    capturedPieceSrc = (Color) active == Color.White
                        ? ((ushort) (src.GetSetBits()[0] - 8)).GetBoardValueOfIndex()
                        : ((ushort) (src.GetSetBits()[0] + 8)).GetBoardValueOfIndex();
                }

                //    Debug.WriteLine(
                //        $"{board.ActivePlayer}'s captured {capturedPiece} is being replaced. ulong={piecePlacement[opp][(int)capturedPiece]}");
                piecePlacement[opp][(int) capturedPiece] ^= capturedPieceSrc;
                //    Debug.WriteLine(
                //        $"{board.ActivePlayer}'s captured {capturedPiece} was replaced. ulong={piecePlacement[opp][(int)capturedPiece]}");
            }

            if (currentMove.MoveType == MoveType.Promotion)
            {
                var promotionPiece = (Piece) (currentMove.PromotionPiece + 1);
                //Debug.WriteLine($"Un-applying promotion to {promotionPiece}.");
                //Debug.WriteLine($"{promotionPiece} ulong is {piecePlacement[active][(int)promotionPiece].ToString()}");
                piecePlacement[active][(int) promotionPiece] &= ~src;
                //Debug.WriteLine(
                //    $"{promotionPiece} ulong is now {piecePlacement[active][(int)promotionPiece].ToString()}");
            }
            else if (currentMove.MoveType == MoveType.Castle)
            {
                var rookMove = MoveHelpers.GetRookMoveForCastleMove(currentMove);
                piecePlacement[active][(int) Piece.Rook] = piecePlacement[active][(int) Piece.Rook] ^
                                                           (rookMove.SourceValue | rookMove.DestinationValue);
            }

            return piecePlacement;
        }


        public LinkedListNode<BoardSnapshot> ExitVariation()
        {
            var currentTree = (MoveTree) CurrentMoveNode.List;
            var variationParentMove = currentTree.VariationParentNode;
            CurrentMoveNode = variationParentMove;
            var parentFen = currentTree.StartingFEN;
            var newBoard = new FenTextToBoard().Translate(parentFen);
            ApplyNewBoard(newBoard);
            TraverseForward();
            return variationParentMove;
        }

        #endregion
    }
}