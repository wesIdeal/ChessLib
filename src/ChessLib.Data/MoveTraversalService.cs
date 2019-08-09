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
using System.Linq;
using System.Threading.Tasks;

namespace ChessLib.Data
{
    public class MoveTraversalService : IMoveTraversalService
    {
        public string InitialFEN { get; private set; }
        public IBoard InitialBoard { get; private set; }
        public BoardInfo Board { get; private set; }
        public MoveTree<MoveStorage> MoveTree { get; private set; }
        public MoveNode<MoveStorage> CurrentMove { get; set; }

        public MoveTraversalService(Game<MoveStorage> game) : this(game.TagSection.FENStart, ref game.MoveSection) { }

        public MoveTraversalService(string initialFEN, ref MoveTree<MoveStorage> moveTree)
        {
            MoveTree = moveTree;
            CurrentMove = moveTree.HeadMove;
            InitialFEN = initialFEN;
            Board = new BoardInfo(InitialFEN);
            InitialBoard = (BoardInfo)Board.Clone();
        }

        public MoveTraversalService() : this(new Game<MoveStorage>())
        {
        }

        public string CurrentFEN => Board.CurrentFEN;

        public static ushort[] GetSquaresUpdated(MoveExt move)
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
            return nextMoveNodes.Select(x => x.MoveData).ToArray();
        }

        private MoveNode<MoveStorage>[] GetNextMoveNodes()
        {
            if (CurrentMove.Next == null)
            {
                return new MoveNode<MoveStorage>[] { };
            }
            var lMoves = new List<MoveNode<MoveStorage>>();
            lMoves.Add(CurrentMove.Next);
            if (CurrentMove.Next.Variations.Any())
            {
                lMoves.AddRange(CurrentMove.Next.Variations.Select(x => x.HeadMove));
            }
            return lMoves.ToArray();
        }

        public IBoard TraverseForward()
        {
            var foundMove = GetNextMoveNodes();
            if (foundMove.Any() && foundMove != null)
            {
                ApplyMoveToBoard(foundMove[0].MoveData);
                CurrentMove = foundMove[0];
                return Board;
            }
            return Board;
        }

        public IBoard TraverseForward(out ushort[] indexesUpdated)
        {
            throw new NotImplementedException();
        }


        public IBoard TraverseBackward()
        {
            Debug.WriteLine($"Before unapply, FEN is:\t{CurrentFEN}");
            var previousMoveNode = FindPreviousMove(CurrentMove);
            MoveStorage rv;
            if (previousMoveNode != null)
            {
                rv = CurrentMove.MoveData;
                UnapplyMove(previousMoveNode.MoveData.BoardState);
                CurrentMove = previousMoveNode;
            }
            else
            {
                Board = (BoardInfo)InitialBoard.Clone();
                return InitialBoard;
            }
            Debug.WriteLine($"After unapply, FEN is:\t{CurrentFEN}");
            return Board;
        }

        public IBoard TraverseBackward(out ushort[] indexesUpdated)
        {
            throw new NotImplementedException();
        }

        public IBoard TraverseForward(MoveStorage move)
        {
            var foundMove = FindNextNode(move);
            if (foundMove != null)
            {
                BoardInfo board;
                ApplyMoveToBoard(move);
                CurrentMove = foundMove;
                return Board;
            }
            return Board;
        }

        public IBoard TraverseForward(MoveStorage move, out ushort[] indexesUpdated)
        {
            throw new NotImplementedException();
        }

        private MoveNode<MoveStorage> FindPreviousMove(in MoveNode<MoveStorage> move)
        {
            if (move.IsNullNode)
            {
                return null;
            }

            if (move.Previous == null)
            {
                var moveParent = move.Parent;
                if (moveParent == null)
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

        #region Go To Specific Moves
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
        #endregion

        #region Move Application
        public MoveNode<MoveStorage> ApplyMove(MoveExt move)
        {
            var pocSource = Board.GetPieceOfColorAtIndex(move.SourceIndex);
            if (pocSource == null) throw new ArgumentException("No piece at source.");
            var moveValidator = new MoveValidator(Board, move);
            var validationError = moveValidator.Validate();
            if (validationError != MoveError.NoneSet)
            {
                throw new MoveException("Error with move.", validationError, move, Board.ActivePlayer);
            }
            return ApplyValidatedMove(move);
        }

        public MoveNode<MoveStorage> ApplyValidatedMove(MoveExt move)
        {
            Debug.WriteLine($"Before applying move {move}, FEN is:\t{Board.CurrentFEN}");
            var pocSource = Board.GetPieceOfColorAtIndex(move.SourceIndex);
            var capturedPiece = Board.GetPieceOfColorAtIndex(move.DestinationIndex);
            if (capturedPiece == null && move.MoveType == MoveType.EnPassant)
            {
                capturedPiece = new PieceOfColor() { Color = Board.ActivePlayer.Toggle(), Piece = Piece.Pawn };
            }
            ApplyMoveToBoard(move);
            CurrentMove = MoveTree.AddMove(new MoveStorage(Board, move, capturedPiece?.Piece));
            Debug.WriteLine($"After applying move {move}, FEN is:\t{Board.CurrentFEN}");
            return CurrentMove;
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
            var pieces = UnApplyPiecesFromMove(CurrentMove.MoveData);
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



        #endregion
    }
}
