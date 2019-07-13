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
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data
{
    public class BoardInfo : BoardBase
    {
        public MoveTree<MoveStorage> MoveTree { get; set; }

        public BoardInfo() : this(FENHelpers.FENInitial) { }

        public BoardInfo(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability,
            ushort? enPassantIdx, uint? halfMoveClock, uint fullMoveCount, bool validationException = true) : base(occupancy, activePlayer,
            castlingAvailability, enPassantIdx, halfMoveClock, fullMoveCount)
        {
            BoardValidator validator = new BoardValidator(this);
            validator.Validate(validationException);
        }

        public BoardInfo(string fen, bool is960 = false)
        {
            MoveTree = new MoveTree<MoveStorage>(null);
            PiecePlacement = fen.BoardFromFen(out Color active, out CastlingAvailability ca, out ushort? enPassant, out uint hmClock, out uint fmClock);
            ActivePlayer = active;
            CastlingAvailability = ca;
            EnPassantSquare = enPassant;
            HalfmoveClock = hmClock;
            FullmoveCounter = fmClock;
            Chess960 = is960;
            InitialFEN = this.ToFEN();
        }

        public ulong ActiveRookOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Rook);
        public ulong ActiveKnightOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Knight);
        public ulong ActivePawnOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Pawn);
        public ulong ActiveQueenOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Queen);
        public ulong ActiveBishopOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Bishop);

        public void ApplySANMove(string moveText)
        {
            var moveTranslatorService = new MoveTranslatorService(this);
            var move = moveTranslatorService.GenerateMoveFromText(moveText);
            ApplyMove(move);
        }

        public MoveError? ApplyMove(ushort sourceIndex, ushort destinationIndex, PromotionPiece? promotionPiece)
        {
            var moveTranslatorService = new MoveTranslatorService(this);
            var move = moveTranslatorService.GenerateMoveFromIndexes(sourceIndex, destinationIndex, promotionPiece);
            return ApplyMove(move);
        }

        public MoveError? ApplyMove(MoveExt move)
        {
            var pocSource = this.GetPieceOfColorAtIndex(move.SourceIndex);
            if (pocSource == null) throw new ArgumentException("No piece at source.");
            var moveValidator = new MoveValidator(this, move);
            var validationError = moveValidator.Validate();
            if (validationError.HasValue)
                throw new MoveException("Error with move.", validationError.Value, move, ActivePlayer);
            ApplyValidatedMove(move);
            return null;
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
            MoveTree.AddMove(new MoveStorage(this.ToFEN(), move, pocSource.Value.Piece, pocSource.Value.Color, san));
            var newBoard = this.ApplyMoveToBoard(move);
            PiecePlacement = newBoard.GetPiecePlacement();
            ActivePlayer = newBoard.ActivePlayer;
            CastlingAvailability = newBoard.CastlingAvailability;
            EnPassantSquare = newBoard.EnPassantSquare;
            HalfmoveClock = newBoard.HalfmoveClock;
            FullmoveCounter = newBoard.FullmoveCounter;

        }

        public MoveNode<MoveStorage> CurrentMove = null;

        public MoveStorage GetPreviousMove()
        {
            return CurrentMove.Previous.MoveData ?? CurrentMove.Parent.MoveData ?? null;
        }

        public MoveStorage[] GetNextMoves()
        {

            var lMoves = new List<MoveStorage>();
            if (CurrentMove.Next != null)
            {
                lMoves.Add(CurrentMove.Next.MoveData);
            }

            if (CurrentMove.Variations.Any())
            {
                lMoves.AddRange(CurrentMove.Variations.Select(x => x.FirstMove.MoveData));
            }

            return lMoves.ToArray();
        }

        public void TraverseForward(MoveStorage move)
        {
            var foundMove = FindNextNode(move);
            if (foundMove == null)
            {
                return;
            }

            CurrentMove = (MoveNode<MoveStorage>)foundMove;
            ApplyCurrentMoveToBoard();
        }

        private void ApplyCurrentMoveToBoard()
        {
            var board = new BoardInfo(CurrentMove.MoveData.FEN);
            this.PiecePlacement = board.GetPiecePlacement();
            this.ActivePlayer = board.ActivePlayer;
            this.CastlingAvailability = board.CastlingAvailability;
            this.EnPassantSquare = board.EnPassantSquare;
            this.HalfmoveClock = board.HalfmoveClock;
            this.FullmoveCounter = board.FullmoveCounter;
        }

        public void TraverseBackward()
        {
            var previousMove = FindPreviousNode();
            if (previousMove == null)
            {
                return;
            }

            CurrentMove = (MoveNode<MoveStorage>)previousMove;
            ApplyCurrentMoveToBoard();
        }

        private IMoveNode<MoveStorage> FindPreviousNode()
        {
            return CurrentMove.Previous ?? CurrentMove.Parent ?? null;
        }

        private IMoveNode<MoveStorage> FindNextNode(MoveStorage move)
        {
            if (CurrentMove.Next.MoveData == move)
            {
                return CurrentMove.Next;
            }

            foreach (var variation in CurrentMove.Variations)
            {
                if (variation.FirstMove.MoveData == move)
                {
                    return variation.FirstMove;
                }
            }

            return null;
        }

        public ulong GetPinnedPieces()
        {
            ulong pinned = 0;
            var xRayBishopAttacks = this.XRayBishopAttacks(ActivePlayerKingIndex);
            var xRayRookAttacks = this.XRayRookAttacks(ActivePlayerKingIndex);
            var oppQueenLocations = GetPiecePlacement().Occupancy(this.OpponentColor(), Piece.Queen);
            var oppBishopLocations = GetPiecePlacement().Occupancy(this.OpponentColor(), Piece.Bishop);
            var oppRookLocations = GetPiecePlacement().Occupancy(this.OpponentColor(), Piece.Rook);
            var bishopPinnedPieces = (oppBishopLocations | oppQueenLocations) & xRayBishopAttacks;
            var rookPinnedPieces = (oppRookLocations | oppQueenLocations) & xRayRookAttacks;
            var allPins = rookPinnedPieces | bishopPinnedPieces;
            while (allPins != 0)
            {
                var square = BitHelpers.BitScanForward(allPins);
                var squaresBetween = BoardHelpers.InBetween(square, ActivePlayerKingIndex);
                var piecesBetween = squaresBetween & GetPiecePlacement().Occupancy(ActivePlayer);
                if (piecesBetween.CountSetBits() == 1) pinned |= piecesBetween;
                allPins &= allPins - 1;
            }

            return pinned;
        }

        public bool DoesPieceAtSquareAttackSquare(ushort attackerSquare, ushort attackedSquare,
            Piece attackerPiece)
        {
            var attackedSquares = Bitboard.GetAttackedSquares(attackerPiece, attackerSquare, TotalOccupancy, Color.White);
            var attackedValue = attackedSquare.GetBoardValueOfIndex();
            return (attackedSquares & attackedValue) != 0;
        }

        public override object Clone()
        {
            return new BoardInfo(this.ToFEN());
        }
    }
}