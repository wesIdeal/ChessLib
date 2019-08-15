using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Validators.MoveValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChessLib.Data
{
    public class MoveTraversalService
    {
        private string _initialFEN;
        public string InitialFEN
        {
            get => _initialFEN;
            protected set
            {
                _initialFEN = value;
                MainMoveTree = new MoveTree(null, _initialFEN);
                InitialBoard = new BoardInfo(_initialFEN);
                Board = (BoardInfo)InitialBoard.Clone();
            }
        }
        private int _depth
        {
            get
            {
                var count = 0;
                var currNode = (MoveTree)CurrentTree?.VariationParentNode?.List;
                while (currNode != null)
                {
                    currNode = (MoveTree)currNode?.VariationParentNode?.List;
                    count++;
                }
                return count;
            }
        }
        public IBoard InitialBoard { get; private set; }
        public BoardInfo Board { get; private set; }
        public string CurrentFEN => Board.CurrentFEN;
        public MoveTree MainMoveTree { get; protected set; }
        public LinkedListNode<MoveStorage> CurrentMoveNode { get; set; }
        
        public MoveTraversalService(string fen)
        {
            Board = new BoardInfo(fen);
            MainMoveTree = new MoveTree(null, fen);
            CurrentMoveNode = MainMoveTree.First;
        }



        public event EventHandler<MoveMadeEventArgs> MoveMade;
        protected void OnMoveMade(MoveExt moveMade, IBoard currentBoard)
        {
            var eventArgs = new MoveMadeEventArgs(moveMade, Board, GetNextMoves(), GetSquaresUpdated(moveMade));
            Volatile.Read(ref MoveMade)?.Invoke(this, eventArgs);
        }

        public static ushort[] GetSquaresUpdated(IMoveExt move)
        {
            if (move.MoveType == MoveType.Castle)
            {
                if (move == MoveHelpers.WhiteCastleKingSide)
                {
                    return new ushort[] { 4, 5, 6, 7 };
                }
                if (move == MoveHelpers.WhiteCastleQueenSide)
                {
                    return new ushort[] { 2, 3, 4 };
                }
                if (move == MoveHelpers.BlackCastleKingSide)
                {
                    return new ushort[] { 60, 61, 62, 63 };
                }
                else //if (move == MoveHelpers.BlackCastleQueenSide)
                {
                    return new ushort[] { 56, 58, 59, 60 };
                }
            }

            var rv = new ushort[] { move.SourceIndex, move.DestinationIndex };

            if (move.MoveType == MoveType.EnPassant)
            {
                ushort extra;
                if (move.DestinationIndex.GetRank() == 5)
                {
                    extra = (ushort)(move.DestinationIndex - (ushort)8);
                }
                else
                {
                    extra = (ushort)(move.DestinationIndex + (ushort)8);
                }
                var nRv = rv.ToList();
                nRv.Add(extra);
                rv = nRv.ToArray();
            }
            return rv;
        }

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
            var lMoves = new List<LinkedListNode<MoveStorage>>();
            lMoves.Add(CurrentMoveNode.Next);
            var nextMove = CurrentMoveNode.Next.Value;
            if (nextMove.Variations.Any())
            {
                lMoves.AddRange(nextMove.Variations.Select(x => x.First));
            }
            return lMoves.ToArray();
        }

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
            if (node != null)
            {
                while (!CurrentMoveNode.Equals(node) && CurrentMoveNode != null && !CurrentMoveNode.Value.IsNullMove)
                {
                    UnapplyMove();
                }
            }
            else
            {
                GoToInitialState();
            }
            return CurrentMoveNode;
        }




        protected LinkedListNode<MoveStorage> UnapplyMove()
        {
            //Debug.WriteLine($"Before unapply, FEN is:\t{CurrentFEN}");
            WriteDebugInfo($"{new string('*', 20)}\r\nUnapplying {CurrentMoveNode.Value}");
            if (!PreviousMoveNode.Value.IsNullMove)
            {
                UnapplyMove(PreviousMoveNode);
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

        private void WriteDebugInfo(string message)
        {
            var msg = message.Replace("\r\n", $"\r\n{new string(' ', _depth * 2)}");
            Debug.WriteLine(new string(' ', _depth * 2), message);
        }

        /// <summary>
        /// Traverse the tree to the next move.
        /// </summary>
        /// <param name="move">The move that should be set to current.</param>
        /// <returns>Board state for the resulting board. null if the end of the tree was reached.
        /// <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>
        public IBoard TraverseForward(MoveStorage move)
        {
            LinkedListNode<MoveStorage> foundMove;
            try
            {
                foundMove = FindNextMoveNodeFromMove(move);
            }
            catch (Exception exc)
            {
                throw new MoveException($"TraverseForward({move}) error. Move not found.");
            }
            ApplyMoveToBoard(move);
            CurrentMoveNode = foundMove;
            return Board;

        }

        private LinkedListNode<MoveStorage> FindNextMoveNodeFromMove(MoveStorage move)
        {
            return GetNextMoveNodes().First(x => x.Value.Equals(move));
        }

        /// <summary>
        /// Traverse the tree to the next move.
        /// </summary>
        /// <param name="move">The move that should be set to current.</param>
        /// <param name="indexesUpdated">An array of board indexes for squares affected by move. Handy for a UI to call an update for these squares.</param>
        /// <returns>Board state for the resulting board. null if the end of the tree was reached.
        /// <exception cref="MoveTraversalException">Thrown when the given move is not in the 'next moves' list.</exception>


        #region Go To Specific Moves
        public void GoToInitialState()
        {
            var bi = new BoardInfo(InitialFEN);
            ApplyNewBoard(bi);
            while (CurrentTree.VariationParentNode != null)
            {
                CurrentMoveNode = CurrentTree.VariationParentNode;
            }
            CurrentMoveNode = CurrentTree.First;
        }

        public void GoToLastMove()
        {
            LinkedListNode<MoveStorage> node = NextMoveNode;
            while (node != null)
            {
                TraverseForward();
                node = NextMoveNode;
            }
        }
        #endregion

        #region Move Application

        #region Variations
        protected LinkedListNode<MoveStorage> ApplySANVariationMove(string moveText)
        {
            TraverseBackward();
            var move = TranslateSANMove(moveText);
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
                var variationParentFEN = CurrentFEN;
                ApplyMoveToBoard(move);
                var moveStorageObj = new MoveStorage(Board, move, capturedPiece) { Validated = true };
                CurrentMoveNode = CurrentMoveNode.Next.Value.AddVariation(CurrentMoveNode, moveStorageObj, variationParentFEN);
                WriteDebugInfo($"Applying variation {move}");
                return CurrentMoveNode;
            }
            catch (Exception e)
            {
                throw new MoveException($"Issue while applying move {move}.", e);
            }
        }
        #endregion

        #region Regular Moves
        public LinkedListNode<MoveStorage> ApplySANMove(string moveText, MoveApplicationStrategy moveApplicationStrategy)
        {
            Debug.WriteLine($"Applying move {moveText}");
            if (moveApplicationStrategy == MoveApplicationStrategy.Variation)
            {
                return ApplySANVariationMove(moveText);
            }
            MoveExt move = TranslateSANMove(moveText);
            move.SAN = moveText;
            return ApplyMove(move);
        }

        public LinkedListNode<MoveStorage> ApplyMove(MoveExt move, MoveApplicationStrategy moveApplicationStrategy = MoveApplicationStrategy.ContinueMainLine)
        {
            ValidateMove(move);
            OnMoveMade(move, Board);
            return ApplyValidatedMove(move);
        }

        protected LinkedListNode<MoveStorage> ApplyValidatedMove(MoveExt move, MoveApplicationStrategy moveApplicationStrategy = MoveApplicationStrategy.ContinueMainLine)
        {
            Debug.WriteLine($"Before applying move {move}, FEN is:\t{Board.CurrentFEN}");
            var capturedPiece = GetCapturedPiece(move);
            if (string.IsNullOrEmpty(move.SAN))
            {
                move.SAN = GetMoveText(move);
            }
            WriteDebugInfo($"Applying move {move}");
            ApplyMoveToBoard(move);
            var moveStorageObject = new MoveStorage(Board, move, capturedPiece) { Validated = true };
            CurrentMoveNode = CurrentTree.AddMove(moveStorageObject);
            Debug.WriteLine($"After applying move {move}, FEN is:\t{Board.CurrentFEN}");
            return CurrentMoveNode;
        }
        #endregion
        protected void ValidateMove(MoveExt move)
        {
            if (move is MoveStorage)
            {
                if ((move as MoveStorage).Validated) { return; }
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
                capturedPiece = new PieceOfColor() { Color = Board.ActivePlayer.Toggle(), Piece = Piece.Pawn };
            }
            return capturedPiece?.Piece;
        }

       

        protected void ApplyMoveToBoard(MoveExt move)
        {
            var newBoard = BoardHelpers.ApplyMoveToBoard(Board, move);
            ApplyNewBoard(newBoard);
        }

        /// <summary>
        /// Applies the given board parameter to this board
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
        }

        private void UnapplyMove(LinkedListNode<MoveStorage> previousNode)
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
            var piece = currentMove.MoveType == MoveType.Promotion ? Piece.Pawn :
            Board.GetPieceAtIndex(currentMove.DestinationIndex);

            Debug.Assert(piece.HasValue, "Piece for unapply() has no value.");
            var src = currentMove.DestinationValue;
            var dst = currentMove.SourceValue;
            var board = (IBoard)Board.Clone();
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

        private MoveExt TranslateSANMove(string moveText)
        {
            var moveTranslatorService = new MoveTranslatorService(Board);
            var move = moveTranslatorService.GetMoveFromSAN(moveText);
            return move;
        }

        public LinkedListNode<MoveStorage> ExitVariation()
        {
            var currentTree = (MoveTree)CurrentMoveNode.List;
            var parentMove = currentTree.VariationParentNode;
            var parentFEN = currentTree.StartingFEN;
            Board = new BoardInfo(parentFEN);
            CurrentMoveNode = parentMove;
            TraverseForward();
            return parentMove;
        }

        #endregion
    }
}
