using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.EventArgs;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.MoveValidation;

namespace ChessLib.Core.Translate
{
    public abstract class MoveTraversalService : IEquatable<MoveTraversalService>
    {
        private static readonly FenTextToBoard FenTextToBoard = new FenTextToBoard();

        private readonly MoveToSan _moveToSan = new MoveToSan();
        private LinkedListNode<BoardSnapshot> _currentMoveNode;
        private bool _pauseMoveEvents;

        protected MoveTraversalService(string fen)
        {
            var board = FenTextToBoard.Translate(fen);
            MainMoveTree = new MoveTree(board);

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

        public string InitialFen => MainMoveTree.StartingFen;
        public Board InitialBoard => MainMoveTree.InitialBoard;


        public string CurrentFen => CurrentMoveNode.Value.Board.CurrentFEN;
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
        public MoveTree CurrentTree => (MoveTree) CurrentMoveNode?.List;
        public LinkedListNode<BoardSnapshot> NextMoveNode => CurrentMoveNode.Next;

        public LinkedListNode<BoardSnapshot> PreviousMoveNode => GetPreviousNode(CurrentMoveNode);

        public bool Equals(MoveTraversalService other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var treeEq = MainMoveTree.Equals(other.MainMoveTree);
            return treeEq;
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

        public Board TraverseBackward()
        {
            var previousNode = PreviousMoveNode?.Value?.Board ?? InitialBoard;
            UnApplyMove();
            return previousNode;
        }

        public IBoard TraverseForward()
        {
            if (!HasNextMove)
            {
                return CurrentBoard;
            }

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


        protected LinkedListNode<BoardSnapshot> UnApplyMove()
        {
            if (!PreviousMoveNode.Value.IsNullMove)
            {
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
            Volatile.Read(ref MoveMade)?.Invoke(this, new MoveMadeEventArgs(moves.ToArray(), CurrentFen));
        }


        private void AddMoveToTree(MoveTree currentTree, BoardSnapshot moveStorageObject)
        {
            CurrentMoveNode = currentTree.AddMove(moveStorageObject);
        }

        private void SetSan(Move move, Board preMoveBoard, Board postMoveBoard)
        {
            if (string.IsNullOrEmpty(move.SAN))
            {
                move.SAN = _moveToSan.Translate(move, preMoveBoard, postMoveBoard);
            }
        }


        public LinkedListNode<BoardSnapshot> AddMove(Move move,
            MoveApplicationStrategy moveApplicationStrategy = MoveApplicationStrategy.ContinueMainLine)
        {
            ValidateMove(move);
            var tree = CurrentTree ?? MainMoveTree;
            if (moveApplicationStrategy == MoveApplicationStrategy.Variation)
            {
                tree = CurrentMoveNode.Value.AddVariation(CurrentMoveNode);
            }

            var preMoveBoard = CurrentBoard;
            var postMoveBoard = CurrentBoard.ApplyMoveToBoard(move);
            SetSan(move, preMoveBoard, postMoveBoard);
            var moveStorageObject = new BoardSnapshot(postMoveBoard, move) {Validated = true};
            AddMoveToTree(tree, moveStorageObject);
            CurrentBoard = postMoveBoard;
            return CurrentMoveNode;
        }

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


        private Board GetBoardFromBoardState(LinkedListNode<BoardSnapshot> previousNode)
        {
            if (previousNode.Value.IsNullMove)
            {
                return InitialBoard;
            }

            var previousState = previousNode.Value.Board;
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
            var capturedPiece = currentMove.Board.PieceCaptured;


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
            TraverseForward();
            return variationParentMove;
        }


        #region Go To Specific Moves

        public void GoToInitialState()
        {
            CurrentMoveNode = MainMoveTree.VariationParentNode;
            CurrentBoard = MainMoveTree.InitialBoard;
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
    }
}